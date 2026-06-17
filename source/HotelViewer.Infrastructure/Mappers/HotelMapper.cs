using System.Data;
using System.Linq.Expressions;
using HotelViewer.Domain.Entity;
using LanguageExt;
using static LanguageExt.Prelude;

namespace HotelViewer.Infrastructure.Mappers;

public class HotelMapper : IEntityMapper<Hotel> {
  private static readonly HashMap<string, string> PropToCol = HashMap(
    (nameof(Hotel.Id), "ИдентификаторГостиницы"),
    (nameof(Hotel.Name), "Название"),
    (nameof(Hotel.Number), "ТелефонДежурной"),
    (nameof(Hotel.Address), "Адрес"),
    (nameof(Hotel.OrganizationId), "ИдентификаторОрганизации")
  );

  private static readonly HashMap<string, string> ColToProp = ColToProp.Invert();

  public static Hotel MapFromDb(DataRow dataRow) =>
    new (
      new (dataRow.Int<Hotel>(ColToProp, e => e.Id)),
      dataRow.Str<Hotel>(ColToProp, e => e.Name),
      new(dataRow.Str<Hotel>(ColToProp, e => e.Number)),
      new(dataRow.Str<Hotel>(ColToProp, e => e.Address)),
      new (dataRow.Int<Hotel>(ColToProp, e => e.OrganizationId))
    );

  public static DataTable MapIntoDb(Hotel entity, DataTable table) =>
    table.GetOrNewRow()
      .Set<Hotel>(PropToCol, u => u.Id, entity.Id.Value)
      .Set<Hotel>(PropToCol, u => u.Name, entity.Name)
      .Set<Hotel>(PropToCol, u => u.Number, entity.Number.Value)
      .Set<Hotel>(PropToCol, u => u.Address, entity.Address.Value)
      .Set<Hotel>(PropToCol, u => u.OrganizationId, entity.OrganizationId.Value)
      .Table;

  public static Option<string> MapPropertyIntoDbColumn<TValue>(
    Expression<Func<Hotel, TValue>> propertySelector) =>
    PropertyExt.GetPropertyName(propertySelector).Bind(name => PropToCol.Find(name));

  public static Option<string> MapDbColumnIntoProperty(string columnName) =>
    ColToProp.Find(columnName);
}
