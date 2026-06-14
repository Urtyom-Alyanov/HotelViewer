using System.Data;
using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Value;

namespace HotelViewer.Infrastructure.Mappers;

public class RoomMapper : IEntityMapper<Room>
{
    public Room MapFromDb(DataRow dataRow)
    {
        return new Room(
            new RoomId(
                RoomNumber.FromDbValue(dataRow.Int("Номер")),
                new HostelId(dataRow.Int("ИдентификаторОтеля"))
                ),
            (RoomType)dataRow.Int("ТипНомера")
            );
    }

    public DataRow MapIntoDb(Room entity, DataTable table)
    {
        return table.GetOrNewRow()
            .Set("Номер", entity.RoomId.Number.ToDbValue())
            .Set("ИдентификаторГостиницы", entity.RoomId.HostelId.Value)
            .Set("ТипНомера", (int)entity.Type);
    }
}