using System.Collections.ObjectModel;
using System.Windows;
using HotelViewer.ApplicationLayer.Services;
using HotelViewer.Domain.Entity;
using HotelViewer.Presentation.Infrastructure;
using HotelViewer.Presentation.Windows.Editors;

namespace HotelViewer.Presentation.ViewModels;

public class EntityListViewModel<TEntity, TEntityId>(
  EntityService<TEntity, TEntityId> service,
  SessionContext sessionContext,
  Func<TEntity, TEntityId> idSelector,
  Func<IEntityEditor<TEntity>> editorFactory) : ViewModelBase
{
  public SessionContext Session => sessionContext;
  public ObservableCollection<TEntity> Items { get; } = new();

  private TEntity? _selectedItem;
  public TEntity? SelectedItem {
    get => _selectedItem;
    set { _selectedItem = value; OnPropertyChanged(); }
  }

  public RelayCommand DeleteSelectedCommand => new(_ => {
    if (SelectedItem != null) {
      var id = idSelector(SelectedItem);
      service.DropById(id).Match(
        err => MessageBox.Show(err.Message),
        _ => LoadCommand.Execute(null)
      );
    }
  }, _ => SelectedItem != null && sessionContext.IsInRole(UserRole.Redactor));

  public RelayCommand AddCommand => new(_ => {
    var editor = editorFactory();
    if (editor.ShowDialog() == true && editor.Entity != null) {
      service.Save(editor.Entity).Match(
        err => MessageBox.Show(err.Message),
        _ => LoadCommand.Execute(null)
      );
    }
  }, _ => sessionContext.IsInRole(UserRole.Redactor));

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
  }, _ => sessionContext.IsInRole(UserRole.Redactor));
}
