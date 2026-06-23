using System.Windows.Controls;
using System.Windows.Data;
using HotelViewer.Presentation.Converters;

namespace HotelViewer.Presentation.Controls;

public partial class EntityTableControl : UserControl {
  public EntityTableControl() {
    InitializeComponent();
  }

  private void DataGrid_OnAutoGeneratingColumn(object? sender, DataGridAutoGeneratingColumnEventArgs e) {
    var blackList = new[] { "PasswordHash", "PasswordSalt" };
    if (blackList.Contains(e.PropertyName)) {
      e.Cancel = true;
      return;
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
}

