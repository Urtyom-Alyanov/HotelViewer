using HotelViewer.ApplicationLayer.Errors;
using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Helper;
using HotelViewer.Domain.Repository;
using LanguageExt;

namespace HotelViewer.ApplicationLayer.Services;

/// <summary>
/// Общий сервис для сущностей
/// </summary>
/// <param name="repository">Репозиторий</param>
/// <param name="sessionContext">Сессия</param>
/// <typeparam name="TEntity">Тип сущности</typeparam>
/// <typeparam name="TEntityId">Тип идентификатора сущности</typeparam>
public class EntityService
  <TEntity, TEntityId>
  (IRepository<TEntity, TEntityId> repository, SessionContext sessionContext)
  : BaseService(sessionContext) {
  /// <summary>
  /// Получить список сущностей с фильтрацией и сортировкой (доступно всем)
  /// </summary>
  public Either<ApplicationError, IEnumerable<TEntity>> FindMany(
    Option<Filter<TEntity>> filter = default,
    Option<Sort<TEntity>> sort = default,
    Option<uint> limit = default,
    Option<uint> offset = default) =>
    EnsureRole(UserRole.Reader, $"Чтение списка {typeof(TEntity).Name}")
      .Bind(_ => repository.FindMany(filter, sort, limit, offset)
        .MapLeft(MapError));

  /// <summary>
  /// Найти одну сущность по ID (доступно всем)
  /// </summary>
  public Either<ApplicationError, TEntity> FindOneById(TEntityId id) =>
    EnsureRole(UserRole.Reader, $"Чтение {typeof(TEntity).Name}")
      .Bind(_ => repository.FindOneById(id)
        .MapLeft(MapError));

  /// <summary>
  /// Сохранить (создать или обновить) сущность (требуется роль Редактор)
  /// </summary>
  public Either<ApplicationError, Unit> Save(TEntity entity) =>
    EnsureRole(UserRole.Redactor, $"Сохранение {typeof(TEntity).Name}")
      .Bind(_ => repository.Save(entity)
        .MapLeft(MapError));

  /// <summary>
  /// Удалить сущность по ID (требуется роль Редактор)
  /// </summary>
  public Either<ApplicationError, Unit> DropById(TEntityId id) =>
    EnsureRole(UserRole.Redactor, $"Удаление {typeof(TEntity).Name}")
      .Bind(_ => repository.DropById(id)
        .MapLeft(MapError));

  /// <summary>
  /// Получить количество записей (доступно всем)
  /// </summary>
  public Either<ApplicationError, uint> Count(Option<Filter<TEntity>> filter = default) =>
    EnsureRole(UserRole.Reader, $"Подсчет {typeof(TEntity).Name}")
      .Bind(_ => repository.Count(filter)
        .MapLeft(MapError));

  /// <summary>
  /// Вспомогательный метод для маппинга ошибок репозитория в ошибки приложения
  /// </summary>
  private static ApplicationError MapError(RepositoryError error) =>
    new RepositoryFailure(error);
}
