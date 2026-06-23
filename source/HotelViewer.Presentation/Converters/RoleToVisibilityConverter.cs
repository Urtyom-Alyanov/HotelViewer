using System.Windows;
using System.Windows.Data;
using HotelViewer.Domain.Entity;

namespace HotelViewer.Presentation.Converters;

public class RoleToVisibilityConverter : IValueConverter {
  public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
    if (value is UserRole currentRole && parameter is string minRoleStr) {
      var minRole = Enum.Parse<UserRole>(minRoleStr);
      return currentRole >= minRole ? Visibility.Visible : Visibility.Collapsed;
    }
    return Visibility.Collapsed;
  }
  public object ConvertBack(object v, Type t, object p, System.Globalization.CultureInfo c) => throw new NotImplementedException();
}
