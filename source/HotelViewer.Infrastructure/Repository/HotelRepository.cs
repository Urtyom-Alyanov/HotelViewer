using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Repository;
using HotelViewer.Infrastructure.Mappers;
using LanguageExt;
using static LanguageExt.Prelude;

namespace HotelViewer.Infrastructure.Repository;

public class HotelRepository(DataAccess db) : IHotelRepository {
  private const string Query = "SELECT * FROM Гостиница WHERE ИдентификаторГостиницы = ?";

  public Either<RepositoryError, Hotel> FindById(HotelId id) {
    return db
      .LoadTable(Query, id.Value)
      .MapLeft(DataAccessErrorToRepositoryErrorMapper.Map)
      .Bind(table =>
        table.Rows.Count == 0
          ? Left<RepositoryError, Hotel>(new EntityNotFound<HotelId>(id))
          : Right<RepositoryError, Hotel>(HotelMapper.MapFromDb(table.Rows[0]))
      );
  }

  public Either<RepositoryError, Unit> Save(Hotel entity) {
    return db
      .LoadTable(Query, entity.Id.Value)
      .Map(table => HotelMapper.MapIntoDb(entity, table))
      .Bind(table => db.SaveTable(Query, table))
      .MapLeft(DataAccessErrorToRepositoryErrorMapper.Map);
  }
}
