using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Value;
using LanguageExt;

namespace HotelViewer.Domain.Repository;

public interface IResidenceRepository : IRepository<Residence, ResidenceId>
{
    public Either<RepositoryError, List<Residence>> GetByRoomAndResident(RoomNumber number, HostelId hostelId, ResidentId residentId);
}