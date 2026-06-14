using System.Data;
using HotelViewer.Domain.Entity;

namespace HotelViewer.Infrastructure.Mappers;

public class OrganizationMapper: IEntityMapper<Organization>
{
    public Organization MapFromDb(DataRow dataRow)
    {
        return new Organization(
            new OrganizationId(dataRow.Int("ИдентификаторОрганизации")),
            dataRow.Str("Название")
        );
    }

    public DataRow MapIntoDb(Organization entity, DataTable table)
    {
        return table.GetOrNewRow()
            .Set("ИдентификаторОрганизации", entity.Id.Value)
            .Set("Название", entity.Name);
    }
}