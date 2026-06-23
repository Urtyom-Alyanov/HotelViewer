using System.Globalization;
using System.Windows.Data;
using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Value;

namespace HotelViewer.Presentation.Converters;

public class EnumDescriptionConverter : IValueConverter {
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value switch {
      UserRole.Admin => "Администратор",
      UserRole.Redactor => "Редактор (может изменять)",
      UserRole.Reader => "Наблюдатель (только чтение)",

      RoomType.Standard => "Стандарт",
      RoomType.Suite => "Люкс",
      RoomType.Presidential => "Президентский",

      Sex.Male => "Мужской",
      Sex.Female => "Женский",
      _ => value?.ToString() ?? ""
    };

  public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotImplementedException();
}
