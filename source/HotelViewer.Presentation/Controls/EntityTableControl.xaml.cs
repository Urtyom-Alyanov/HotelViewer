using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Value;
using HotelViewer.Presentation.Converters;
using HotelViewer.Presentation.Mappers;
using HotelViewer.Presentation.ViewModels;

namespace HotelViewer.Presentation.Controls;

public partial class EntityTableControl : UserControl {
  public dynamic? ViewModel => DataContext;

  public EntityTableControl() {
    InitializeComponent();
    this.DataContextChanged += OnDataContextChanged;
  }

  private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
    if (ViewModel == null) return;

    MainGrid.Columns.Clear();

    foreach (var config in (IEnumerable<IColumnConfig>)ViewModel.ColumnConfigs) {
      var column = new DataGridTextColumn {
        Header = config.Header,
        Binding = new Binding(config.PropertyPath) {
          Converter = config.Converter ?? new DomainObjectConverter()
        },
        SortMemberPath = config.SortPath
      };
      MainGrid.Columns.Add(column);
    }
  }

  private void ResetSearch_Click(object sender, RoutedEventArgs e) {
    ViewModel?.SearchText = string.Empty;
    ViewModel?.LoadCommand.Execute(null);
  }

  private void DataGrid_OnSorting(object sender, DataGridSortingEventArgs e) {
    if (e.Column == null) return;

    string propertyName = e.Column.SortMemberPath;
    if (string.IsNullOrEmpty(propertyName)) return;

    if (DataContext == null) return;

    e.Handled = true;

    bool ascending = e.Column.SortDirection != ListSortDirection.Ascending;
    e.Column.SortDirection = ascending ? ListSortDirection.Ascending : ListSortDirection.Descending;

    ViewModel?.ApplySort(propertyName, ascending);
  }
}

