using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Repository;
using HotelViewer.Infrastructure.Mappers;
using LanguageExt;
using static LanguageExt.Prelude;

namespace HotelViewer.Infrastructure.Repository;

public class ResidentRepository(DataAccess db) : IResidentRepository {
  private const string Query = "SELECT * FROM Проживающий WHERE ИдентификаторПроживающего = ?";

  /// <summary>
  /// Найти проживающего по идентификатору
  /// </summary>
  /// <param name="id">Идентификатор</param>
  /// <returns>Проживающий</returns>
  public Either<RepositoryError, Resident> FindById(ResidentId id) {
    return db
      .LoadTable(Query, id.Value)
      .MapLeft(DataAccessErrorToRepositoryErrorMapper.Map)
      .Bind(table =>
        table.Rows.Count == 0
          ? Left<RepositoryError, Resident>(new EntityNotFound<ResidentId>(id))
          : Right<RepositoryError, Resident>(ResidentMapper.MapFromDb(table.Rows[0]))
      );
  }

  /// <summary>
  /// Сохранить проживающего в базе данных
  /// </summary>
  /// <param name="entity">Проживающий</param>
  /// <returns>НИЧЕГО</returns>
  public Either<RepositoryError, Unit> Save(Resident entity) {
    return db
      .LoadTable(Query, entity.Id.Value)
      .Map(table => ResidentMapper.MapIntoDb(entity, table))
      .Bind(table => db.SaveTable(Query, table))
      .MapLeft(DataAccessErrorToRepositoryErrorMapper.Map);
  }
}
