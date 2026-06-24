using System.Globalization;
using System.Windows.Data;
using HotelViewer.ApplicationLayer;

namespace HotelViewer.Presentation.Converters;

public class DomainObjectConverter : IValueConverter {
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
    return DomainObjectFormatter.Format(value);
  }

  public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotImplementedException();
}
