using System.Windows;
using HotelViewer.ApplicationLayer.Services;
using HotelViewer.Domain.Entity;
using HotelViewer.Presentation.Infrastructure;
using HotelViewer.Presentation.Mappers;
using HotelViewer.Presentation.Windows;
using HotelViewer.Presentation.Windows.Editors;
using Microsoft.Extensions.DependencyInjection;

namespace HotelViewer.Presentation.ViewModels;

public class MainViewModel : ViewModelBase {
  private SessionContext Session { get; }
  private readonly IServiceProvider _serviceProvider;

  public static MainViewModel Instance { get; private set; } = null!;

  public EntityListViewModel<Hotel, HotelId> Hotels { get; }
  public EntityListViewModel<Resident, ResidentId> Residents { get; }
  public EntityListViewModel<Residence, ResidenceId> Residencies { get; }
  public EntityListViewModel<Organization, OrganizationId> Organizations { get; }
  public EntityListViewModel<Room, RoomId> Rooms { get; }
  public EntityListViewModel<User, Username> Users { get; }

  public User? CurrentUser => Session.CurrentUser.Match(u => u, () => null);

  public RelayCommand LogoutCommand => new(_ => {
    Session.Clear();

    var loginWin = _serviceProvider.GetRequiredService<LoginWindow>();
    loginWin.Show();

    Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()?.Close();
  });

  public MainViewModel(
    IServiceProvider serviceProvider,
    SessionContext session,
    EntityService<Hotel, HotelId> hotelService,
    EntityService<Resident, ResidentId> residentService,
    EntityService<Residence, ResidenceId> residenceService,
    EntityService<Organization, OrganizationId> organizationService,
    EntityService<Room, RoomId> roomService,
    EntityService<User, Username> userService,
    ExportService<Hotel, HotelId> hotelExport,
    ExportService<Resident, ResidentId> residentExport,
    ExportService<Residence, ResidenceId> residenceExport,
    ExportService<Organization, OrganizationId> organizationExport,
    ExportService<Room, RoomId> roomExport,
    ExportService<User, Username> userExport) {
    Session = session;
    _serviceProvider = serviceProvider;

    Instance = this;

    Hotels = new EntityListViewModel<Hotel, HotelId>(
      hotelService,
      hotelExport,
      session,
      CurrentUser,
      HotelUiMapper.Columns,
      e => e.Id,
      () => new HotelEditorWindow()
      );
    Residents = new EntityListViewModel<Resident, ResidentId>(
      residentService,
      residentExport,
      session,
      CurrentUser,
      ResidentUiMapper.Columns,
      e => e.Id,
      () => new ResidentEditorWindow()
      );
    Residencies = new EntityListViewModel<Residence, ResidenceId>(
      residenceService,
      residenceExport,
      session,
      CurrentUser,
      ResidenceUiMapper.Columns,
      e => e.ResidenceId,
      () => new ResidenceEditorWindow()
      );
    Organizations = new EntityListViewModel<Organization, OrganizationId>(
      organizationService,
      organizationExport,
      session,
      CurrentUser,
      OrganizationUiMapper.Columns,
      e => e.Id,
      () => new OrganizationEditorWindow()
      );
    Rooms = new EntityListViewModel<Room, RoomId>(
      roomService,
      roomExport,
      session,
      CurrentUser,
      RoomUiMapper.Columns,
      e => e.RoomId,
      () => new RoomEditorWindow()
      );

    Hotels.LoadCommand.Execute(null);
    Residents.LoadCommand.Execute(null);
    Residencies.LoadCommand.Execute(null);
    Organizations.LoadCommand.Execute(null);
    Rooms.LoadCommand.Execute(null);

    if (CurrentUser?.Role != UserRole.Admin) return;

    Users = new EntityListViewModel<User, Username>(
      userService,
      userExport,
      session,
      CurrentUser,
      UserUiMapper.Columns,
      e => e.Username,
      () => new UserEditorWindow()
    );

    Users.LoadCommand.Execute(null);
  }
}
