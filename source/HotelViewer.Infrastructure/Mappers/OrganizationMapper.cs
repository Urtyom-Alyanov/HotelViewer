using System.Data;
using System.Linq.Expressions;
using HotelViewer.Domain.Entity;
using LanguageExt;
using static LanguageExt.Prelude;

namespace HotelViewer.Infrastructure.Mappers;

public class OrganizationMapper : IEntityMapper<Organization> {
  private static readonly HashMap<string, string> PropToCol = HashMap(
    (nameof(Organization.Id), "ИдентификаторОрганизации"),
    (nameof(Organization.Name), "Название")
  );

  private static readonly HashMap<string, string> ColToProp = PropToCol.Invert();

  public static Organization MapFromDb(DataRow dataRow) =>
    new(
      new(dataRow.Int<Organization>(PropToCol, e => e.Id)),
      dataRow.Str<Organization>(PropToCol, e => e.Name)
    );

  public static DataTable MapIntoDb(Organization entity, DataTable table) =>
    table.GetOrNewRow()
      .Set<Organization>(PropToCol, u => u.Id, entity.Id.Value)
      .Set<Organization>(PropToCol, u => u.Name, entity.Name)
      .Table;

  public static Option<string> MapPropertyIntoDbColumn<TValue>(
    Expression<Func<Organization, TValue>> propertySelector) =>
    PropertyExt.GetPropertyName(propertySelector).Bind(name => PropToCol.Find(name));

  public static Option<string> MapDbColumnIntoProperty(string columnName) =>
    ColToProp.Find(columnName);
}
