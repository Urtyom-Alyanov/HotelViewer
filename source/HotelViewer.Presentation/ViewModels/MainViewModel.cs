using HotelViewer.ApplicationLayer.Services;
using HotelViewer.Domain.Entity;

namespace HotelViewer.Presentation.ViewModels;

public class MainViewModel {
  public SessionContext Session { get; }

  public EntityListViewModel<Hotel, HotelId> Hotels { get; }
  public EntityListViewModel<Resident, ResidentId> Residents { get; }

  public MainViewModel(
    SessionContext session,
    EntityService<Hotel, HotelId> hotelService,
    EntityService<Resident, ResidentId> residentService)
  {
    Session = session;

    Hotels = new EntityListViewModel<Hotel, HotelId>(hotelService, session);
    Residents = new EntityListViewModel<Resident, ResidentId>(residentService, session);

    Hotels.LoadCommand.Execute(null);
  }
}
