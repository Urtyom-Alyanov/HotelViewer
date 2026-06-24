using System.Collections.ObjectModel;
using System.Windows;
using HotelViewer.ApplicationLayer.Services;
using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Helper;
using HotelViewer.Presentation.Infrastructure;
using HotelViewer.Presentation.Mappers;
using HotelViewer.Presentation.Windows.Editors;
using LanguageExt;
using Microsoft.Win32;
using static LanguageExt.Prelude;

namespace HotelViewer.Presentation.ViewModels;

public class EntityListViewModel<TEntity, TEntityId>(
  EntityService<TEntity, TEntityId> service,
  ExportService<TEntity, TEntityId> exportService,
  SessionContext sessionContext,
  User currentUser,
  List<IColumnConfig> columnConfigs,
  Func<TEntity, TEntityId> idSelector,
  Func<IEntityEditor<TEntity>> editorFactory) : ViewModelBase {

  public SessionContext Session => sessionContext;
  public ObservableCollection<TEntity> Items { get; } = new();

  public List<IColumnConfig> ColumnConfigs => columnConfigs;

  private TEntity? _selectedItem;
  public TEntity? SelectedItem {
    get => _selectedItem;
    set { _selectedItem = value; OnPropertyChanged(); }
  }

  private Option<Sort<TEntity>> _currentSort = None;

  private string _searchText = "";
  public string SearchText { get => _searchText; set { _searchText = value; OnPropertyChanged(); } }

  private IColumnConfig? _selectedProperty;
  public IColumnConfig? SelectedProperty { get => _selectedProperty; set { _selectedProperty = value; OnPropertyChanged(); } }

  private FilterOp _selectedOp = FilterOp.Like;
  public FilterOp SelectedOp { get => _selectedOp; set { _selectedOp = value; OnPropertyChanged(); } }

  public List<IColumnConfig> AvailableProperties => ColumnConfigs;

  public void ApplySort(string propertyName, bool ascending) {
    if (string.IsNullOrEmpty(propertyName)) return;

    var param = System.Linq.Expressions.Expression.Parameter(typeof(TEntity), "e");
    System.Linq.Expressions.Expression body = param;

    foreach (var member in propertyName.Split('.'))
      body = System.Linq.Expressions.Expression.PropertyOrField(body, member);

    var conversion = System.Linq.Expressions.Expression.Convert(body, typeof(object));
    var selector = System.Linq.Expressions.Expression.Lambda<Func<TEntity, object>>(conversion, param);

    _currentSort = Some(new Sort<TEntity>(selector, ascending));

    LoadData();
  }

  private void LoadData() {
    service.FindMany(filter: BuildFilter(), sort: _currentSort).Match(
      err => MessageBox.Show(err.Message),
      list => {
        Items.Clear();
        foreach (var item in list) Items.Add(item);
      }
    );
  }

  public List<FilterOp> AvailableOps => Enum.GetValues(typeof(FilterOp)).Cast<FilterOp>().ToList();

  public User? CurrentUser => currentUser;

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

  public RelayCommand LoadCommand => new(_ => LoadData());

  private Option<Filter<TEntity>> BuildFilter() {
    if (string.IsNullOrWhiteSpace(SearchText) || SelectedProperty == null)
      return None;

    var param = System.Linq.Expressions.Expression.Parameter(typeof(TEntity), "e");

    System.Linq.Expressions.Expression body = param;

    foreach (var member in SelectedProperty.PropertyPath.Split('.'))
      body = System.Linq.Expressions.Expression.PropertyOrField(body, member);

    var conversion = System.Linq.Expressions.Expression.Convert(body, typeof(object));
    var selector = System.Linq.Expressions.Expression.Lambda<Func<TEntity, object>>(conversion, param);

    object value;
    if (SelectedOp == FilterOp.In) {
      var parts = SearchText
        .Split([',', ';'], StringSplitOptions.RemoveEmptyEntries)
        .Select(p => p.Trim())
        .ToList();

      var propType = body.Type;
      if (propType == typeof(int))
        value = parts.Select(p => int.TryParse(p, out var i) ? i : 0).ToList();
      else value = parts;

    }
    else value = SearchText;

    return Some(new Filter<TEntity>(new FilterCriterion<TEntity>(selector, value, SelectedOp)));
  }

  public RelayCommand EditCommand => new(_ => {
    if (SelectedItem == null) return;
    var editor = editorFactory();
    editor.SetEntity(SelectedItem);

    if (editor.ShowDialog() == true && editor.Entity != null) {
      service.Save(editor.Entity).Match(
        err => MessageBox.Show(err.Message),
        _ => LoadCommand.Execute(null)
      );
    }
  }, _ => SelectedItem != null && sessionContext.IsInRole(UserRole.Redactor));

  public RelayCommand CSVExport => new(_ => {
    var dialog = new SaveFileDialog {
      Filter = "CSV файл (*.csv)|*.csv",
      FileName = $"{typeof(TEntity).Name}_Export_{DateTime.Now:yyyyMMdd}"
    };
    if (dialog.ShowDialog() == true)
      exportService.ExportToCsv(dialog.FileName).Match(
        err => MessageBox.Show(err.Message, "Ошибка экспорта"),
        path => MessageBox.Show($"Данные успешно экспортированы: {path}", "Экспорт")
      );
  }, _ => sessionContext.IsInRole(UserRole.Reader));

  public RelayCommand XLSXExport => new(_ => {
    var dialog = new SaveFileDialog {
      Filter = "Excel книга (*.xlsx)|*.xlsx",
      FileName = $"{typeof(TEntity).Name}_Export_{DateTime.Now:yyyyMMdd}"
    };
    if (dialog.ShowDialog() == true)
      exportService.ExportToXLSX(dialog.FileName).Match(
        err => MessageBox.Show(err.Message, "Ошибка экспорта"),
        path => MessageBox.Show($"Данные успешно экспортированы: {path}", "Экспорт")
      );
  }, _ => sessionContext.IsInRole(UserRole.Reader));

  public RelayCommand DeleteCommand => new(id => {
    if (id is TEntityId entityId) {
      service.DropById(entityId).Match(
        err => MessageBox.Show(err.Message),
        _ => LoadCommand.Execute(null)
      );
    }
  }, _ => sessionContext.IsInRole(UserRole.Redactor));
}
