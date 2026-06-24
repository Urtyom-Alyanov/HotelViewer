using HotelViewer.Domain.Entity;
using HotelViewer.Presentation.Converters;

namespace HotelViewer.Presentation.Mappers;

public class HotelUiMapper : IUiMapper {
  public static List<IColumnConfig> Columns => new() {
    new ColumnInfo<Hotel>("ID", e => e.Id, e => e.Id),
    new ColumnInfo<Hotel>("Название", e => e.Name),
    new ColumnInfo<Hotel>("Телефон дежурной", e => e.Number, e => e.Number),
    new ColumnInfo<Hotel>("Организация", e => e.OrganizationId, e => e.OrganizationId, new LookupConverter())
  };
}
