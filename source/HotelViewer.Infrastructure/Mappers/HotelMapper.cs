using System.Data;
using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Value;

namespace HotelViewer.Infrastructure.Mappers;

public class HotelMapper : IEntityMapper<Hotel>
{
    public Hotel MapFromDb(DataRow dataRow)
    {
        return new Hotel(
            new HostelId(dataRow.Int("ИдентификаторГостиницы")),
            dataRow.Str("Название"),
            new PhoneNumber(dataRow.Str("ТелефонДежурной")),
            new Address(dataRow.Str("Адрес")),
            new OrganizationId(dataRow.Int("ИдентификаторОрганизации"))
        );
    }

    public DataRow MapIntoDb(Hotel entity, DataTable table)
    {
        return table.GetOrNewRow()
            .Set("ИдентификаторГостиницы", entity.Id.Value)
            .Set("Название", entity.Name)
            .Set("ТелефонДежурной", entity.Number.Value)
            .Set("Адрес", entity.Address.Value)
            .Set("ИдентификаторОрганизации", entity.OrganizationId.Value);
    }
}