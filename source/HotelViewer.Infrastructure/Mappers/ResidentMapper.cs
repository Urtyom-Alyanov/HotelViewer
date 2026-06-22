using System.Data;
using System.Linq.Expressions;
using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Value;
using LanguageExt;
using static LanguageExt.Prelude;

namespace HotelViewer.Infrastructure.Mappers;

public class ResidentMapper : IEntityMapper<Resident> {
  private static readonly HashMap<string, string> PropToCol = HashMap(
    (nameof(Resident.Id), "ИдентификаторПроживающего"),
    (nameof(Resident.Name), "ФИО"),
    (nameof(Resident.Address), "Адрес"),
    (nameof(Resident.Sex), "Пол"),
    (nameof(Resident.PhoneNumber), "НомерТелефона")
  );

  private static readonly HashMap<string, string> ColToProp = PropToCol.Invert();

  public static Resident MapFromDb(DataRow dataRow) =>
    new(
      new ResidentId(dataRow.Int<Resident>(ColToProp, u => u.Id)),
      FullName.FromDbValue(dataRow.Str<Resident>(ColToProp, u => u.Name)),
      new Address(dataRow.Str<Resident>(ColToProp, u => u.Address)),
      (Sex)dataRow.Int<Resident>(ColToProp, u => u.Sex),
      new PhoneNumber(dataRow.Str<Resident>(ColToProp, u => u.PhoneNumber))
    );

  public static DataTable MapIntoDb(Resident entity, DataTable table) =>
    table.GetOrNewRow()
      .Set<Resident>(PropToCol, u => u.Id, entity.Id.Value)
      .Set<Resident>(PropToCol, u => u.Name, entity.Name.ToDbValue())
      .Set<Resident>(PropToCol, u => u.Address, entity.Address.Value)
      .Set<Resident>(PropToCol, u => u.Sex, (int)entity.Sex)
      .Set<Resident>(PropToCol, u => u.PhoneNumber, entity.PhoneNumber.Value)
      .Table;

  public static Option<string> MapPropertyIntoDbColumn<TValue>(
    Expression<Func<Resident, TValue>> propertySelector) =>
    PropertyExt.GetPropertyName(propertySelector).Bind(name => PropToCol.Find(name));

  public static Option<string> MapDbColumnIntoProperty(string columnName) =>
    ColToProp.Find(columnName);
}
