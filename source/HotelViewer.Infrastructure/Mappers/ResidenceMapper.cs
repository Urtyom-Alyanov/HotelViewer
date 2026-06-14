using System.Data;
using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Value;

namespace HotelViewer.Infrastructure.Mappers;

public class ResidenceMapper : IEntityMapper<Residence>
{
    public Residence MapFromDb(DataRow dataRow)
    {
        return new Residence(
            new ResidenceId(dataRow.Int("ИдентификаторПроживания")),
            RoomNumber.FromDbValue(dataRow.Int("Номер")),
            new HostelId(dataRow.Int("ИдентификаторГостиницы")),
            new ResidentId(dataRow.Int("ИдентификаторПроживающего")),
            dataRow.UInt("НаСколько"),
            dataRow.DateTime("ДатаПрибытия")
        );
    }

    public DataRow MapIntoDb(Residence entity, DataTable table)
    {
        return table.GetOrNewRow()
            .Set("ИдентификаторПроживания", entity.ResidenceId.Value)
            .Set("Номер", entity.Number.ToDbValue())
            .Set("ИдентификаторГостиницы", entity.HostelId.Value)
            .Set("ИдентификаторПроживающего", entity.ResidentId.Value)
            .Set("НаСколько", entity.DaysPerNight)
            .Set("ДатаПрибытия", entity.ResidenceAt);
    }
}