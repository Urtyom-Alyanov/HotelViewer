namespace HotelViewer.Domain.Helper;

/// <summary>
/// Операторы сравнения
/// </summary>
public enum FilterOp { Eq, Like, Gt, Lt, GtEq, LtEq, In }

/// <summary>
/// Единичный критерий фильтрации
/// </summary>
public record FilterCriterion<TField>(TField Field, object Value, FilterOp Op = FilterOp.Eq) where TField : Enum;

/// <summary>
/// Набор фильтров
/// </summary>
public record Filter<TField>(params FilterCriterion<TField>[] Criteria) where TField : Enum {
  public static Filter<TField> Empty => new();
}
