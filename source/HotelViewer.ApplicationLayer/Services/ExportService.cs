using System.Reflection;
using System.Text;
using ClosedXML.Excel;
using HotelViewer.ApplicationLayer.Errors;
using HotelViewer.Domain.Repository;
using LanguageExt;
using static LanguageExt.Prelude;

namespace HotelViewer.ApplicationLayer.Services;

public class ExportService<TEntity, TEntityId>(IRepository<TEntity, TEntityId> repository) {
  /// <summary>
  /// Экспортировать отчёт в CSV файл
  /// </summary>
  /// <param name="filePath">Путь к CSV файлу</param>
  /// <returns>Путь к CSV файлу</returns>
  public Either<ApplicationError, string> ExportToCsv(string filePath) =>
    repository.FindMany(None, None, None, None)
      .MapLeft<ApplicationError>(err => new RepositoryFailure(err))
      .Bind(entities => {
        try {
          var properties = typeof(TEntity)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => !p.Name.Contains("Password"))
            .ToList();

          var csv = new StringBuilder();

          var headers = properties.Select(p => p.Name);
          csv.AppendLine(string.Join(";", headers));

          foreach (var entity in entities) {
            var values = properties.Select(p => {
              var val = p.GetValue(entity);
              return FormatValue(val);
            });

            csv.AppendLine(string.Join(";", values.Select(v => $"\"{v}\"")));
          }

          File.WriteAllText(filePath, csv.ToString(), Encoding.UTF8);
          return Right<ApplicationError, string>(filePath);
        }
        catch (Exception ex) {
          return Left<ApplicationError, string>(new ValidationError($"Ошибка записи файла: {ex.Message}"));
        }
      });

  /// <summary>
  /// Экспортировать отчёт в Excel файл
  /// </summary>
  /// <param name="filePath">Путь к XML файлу</param>
  /// <returns>Путь к XML файлу</returns>
  public Either<ApplicationError, string> ExportToXML(string filePath)
  => repository.FindMany(None, None, None, None)
    .MapLeft<ApplicationError>(err => new RepositoryFailure(err))
    .Bind(entities => {
      try {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(typeof(TEntity).Name);

        var properties = typeof(TEntity).GetProperties()
          .Where(p => !p.Name.Contains("Password")).ToList();

        for (int i = 0; i < properties.Count; i++) {
          var cell = worksheet.Cell(1, i + 1);
          cell.Value = properties[i].Name;
          cell.Style.Font.Bold = true;
          cell.Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        var list = entities.ToList();
        for (int row = 0; row < list.Count; row++) {
          for (int col = 0; col < properties.Count; col++) {
            var val = properties[col].GetValue(list[row]);
            worksheet.Cell(row + 2, col + 1).Value = DomainObjectFormatter.Format(val);
          }
        }

        worksheet.Columns().AdjustToContents();
        workbook.SaveAs(filePath);

        return Right<ApplicationError, string>(filePath);
      }
      catch (Exception ex) {
        return Left<ApplicationError, string>(new ValidationError(ex.Message));
      }
    });

  /// <summary>
  /// Перевод из Value objects в значения
  /// </summary>
  /// <param name="value">значение</param>
  /// <returns>"сырое" значение</returns>
  private string FormatValue(object? value) {
    if (value == null)
      return "";

    var type = value.GetType();

    if (type.IsPrimitive || value is string || value is DateTime || value is decimal)
      return value.ToString() ?? "";

    var toDbMethod = type.GetMethod("ToDbValue");
    if (toDbMethod != null)
      return toDbMethod.Invoke(value, null)?.ToString() ?? "";

    var valueProp = type.GetProperty("Value");
    if (valueProp != null)
      return valueProp.GetValue(value)?.ToString() ?? "";

    return value.ToString() ?? "";
  }
}
