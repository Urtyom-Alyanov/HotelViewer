using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Repository;
using HotelViewer.Infrastructure.Mappers;
using LanguageExt;
using static LanguageExt.Prelude;

namespace HotelViewer.Infrastructure.Repository;

public class ResidenceRepository(DataAccess db) : IResidenceRepository {
  private const string Query = "SELECT * FROM Проживание WHERE ИдентификаторПроживания = ?";

  public Either<RepositoryError, Residence> FindById(ResidenceId id) {
    return db
      .LoadTable(Query, id.Value)
      .MapLeft(DataAccessErrorToRepositoryErrorMapper.Map)
      .Bind(
        table =>
        table.Rows.Count == 0
          ? Left<RepositoryError, Residence>(new EntityNotFound<ResidenceId>(id))
          : Right<RepositoryError, Residence>(ResidenceMapper.MapFromDb(table.Rows[0]))
        );
  }

  public Either<RepositoryError, Unit> Save(Residence entity) {
    return db
      .LoadTable(Query, entity.ResidenceId.Value)
      .Map(table => ResidenceMapper.MapIntoDb(entity, table))
      .Bind(table => db.SaveTable(Query, table))
      .MapLeft(DataAccessErrorToRepositoryErrorMapper.Map);
  }
}
