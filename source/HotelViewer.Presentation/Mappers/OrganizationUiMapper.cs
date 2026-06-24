using HotelViewer.Domain.Entity;

namespace HotelViewer.Presentation.Mappers;

public class OrganizationUiMapper {
  public static List<IColumnConfig> Columns => new() {
    new ColumnInfo<Organization>("ID", e => e.Id, e => e.Id),
      new ColumnInfo<Organization>("Название", e => e.Name, e => e.Name),
  };
}
