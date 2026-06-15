using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Repository;
using HotelViewer.Infrastructure.Mappers;
using LanguageExt;
using static LanguageExt.Prelude;

namespace HotelViewer.Infrastructure.Repository;

public class OrganizationRepository(DataAccess db) : IOrganizationRepository {
  private const string Query = "SELECT * FROM Организация WHERE ИдентификаторОрганизации = ?";


  public Either<RepositoryError, Organization> FindById(OrganizationId id) {
    return db
      .LoadTable(Query, id.Value)
      .MapLeft(DataAccessErrorToRepositoryErrorMapper.Map)
      .Bind(table =>
        table.Rows.Count == 0
          ? Left<RepositoryError, Organization>(new EntityNotFound<OrganizationId>(id))
          : Right<RepositoryError, Organization>(OrganizationMapper.MapFromDb(table.Rows[0]))
          );
  }

  public Either<RepositoryError, Unit> Save(Organization entity) {
    return db
      .LoadTable(Query, entity.Id.Value)
      .Map(table => OrganizationMapper.MapIntoDb(entity, table))
      .Bind(table => db.SaveTable(Query, table))
      .MapLeft(DataAccessErrorToRepositoryErrorMapper.Map);
  }
}
