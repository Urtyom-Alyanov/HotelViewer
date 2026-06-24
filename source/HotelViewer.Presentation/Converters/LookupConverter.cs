using System.Globalization;
using System.Windows.Data;
using HotelViewer.Domain.Entity;
using HotelViewer.Presentation.ViewModels;

namespace HotelViewer.Presentation.Converters;

public class LookupConverter : IValueConverter {
  public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value switch {
      null => "—",

      OrganizationId id => MainViewModel.Instance.Organizations.Items
          .FirstOrDefault(x => x.Id.Value == id.Value)?.Name ?? $"ID: {id.Value}",

      HotelId id => MainViewModel.Instance.Hotels.Items
          .FirstOrDefault(x => x.Id.Value == id.Value)?.Name ?? $"ID: {id.Value}",

      ResidentId id => MainViewModel.Instance.Residents.Items
          .FirstOrDefault(x => x.Id.Value == id.Value)?.Name.ToDbValue() ?? $"ID: {id.Value}",

      _ => value
    };

  public object ConvertBack(object? value, Type t, object? p, CultureInfo c) => throw new NotImplementedException();
}
