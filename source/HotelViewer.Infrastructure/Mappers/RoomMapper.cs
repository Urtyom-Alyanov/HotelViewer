using System.Data;
using System.Linq.Expressions;
using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Value;
using LanguageExt;
using static LanguageExt.Prelude;

namespace HotelViewer.Infrastructure.Mappers;

public class RoomMapper : IEntityMapper<Room> {
  private static readonly HashMap<string, string> PropToCol = HashMap(
    (nameof(Room.RoomId.Number), "Номер"),
    (nameof(Room.RoomId.HotelId), "ИдентификаторГостиницы"),
    (nameof(Room.Type), "ТипНомера")
  );

  private static readonly HashMap<string, string> ColToProp = ColToProp.Invert();

  public static Room MapFromDb(DataRow dataRow) {
    var id = new RoomId(
      RoomNumber.FromDbValue(dataRow.Int<Room>(ColToProp, e => e.RoomId.Number)),
      new HotelId(dataRow.Int<Room>(ColToProp, e => e.RoomId.HotelId))
    );

    return new Room(
      id,
      (RoomType)dataRow.Int<Room>(ColToProp, u => u.Type)
    );
  }

  public static DataTable MapIntoDb(Room entity, DataTable table) =>
    table.GetOrNewRow()
      .Set<Room>(PropToCol, u => u.RoomId.Number, entity.RoomId.Number.ToDbValue())
      .Set<Room>(PropToCol, u => u.RoomId.HotelId, entity.RoomId.HotelId.Value)
      .Set<Room>(PropToCol, u => u.Type, (int)entity.Type)
      .Table;

  public static Option<string> MapPropertyIntoDbColumn<TValue>(
    Expression<Func<Room, TValue>> propertySelector) =>
    PropertyExt.GetPropertyName(propertySelector).Bind(name => PropToCol.Find(name));

  public static Option<string> MapDbColumnIntoProperty(string columnName) =>
    ColToProp.Find(columnName);
}
