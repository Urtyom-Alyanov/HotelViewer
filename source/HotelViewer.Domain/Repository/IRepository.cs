using System.Reflection;
using HotelViewer.Domain.Helper;
using LanguageExt;

namespace HotelViewer.Domain.Repository;

public abstract record RepositoryError(string Message);
public record EntityNotFoundById<TEntityId>(TEntityId Id) : RepositoryError($"Сущность с ID {Id} не найдена в системе.");
public record EntityNotFoundByFilter<TEntity>(Option<Filter<TEntity>> Filter) : RepositoryError($"Сущность с такими фильтрами не найдена. {Filter}");

public record InfrastructureFault(string Detail) : RepositoryError($"Сбой инфраструктуры данных: {Detail}");

/// <summary>
/// Интерфейс репозитория сущности
/// </summary>
/// <typeparam name="TEntity">Сущность</typeparam>
/// <typeparam name="TEntityId">Идентификатор сущности</typeparam>
public interface IRepository<TEntity, in TEntityId> {
  /// <summary>
  /// Получить сущностей
  /// </summary>
  /// <param name="filter">Критерии фильтрации</param>
  /// <param name="sort">Критерии сортировки</param>
  /// <param name="limit">Лимит</param>
  /// <param name="offset">Отступ</param>
  /// <returns>Итерируемый объект с сущностями</returns>
  public Either<RepositoryError, IEnumerable<TEntity>> FindMany(Option<Filter<TEntity>> filter, Option<Sort<TEntity>> sort, Option<uint> limit, Option<uint> offset);

  /// <summary>
  /// Получить количество сущностей
  /// </summary>
  /// <param name="filter">Критерии фильтрации</param>
  /// <returns>Итерируемый объект с сущностями</returns>
  public Either<RepositoryError, uint> Count(Option<Filter<TEntity>> filter);

  /// <summary>
  /// Получить одну сущность
  /// </summary>
  /// <param name="filter">Критерии фильтрации</param>
  /// <param name="sort">Критерии сортировки (если критерии не уникальны)</param>
  /// <returns>Итерируемый объект с сущностями</returns>
  public Either<RepositoryError, TEntity> FindOne(Option<Filter<TEntity>> filter, Option<Sort<TEntity>> sort);

  /// <summary>
  /// Найти сущность по идентификатору
  /// </summary>
  /// <param name="id">Идентификатор сущности</param>
  /// <returns>Сущность</returns>
  public Either<RepositoryError, TEntity> FindOneById(TEntityId id);

  /// <summary>
  /// Сохранить сущность в базу данных
  /// </summary>
  /// <param name="entity">Сущность</param>
  /// <returns>НИЧЕГО при успехе</returns>
  public Either<RepositoryError, Unit> Save(TEntity entity);

  /// <summary>
  /// Удалить сущность из базы данных
  /// </summary>
  /// <param name="filter">Фильтр для удаления</param>
  /// <returns>НИЧЕГО при успехе</returns>
  public Either<RepositoryError, Unit> Drop(Filter<TEntity> filter);

  /// <summary>
  /// Удалить сущность из базы данных
  /// </summary>
  /// <param name="id">Идентификатор сущности</param>
  /// <returns>НИЧЕГО при успехе</returns>
  public Either<RepositoryError, Unit> DropById(TEntityId id);
}
