using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using HotelViewer.Presentation.Converters;
using HotelViewer.Presentation.ViewModels;

namespace HotelViewer.Presentation.Controls;

public partial class EntityTableControl : UserControl {
  public dynamic? ViewModel => DataContext;

  public EntityTableControl() {
    InitializeComponent();
  }

  private void DataGrid_OnAutoGeneratingColumn(object? sender, DataGridAutoGeneratingColumnEventArgs e) {
    var blackList = new[] { "PasswordHash", "PasswordSalt" };
    if (blackList.Contains(e.PropertyName)) {
      e.Cancel = true;
      return;
    }

    e.Column.CanUserSort = true;

    if (e.PropertyType.Namespace != null && e.PropertyType.Namespace.Contains("Domain.Value")) {
      var subProps = e.PropertyType.GetProperties();

      if (subProps.Any(p => p.Name == "Value")) {
        e.Column.SortMemberPath = $"{e.PropertyName}.Value";
      }
      else if (subProps.Length > 0) {
        e.Column.SortMemberPath = $"{e.PropertyName}.{subProps[0].Name}";
      }
    }

    if (e.Column is DataGridTextColumn textColumn && textColumn.Binding is Binding binding) {
      if (e.PropertyType.IsEnum)
        binding.Converter = new EnumDescriptionConverter();
      else if (e.PropertyType.Namespace != null && e.PropertyType.Namespace.Contains("Domain.Value"))
        binding.Converter = new DomainObjectConverter();
      else {
        var group = new ConverterGroup { new DomainObjectConverter() };
        binding.Converter = group;
      }
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

