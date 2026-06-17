using HotelViewer.Domain.Helper;
using LanguageExt;

namespace HotelViewer.Domain.Repository;

public abstract record RepositoryError(string Message);
public record EntityNotFound<TEntityId>(TEntityId Id) : RepositoryError($"Сущность с ID {Id} не найдена в системе.");
public record InfrastructureFault(string Detail) : RepositoryError($"Сбой инфраструктуры данных: {Detail}");

/// <summary>
/// Интерфейс репозитория сущности
/// </summary>
/// <typeparam name="TEntity">Сущность</typeparam>
/// <typeparam name="TEntityId">Идентификатор сущности</typeparam>
/// <typeparam name="TField">Критерии фильтрации и сортировки сущности</typeparam>
public interface IRepository<TEntity, in TEntityId, TField> where TField : Enum {
  /// <summary>
  /// Получить сущностей
  /// </summary>
  /// <param name="sort">Критерии сортировки</param>
  /// <returns>Итерируемый объект с сущностями</returns>
  public Either<RepositoryError, IEnumerable<TEntity>> FindMany(Sort<TField> sort);

  /// <summary>
  /// Получить отфильтрованный список сущностей
  /// </summary>
  /// <param name="filter">Критерии фильтрации</param>
  /// <param name="sort">Критерии сортировки</param>
  /// <returns>Итерируемый объект с сущностями</returns>
  public Either<RepositoryError, IEnumerable<TEntity>> FindFiltered(Filter<TField> filter, Sort<TField> sort);

  /// <summary>
  /// Найти сущность по идентификатору
  /// </summary>
  /// <param name="id">Идентификатор сущности</param>
  /// <returns>Сущность</returns>
  public Either<RepositoryError, TEntity> FindById(TEntityId id);

  /// <summary>
  /// Сохранить сущность в базу данных
  /// </summary>
  /// <param name="entity">Сущность</param>
  /// <returns>НИЧЕГО при успехе</returns>
  public Either<RepositoryError, Unit> Save(TEntity entity);

  /// <summary>
  /// Удалить сущность из базы данных
  /// </summary>
  /// <param name="id">Идентификатор сущности</param>
  /// <returns>НИЧЕГО при успехе</returns>
  public Either<RepositoryError, Unit> Drop(TEntityId id);
}
