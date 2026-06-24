using System.Globalization;
using System.Windows.Data;

namespace HotelViewer.Presentation.Converters;

public class DateConverter : IValueConverter {
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value switch {
    DateTime dateTime => dateTime.ToString("dd/MM/yyyy"),
    _ => value
  };

  public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotImplementedException();
}
