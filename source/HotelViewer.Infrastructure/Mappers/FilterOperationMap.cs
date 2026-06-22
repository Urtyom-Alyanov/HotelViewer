using System.Reflection;
using HotelViewer.Domain.Helper;
using LanguageExt;
using static LanguageExt.Prelude;

namespace HotelViewer.Infrastructure.Mappers;

public static class FilterOperationMapper {
  /// <summary>
  /// Преобразует доменный оператор фильтрации в SQL-оператор для MS Access
  /// </summary>
  /// <param name="op">Оператор</param>
  public static Option<string> Map(FilterOp op) => op switch {
    FilterOp.Eq => Some("="),
    FilterOp.Like => Some("LIKE"),
    FilterOp.Gt => Some(">"),
    FilterOp.Lt => Some("<"),
    FilterOp.GtEq => Some(">="),
    FilterOp.LtEq => Some("<="),
    FilterOp.In => Some("IN"),
    _ => None
  };

  /// <summary>
  /// Генерация мест для посадки значений
  /// </summary>
  /// <param name="op">Оператор</param>
  /// <param name="count">Количество</param>
  /// <returns>Строка с операторами</returns>
  public static string MapPlaceholders(FilterOp op, Option<uint> count) => op switch {
    FilterOp.In => count
      .Match(
        countUnwrapped => $"({string.Join(", ", Enumerable.Repeat("?", (int)countUnwrapped))})",
        () => "?"
        ),
    _ => "?"
  };

  /// <summary>
  /// Получить "сырое" знрачение для базы данных
  /// </summary>
  /// <param name="value">Value Object</param>
  /// <returns>"сырое" значение</returns>
  public static object UnwrapValueObject(object value) {
    var type = value.GetType();

    if (type.IsPrimitive || value is string || value is DateTime || value is decimal)
      return value;

    var toDbMethod = type.GetMethod(
      "ToDbValue",
      BindingFlags.Public | BindingFlags.Instance
    );

    if (toDbMethod != null)
      return toDbMethod.Invoke(value, null) ?? DBNull.Value;

    var valueProperty = type.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);

    if (valueProperty != null)
      return valueProperty.GetValue(value) ?? DBNull.Value;

    return value;
  }

  /// <summary>
  /// Преобразует значение в зависимости от оператора
  /// </summary>
  /// <param name="op">операция</param>
  /// <param name="value">значение</param>
  /// <returns>значение для SQL</returns>
  public static object MapValue(FilterOp op, object value) {
    if (op == FilterOp.In && value is System.Collections.IEnumerable list && value is not string) {
      return list.Cast<object>().Select(UnwrapValueObject).ToList();
    }

    var valueUnwrapped = UnwrapValueObject(value);

    return op switch {
      FilterOp.Like => $"%{valueUnwrapped}%",
      _ => valueUnwrapped
    } ?? DBNull.Value;
  }
}
