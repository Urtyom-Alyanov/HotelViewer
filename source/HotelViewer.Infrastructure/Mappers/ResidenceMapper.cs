using System.Data;
using System.Linq.Expressions;
using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Value;
using LanguageExt;
using static LanguageExt.Prelude;

namespace HotelViewer.Infrastructure.Mappers;

public class ResidenceMapper : IEntityMapper<Residence> {
  private static readonly HashMap<string, string> PropToCol = HashMap(
    (nameof(Residence.ResidenceId), "ИдентификаторПроживания"),
    (nameof(Residence.Number), "Номер"),
    (nameof(Residence.HotelId), "ИдентификаторГостиницы"),
    (nameof(Residence.ResidentId), "ИдентификаторПроживающего"),
    (nameof(Residence.DaysPerNight), "НаСколько"),
    (nameof(Residence.ResidenceAt), "ДатаПрибытия")
  );

  private static readonly HashMap<string, string> ColToProp = PropToCol.Invert();

  public static Residence MapFromDb(DataRow dataRow) =>
    new(
      new(dataRow.Int<Residence>(PropToCol, u => u.ResidenceId)),
      RoomNumber.FromDbValue(dataRow.Int<Residence>(PropToCol, u => u.Number)),
      new(dataRow.Int<Residence>(PropToCol, u => u.HotelId)),
      new(dataRow.Int<Residence>(PropToCol, u => u.ResidentId)),
      dataRow.UInt<Residence>(PropToCol, u => u.DaysPerNight),
      dataRow.DateTime<Residence>(PropToCol, u => u.ResidenceAt)
    );

  public static DataTable MapIntoDb(Residence entity, DataTable table) =>
    table.GetOrNewRow()
      .Set<Residence>(PropToCol, u => u.ResidenceId, entity.ResidenceId.Value)
      .Set<Residence>(PropToCol, u => u.Number, entity.Number.ToDbValue())
      .Set<Residence>(PropToCol, u => u.ResidenceAt, entity.ResidenceAt)
      .Set<Residence>(PropToCol, u => u.DaysPerNight, entity.DaysPerNight)
      .Set<Residence>(PropToCol, u => u.HotelId, entity.HotelId.Value)
      .Set<Residence>(PropToCol, u => u.ResidentId, entity.ResidentId.Value)
      .Table;

  public static Option<string> MapPropertyIntoDbColumn<TValue>(
    Expression<Func<Residence, TValue>> propertySelector) =>
    PropertyExt.GetPropertyName(propertySelector).Bind(name => PropToCol.Find(name));

  public static Option<string> MapDbColumnIntoProperty(string columnName) =>
    ColToProp.Find(columnName);
}
