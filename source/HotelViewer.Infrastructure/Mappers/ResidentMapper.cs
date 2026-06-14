using System.Data;
using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Value;

namespace HotelViewer.Infrastructure.Mappers;

public class ResidentMapper : IEntityMapper<Resident>
{
    public Resident MapFromDb(DataRow dataRow)
    {
        return new Resident(
            new ResidentId(dataRow.Int("ИдентификаторПроживающего")),
            FullName.FromDbValue(dataRow.Str("ФИО")),
            new Address(dataRow.Str("Адрес")),
            (Sex)dataRow.Int("Пол"),
            new PhoneNumber(dataRow.Str("НомерТелефона")));
    }

    public DataRow MapIntoDb(Resident entity, DataTable table)
    {
        return table.GetOrNewRow()
            .Set("ИдентификаторПроживающего", entity.Id.Value)
            .Set("ФИО", entity.Name.ToDbValue())
            .Set("Пол", (int)entity.Sex)
            .Set("НомерТелефона", entity.PhoneNumber.Value)
            .Set("Адрес", entity.Address.Value);
    }
}