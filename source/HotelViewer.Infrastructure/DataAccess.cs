using System.Data;
using System.Data.OleDb;
using System.Text.RegularExpressions;
using LanguageExt;
using static LanguageExt.Prelude;

namespace HotelViewer.Infrastructure;

public abstract record DataAccessError(string Message);

public record DriverNotInstalled() : DataAccessError("Драйвер базы данных не установлен (требуется Microsoft Access Database Engine 2007 или новее)");
public record NormalOperatingSystem() : DataAccessError("Установлена нормальная ОС! Установите ОС Windows и запустите приложение там.");
public record DatabaseConnectionError(Exception Ex) : DataAccessError($"Ошибка подключения к БД: {Ex.Message}");
public record QueryExecutionError(string Query, Exception Ex) : DataAccessError($"Ошибка выполнения запроса [{Query}]: {Ex.Message}");
public record UnknownConnectionError(Exception Ex) : DataAccessError($"Критическая ошибка инфраструктуры: {Ex.Message}");
public record FileNotFoundError(string Path) : DataAccessError($"Файл базы данных не найден в {Path}");

/// <summary>
/// Инициализация базы данных и низкоуровневый интерфейс
/// </summary>
public class DataAccess {
  private static readonly Regex ProviderRegex = new(@"^Microsoft\.ACE\.OLEDB\.(\d+\.\d+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
  private readonly string _connectionString;

  private DataAccess(string databasePath, string provider) {
    _connectionString = $"Provider={provider};Data Source={databasePath};Persist Security Info=False;";
  }


  /// <summary>
  /// Создание подключения к базе данных.
  /// Получает самого нового провайдера для OleDb M$ Access.
  /// На нормальных ОС не работает, только на Windows.
  /// </summary>
  /// <param name="databasePath">Путь к базе данных MS Access</param>
  /// <returns>Доступ или ошибка</returns>
  public static Either<DataAccessError, DataAccess> CreateConnection(string databasePath) {
    if (!File.Exists(databasePath))
      return Left<DataAccessError, DataAccess>(new FileNotFoundError(databasePath));

    if (!OperatingSystem.IsWindows())
      return Left<DataAccessError, DataAccess>(new NormalOperatingSystem());

    try {
      var enumerator = new OleDbEnumerator();
      var dataTable = enumerator.GetElements();

      var installedProvider = dataTable.AsEnumerable()
          .Select(dataRow => dataRow["SOURCES_NAME"].ToString())
          .Where(name => name != null)
          .Select(name => {
            var match = ProviderRegex.Match(name!);
            return new {
              FullName = name,
              IsValid = match.Success,
              Version = match.Success ? Version.Parse(match.Groups[1].Value) : new Version(0, 0)
            };
          })
          .Where(x => x.IsValid)
          .OrderByDescending(x => x.Version)
          .Select(x => x.FullName)
          .FirstOrDefault();

      return installedProvider != null
          ? Right<DataAccessError, DataAccess>(new DataAccess(databasePath, installedProvider))
          : Left<DataAccessError, DataAccess>(new DriverNotInstalled());
    }
    catch (Exception ex) {
      return Left<DataAccessError, DataAccess>(new UnknownConnectionError(ex));
    }
  }

  /// <summary>
  /// Исполнить низкоуровневую команду
  /// </summary>
  /// <param name="query">Запрос</param>
  /// <param name="mapFunction">Функция с дальнейшим парсингом</param>
  /// <typeparam name="T">Возвращаемый тип</typeparam>
  /// <returns>Результат исполнения команды</returns>
  private Either<DataAccessError, T> ExecuteCommand<T>(string query, Func<OleDbCommand, T> mapFunction) {
    if (!OperatingSystem.IsWindows())
      return Left<DataAccessError, T>(new NormalOperatingSystem());

    try {
      using var connection = new OleDbConnection(_connectionString);
      connection.Open();

      using var command = new OleDbCommand(query, connection);

      var result = mapFunction(command);

      return Right<DataAccessError, T>(result);
    }
    catch (OleDbException ex) {
      if (ex.Message.Contains("формат базы данных") || ex.Message.Contains("не удалось подключиться")) {
        return Left<DataAccessError, T>(new DatabaseConnectionError(ex));
      }

      return Left<DataAccessError, T>(new QueryExecutionError(query, ex));
    }
    catch (Exception ex) {
      return Left<DataAccessError, T>(new UnknownConnectionError(ex));
    }
  }

  /// <summary>
  /// Загружат таблицу в память
  /// </summary>
  /// <param name="query">Запрос</param>
  /// <param name="parameters">Парпаметры</param>
  /// <returns>Таблица</returns>
  public Either<DataAccessError, DataTable> LoadTable(string query, params object[] parameters) => ExecuteCommand(query, command => {
    var table = new DataTable();
#pragma warning disable CA1416
    command.Parameters.AddRange(
        parameters.Select(p => new OleDbParameter("?", p ?? DBNull.Value)).ToArray()
    );
    using var adapter = new OleDbDataAdapter(command);
    adapter.Fill(table);
#pragma warning restore CA1416
    return table;
  });

  /// <summary>
  /// Сохранить изменённые данные из SELECT запроса
  /// </summary>
  /// <param name="query">SELECT запрме</param>
  /// <param name="table">Ефблица</param>
  /// <returns>Ошибка или ничего</returns>
  public Either<DataAccessError, Unit> SaveTable(string query, DataTable table) => ExecuteCommand(query, command => {
#pragma warning disable CA1416
    using var adapter = new OleDbDataAdapter(command);
    using var builder = new OleDbCommandBuilder(adapter);

    adapter.InsertCommand = builder.GetInsertCommand();
    adapter.UpdateCommand = builder.GetUpdateCommand();
    adapter.DeleteCommand = builder.GetDeleteCommand();

    adapter.Update(table);
#pragma warning restore CA1416
    return unit;
  });
}
