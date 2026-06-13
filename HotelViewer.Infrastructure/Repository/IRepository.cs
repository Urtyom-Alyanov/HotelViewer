using LanguageExt;

namespace HotelViewer.Infrastructure.Repository;

public record RepositoryError;

public interface IRepository<Entity, EntityId>
{
    public Either<RepositoryError, Entity> GetById(EntityId id);
    public Either<RepositoryError, Entity> Save(Entity entity);
}