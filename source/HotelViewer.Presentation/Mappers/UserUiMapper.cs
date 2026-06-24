using HotelViewer.Domain.Entity;
using HotelViewer.Presentation.Converters;
using HotelViewer.Presentation.Mappers;

namespace HotelViewer.Presentation.Mappers;

public class UserUiMapper : IUiMapper {
  public static List<IColumnConfig> Columns => new() {
    new ColumnInfo<User>("Имя пользователя", e => e.Username, e => e.Username),
    new ColumnInfo<User>("Роль", e => e.Role, e => e.Role, new EnumDescriptionConverter()),
  };
}
