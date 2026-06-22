using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Helper;
using HotelViewer.Domain.Repository;
using HotelViewer.Infrastructure.Helpers;
using HotelViewer.Infrastructure.Mappers;
using LanguageExt;
using static LanguageExt.Prelude;

namespace HotelViewer.Infrastructure.Repository;

/// <summary>
/// Репозиторий с организациями
/// </summary>
/// <param name="db">Доступ к базе данных</param>
/// <param name="hotelRepository">Репозиторий с отелями для каскадных операций</param>
public class OrganizationRepository(
  DataAccess db,
  IHotelRepository hotelRepository
  ) : IOrganizationRepository {
  private static readonly QueryBuilder<Organization> QueryBuilder = new(
    "SELECT * FROM Организация",
    OrganizationMapper.MapPropertyIntoDbColumn
  );

  private readonly RepositoryUtils<Organization, OrganizationMapper> _repositoryUtils = new(
    db, QueryBuilder);

  public Either<RepositoryError, IEnumerable<Organization>> FindMany(
    Option<Filter<Organization>> filter,
    Option<Sort<Organization>> sort,
    Option<uint> limit,
    Option<uint> offset
  ) => _repositoryUtils.FindMany(filter, sort, limit, offset);

  public Either<RepositoryError, Organization> FindOne(Option<Filter<Organization>> filter, Option<Sort<Organization>> sort) =>
    _repositoryUtils.FindOne(filter, sort);

  public Either<RepositoryError, uint> Count(Option<Filter<Organization>> filter) =>
    _repositoryUtils.Count(filter);

  public Either<RepositoryError, Unit> Save(Organization entity) =>
    _repositoryUtils.Save(
      QueryBuilder.Build(
        new Filter<Organization>([
          new FilterCriterion<Organization>(e => e.Id, entity.Id)
        ]),
        new Sort<Organization>(e => e.Id),
        1u,
        0u
      ), entity);

  public Either<RepositoryError, Unit> Drop(Filter<Organization> filter) =>
    FindMany(filter, None, None, None)
      .Map(hotels => hotels.Select(h => h.Id).ToList())
      .Bind(ids => {
        if (ids.Count == 0) return Right(unit);

        return hotelRepository.Drop(new Filter<Hotel>([
          new FilterCriterion<Hotel>(r => r.OrganizationId, ids, FilterOp.In)
        ])).Bind(_ => _repositoryUtils.Drop(filter));
      });

  public Either<RepositoryError, Organization> FindOneById(OrganizationId id) =>
    FindOne(
      new Filter<Organization>([
        new FilterCriterion<Organization>(e => e.Id, id)
      ]),
      new Sort<Organization>(e => e.Id)
    ).MapLeft(err => err switch {
      EntityNotFoundByFilter<Organization> => new EntityNotFoundById<OrganizationId>(id),
      _ => err
    });

  public Either<RepositoryError, Unit> DropById(OrganizationId id) =>
    Drop(
      new Filter<Organization>([
        new FilterCriterion<Organization>(e => e.Id, id)
      ])
    );
}
