using HotelViewer.Domain.Entity;
using HotelViewer.Presentation.Converters;

namespace HotelViewer.Presentation.Mappers;

public class ResidentUiMapper {
  public static List<IColumnConfig> Columns => new() {
    new ColumnInfo<Resident>("ID", e => e.Id, e => e.Id),
    new ColumnInfo<Resident>("Имя", e => e.Name, e => e.Name, new DomainObjectConverter()),
    new ColumnInfo<Resident>("Номер телефона", e => e.PhoneNumber, e => e.PhoneNumber, new DomainObjectConverter()),
    new ColumnInfo<Resident>("Адрес проживания", e => e.Address, e => e.Address, new DomainObjectConverter()),
    new ColumnInfo<Resident>("Пол", e => e.Sex, e => e.Sex, new EnumDescriptionConverter()),
  };
}
