using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Value;
using LanguageExt;

namespace HotelViewer.Domain.Repository;

public abstract record RepositoryError(string Message);
public record EntityNotFound(object Id) : RepositoryError($"Сущность с ID {Id} не найдена в системе.");
public record InfrastructureFault(string Detail) : RepositoryError($"Сбой инфраструктуры данных: {Detail}");

public interface IRepository<Entity, EntityId>
{
    public Either<RepositoryError, Entity> GetById(EntityId id);
    public Either<RepositoryError, Entity> Save(Entity entity);
}
