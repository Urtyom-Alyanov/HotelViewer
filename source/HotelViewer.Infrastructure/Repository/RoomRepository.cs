using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Repository;
using HotelViewer.Infrastructure.Mappers;
using LanguageExt;
using static LanguageExt.Prelude;

namespace HotelViewer.Infrastructure.Repository;

public class RoomRepository(DataAccess db) : IRoomRepository {
  private const string Query = "SELECT * FROM Номер WHERE Номер = ? AND ИдентификаторГостиницы = ?";

  /// <summary>
  /// Получить комнату по идентификатору
  /// </summary>
  /// <param name="id">Сложный идентификатор состоящий из номера и идентификатора гостиницы</param>
  /// <returns>Комната</returns>
  public Either<RepositoryError, Room> FindById(RoomId id) {
    var number = id.Number;
    var hostelId = id.HotelId;

    return db
      .LoadTable(Query, number.ToDbValue(), hostelId.Value)
      .MapLeft(DataAccessErrorToRepositoryErrorMapper.Map)
      .Bind(table =>
        table.Rows.Count == 0
          ? Left<RepositoryError, Room>(new EntityNotFound<RoomId>(id))
          : Right<RepositoryError, Room>(RoomMapper.MapFromDb(table.Rows[0]))
      );
  }

  /// <summary>
  /// Сохранение комнаты
  /// </summary>
  /// <param name="entity">Комната в гостинице</param>
  /// <returns>Ничего</returns>
  public Either<RepositoryError, Unit> Save(Room entity) {
    return db
      .LoadTable(Query, entity.RoomId.Number.ToDbValue(), entity.RoomId.HotelId.Value)
      .Map(table => RoomMapper.MapIntoDb(entity, table))
      .Bind(table => db.SaveTable(Query, table))
      .MapLeft(DataAccessErrorToRepositoryErrorMapper.Map);
  }
}
