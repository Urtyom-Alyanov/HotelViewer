using System.Collections.ObjectModel;
using System.Windows;
using HotelViewer.ApplicationLayer.Services;
using HotelViewer.Domain.Entity;
using HotelViewer.Presentation.Infrastructure;

namespace HotelViewer.Presentation.ViewModels;

public class EntityListViewModel<TEntity, TEntityId>(
  EntityService<TEntity, TEntityId> service,
  SessionContext sessionContext) : ViewModelBase
{
  public SessionContext Session => sessionContext;
  public ObservableCollection<TEntity> Items { get; } = new();

  private TEntity? _selectedItem;
  public TEntity? SelectedItem {
    get => _selectedItem;
    set { _selectedItem = value; OnPropertyChanged(); }
  }

  public RelayCommand LoadCommand => new(_ => {
    service.FindMany().Match(
      err => MessageBox.Show(err.Message),
      list => {
        Items.Clear();
        foreach (var item in list) Items.Add(item);
      }
    );
  });

  public RelayCommand DeleteCommand => new(id => {
    if (id is TEntityId entityId) {
      service.DropById(entityId).Match(
        err => MessageBox.Show(err.Message),
        _ => LoadCommand.Execute(null)
      );
    }
  }, _ => sessionContext.IsInRole(UserRole.Admin)); // Кнопка активна только для Админа
}
