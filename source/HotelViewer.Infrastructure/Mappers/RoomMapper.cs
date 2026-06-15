using System.Data;
using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Value;

namespace HotelViewer.Infrastructure.Mappers;

public class RoomMapper : IEntityMapper<Room> {
  public static Room MapFromDb(DataRow dataRow) {
    return new Room(
        new RoomId(
            RoomNumber.FromDbValue(dataRow.Int("Номер")),
            new HotelId(dataRow.Int("ИдентификаторОтеля"))
            ),
        (RoomType)dataRow.Int("ТипНомера")
        );
  }

  public static DataTable MapIntoDb(Room entity, DataTable table) {
    return table.GetOrNewRow()
        .Set("Номер", entity.RoomId.Number.ToDbValue())
        .Set("ИдентификаторГостиницы", entity.RoomId.HotelId.Value)
        .Set("ТипНомера", (int)entity.Type)
        .Table;
  }
}
