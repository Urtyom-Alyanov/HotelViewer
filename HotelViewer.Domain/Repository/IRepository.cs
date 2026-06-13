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

public interface IRoomRepository : IRepository<Room, RoomId>;
public interface IHostelRepository : IRepository<Hostel, HostelId>;

public interface IResidenceRepository : IRepository<Residence, ResidenceId>
{
    public Either<RepositoryError, List<Residence>> GetByRoomAndResident(RoomNumber number, HostelId hostelId, ResidentId residentId);
}

public interface IResidentRepository : IRepository<Resident, ResidentId>;
public interface IOrganizationRepository : IRepository<Organization, OrganizationId>;
