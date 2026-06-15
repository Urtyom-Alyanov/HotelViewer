using LanguageExt;

namespace HotelViewer.Domain.Repository;

public abstract record RepositoryError(string Message);
public record EntityNotFound<TEntityId>(TEntityId Id) : RepositoryError($"Сущность с ID {Id} не найдена в системе.");
public record InfrastructureFault(string Detail) : RepositoryError($"Сбой инфраструктуры данных: {Detail}");

public interface IRepository<TEntity, in TEntityId> {
  public Either<RepositoryError, TEntity> FindById(TEntityId id);
  public Either<RepositoryError, Unit> Save(TEntity entity);
}
