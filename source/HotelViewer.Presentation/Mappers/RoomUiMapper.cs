using HotelViewer.Domain.Entity;
using HotelViewer.Presentation.Converters;
using HotelViewer.Presentation.Mappers;

namespace HotelViewer.Presentation.Mappers;

public class RoomUiMapper : IUiMapper {
  public static List<IColumnConfig> Columns => new() {
    new ColumnInfo<Room>("Номер комнаты", e => e.RoomId.Number, e => e.RoomId.Number, new DomainObjectConverter()),
    new ColumnInfo<Room>("Отель", e => e.RoomId.HotelId, e => e.RoomId.HotelId, new LookupConverter()),
    new ColumnInfo<Room>("Тип", e => e.Type, e => e.Type, new EnumDescriptionConverter()),
  };
}
