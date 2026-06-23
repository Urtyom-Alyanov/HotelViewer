using HotelViewer.ApplicationLayer.Services;
using HotelViewer.Domain.Entity;
using HotelViewer.Presentation.Windows.Editors;

namespace HotelViewer.Presentation.ViewModels;

public class MainViewModel {
  public SessionContext Session { get; }

  public EntityListViewModel<Hotel, HotelId> Hotels { get; }
  public EntityListViewModel<Resident, ResidentId> Residents { get; }
  public EntityListViewModel<Residence, ResidenceId> Residencies { get; }
  public EntityListViewModel<Organization, OrganizationId> Organizations { get; }
  public EntityListViewModel<Room, RoomId> Rooms { get; }
  public EntityListViewModel<User, Username> Users { get; }


  public MainViewModel(
    SessionContext session,
    EntityService<Hotel, HotelId> hotelService,
    EntityService<Resident, ResidentId> residentService,
    EntityService<Residence, ResidenceId> residenceService,
    EntityService<Organization, OrganizationId> organizationService,
    EntityService<Room, RoomId> roomService,
    EntityService<User, Username> userService)
  {
    Session = session;

    Hotels = new EntityListViewModel<Hotel, HotelId>(
      hotelService,
      session,
      e => e.Id,
      () => new HotelEditorWindow()
      );
    Residents = new EntityListViewModel<Resident, ResidentId>(
      residentService,
      session,
      e => e.Id,
      () => new ResidentEditorWindow()
      );
    Residencies = new EntityListViewModel<Residence, ResidenceId>(
      residenceService,
      session,
      e => e.ResidenceId,
      () => new ResidenceEditorWindow()
      );
    Organizations = new EntityListViewModel<Organization, OrganizationId>(
      organizationService,
      session,
      e => e.Id,
      () => new OrganizationEditorWindow()
      );
    Rooms = new EntityListViewModel<Room, RoomId>(
      roomService,
      session,
      e => e.RoomId,
      () => new RoomEditorWindow()
      );
    Users = new EntityListViewModel<User, Username>(
      userService,
      session,
      e => e.Username,
      () => new UserEditorWindow()
    );

    Hotels.LoadCommand.Execute(null);
    Residents.LoadCommand.Execute(null);
    Residencies.LoadCommand.Execute(null);
    Organizations.LoadCommand.Execute(null);
    Rooms.LoadCommand.Execute(null);
    Users.LoadCommand.Execute(null);
  }
}
