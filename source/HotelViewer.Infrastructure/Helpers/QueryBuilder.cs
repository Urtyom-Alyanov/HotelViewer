using System.Linq.Expressions;
using HotelViewer.Domain.Helper;
using HotelViewer.Infrastructure.Mappers;
using LanguageExt;
using static LanguageExt.Prelude;

namespace HotelViewer.Infrastructure.Helpers;

public record QueryWithParameters(string Sql, object Parameters);

public class QueryBuilder<TEntity>(
  string baseQuery,
  Func<Expression<Func<TEntity, object>>, Option<string>> propertyMap) {
  /// <summary>
  /// Построить запрос на основе базового запроса и маппера
  /// </summary>
  /// <param name="filter">Фильтр по критериям</param>
  /// <param name="sort">Сортировка по критерию</param>
  /// <param name="limit">Количество запрашиваемоего</param>
  /// <param name="offset">Отступ</param>
  /// <returns>Строка с запросом</returns>
  public QueryWithParameters Build(
    Option<Filter<TEntity>> filter,
    Option<Sort<TEntity>> sort,
    Option<uint> limit,
    Option<uint> offset
  ) {
    var filterData = filter
      .Map(f => f.Criteria
        .Select(c =>
          from col in propertyMap(c.PropertySelector)
          from op in FilterOperationMapper.Map(c.Op)
          select (
            Clause: $"{col} {op} ?",
            Value: FilterOperationMapper.MapValue(c.Op, c.Value)
          ))
        .Somes()
        .ToList())
      .IfNone(new List<(string Clause, object Value)>());

    var whereSql = filterData.Count > 0
      ? " WHERE " + string.Join(" AND ", filterData.Select(x => x.Clause))
      : "";

    var parameters = filterData.Select(x => x.Value).ToArray();

    var orderSql = sort
      .Bind(s => propertyMap(s.PropertySelector).Map(col =>
        $" ORDER BY {col} {(s.Ascending ? "ASC" : "DESC")}"))
      .IfNone("");

    var query = baseQuery + whereSql + orderSql;

    return new(query, parameters);
  }
}
