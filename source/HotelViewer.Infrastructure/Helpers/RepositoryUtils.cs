using System.Data;
using HotelViewer.Domain.Helper;
using HotelViewer.Domain.Repository;
using HotelViewer.Infrastructure.Mappers;
using LanguageExt;
using static LanguageExt.Prelude;

namespace HotelViewer.Infrastructure.Helpers;

public class RepositoryUtils<TEntity, TMapper>(
  DataAccess db,
  QueryBuilder<TEntity> queryBuilder) where TMapper : IEntityMapper<TEntity> {
  /// <summary>
  /// Стандартная реализация метода репозитория FindMany
  /// </summary>
  /// <param name="filter">Критерии фильтрации</param>
  /// <param name="sort">Критерии сортировки</param>
  /// <param name="limit">Ограничение (в Access такой штуки нет)</param>
  /// <param name="offset">Отступ (в Access такой штуки нет)</param>
  /// <returns>Ошибка репозитория или коллекция сущностей</returns>
  public Either<RepositoryError, IEnumerable<TEntity>> FindMany(
    Option<Filter<TEntity>> filter,
    Option<Sort<TEntity>> sort,
    Option<uint> limit,
    Option<uint> offset
  ) {
    var query = queryBuilder.Build(filter, sort, limit, offset);

    return db
      .LoadTable(query.Sql, query.Parameters)
      .MapLeft(DataAccessErrorToRepositoryErrorMapper.Map)
      .Map(table => {
        var enumCollection = table
          .AsEnumerable()
          .Select(TMapper.MapFromDb)
          .ToList()
          .AsEnumerable();

        if (offset.IsSome) enumCollection = enumCollection.Skip((int)offset.IfNone(0u));
        if (limit.IsSome) enumCollection = enumCollection.Take((int)limit.IfNone(0u));

        return enumCollection;
      }
      );
  }

  /// <summary>
  /// Стандартная реализация метода репозитория FindOne
  /// </summary>
  /// <param name="filter">Критерии фильтрации</param>
  /// <param name="sort">Критерии сортировки (если критерии фильтрации не уникальны)</param>
  /// <returns>Ошибка репозитория или сущность</returns>
  public Either<RepositoryError, TEntity> FindOne(Option<Filter<TEntity>> filter, Option<Sort<TEntity>> sort) =>
    FindMany(filter, sort, 1u, 0u)
      .Bind(list => {
        var first = list.FirstOrDefault();
        return first != null
          ? Right<RepositoryError, TEntity>(first)
          : Left<RepositoryError, TEntity>(new EntityNotFoundByFilter<TEntity>(filter));
      });

  /// <summary>
  /// Получить количество сущностей в базе данных
  /// </summary>
  /// <param name="filter">Критерии фильтрации</param>
  /// <returns>Ошибка репозитория или количество сущностей</returns>
  public Either<RepositoryError, uint> Count(Option<Filter<TEntity>> filter) {
    var query = queryBuilder.Build(filter, None, None, None);
    var countSql = query.Sql.Replace("SELECT *", "SELECT COUNT(*)");

    return db.LoadTable(countSql, query.Parameters)
      .MapLeft(DataAccessErrorToRepositoryErrorMapper.Map)
      .Map(table => Convert.ToUInt32(table.Rows[0][0]));
  }

  /// <summary>
  /// Удалить сущность/сущности в зависимости от критериев фильтрации
  /// </summary>
  /// <param name="filter">Критерии фильтрации</param>
  /// <returns>Ничего при успехе</returns>
  public Either<RepositoryError, Unit> Drop(Filter<TEntity> filter) {
    var (sql, parameters) = queryBuilder.Build(filter, None, None, None);
    var deleteSql = sql.Replace("SELECT *", "DELETE");

    return db.Mutate(deleteSql, parameters)
      .MapLeft(DataAccessErrorToRepositoryErrorMapper.Map)
      .Map(_ => unit);
  }

  /// <summary>
  /// Сохранить сущность
  /// </summary>
  /// <param name="query">Запрос и параметры</param>
  /// <param name="entity">сущнность</param>
  /// <returns></returns>
  public Either<RepositoryError, Unit> Save(QueryWithParameters query, TEntity entity) =>
    db.LoadTable(query.Sql, query.Parameters)
      .Map(table => TMapper.MapIntoDb(entity, table))
      .Bind(table => db.SaveTable(query.Sql, table))
      .MapLeft(DataAccessErrorToRepositoryErrorMapper.Map);
}
