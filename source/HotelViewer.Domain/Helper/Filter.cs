using System.Linq.Expressions;
using System.Reflection;

namespace HotelViewer.Domain.Helper;

/// <summary>
/// Операторы сравнения
/// </summary>
public enum FilterOp { Eq, Like, Gt, Lt, GtEq, LtEq, In }

/// <summary>
/// Единичный критерий фильтрации
/// </summary>
public record FilterCriterion<TEntity>(
  Expression<Func<TEntity, object>> PropertySelector,
  object Value,
  FilterOp Op = FilterOp.Eq
);

/// <summary>
/// Набор фильтров
/// </summary>
public record Filter<TEntity>(params FilterCriterion<TEntity>[] Criteria) {
  public static Filter<TEntity> Empty => new();

  /// <summary>
  /// Добавить & в условие
  /// </summary>
  /// <param name="criterion">Критерий</param>
  /// <returns>Объект фильтра</returns>
  public Filter<TEntity> And(Expression<Func<TEntity, object>> selector, object value, FilterOp op = FilterOp.Eq) =>
    new(Criteria.Append(new FilterCriterion<TEntity>(selector, value, op)).ToArray());
}
