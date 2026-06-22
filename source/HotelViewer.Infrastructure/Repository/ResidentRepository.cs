using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Helper;
using HotelViewer.Domain.Repository;
using HotelViewer.Infrastructure.Helpers;
using HotelViewer.Infrastructure.Mappers;
using LanguageExt;
using static LanguageExt.Prelude;

namespace HotelViewer.Infrastructure.Repository;

/// <summary>
/// Репозиторий с проживающими (OleDB)
/// </summary>
/// <param name="db">Доступ к базе данных</param>
/// <param name="residenceRepository">Репозиторий с проживаниями для каскадных операций</param>
public class ResidentRepository(
  DataAccess db,
  IResidenceRepository residenceRepository
  ) : IResidentRepository {
  private static readonly QueryBuilder<Resident> QueryBuilder = new(
    "SELECT * FROM Проживающий",
    ResidentMapper.MapPropertyIntoDbColumn
  );

  private readonly RepositoryUtils<Resident, ResidentMapper> _repositoryUtils = new(
    db, QueryBuilder);

  public Either<RepositoryError, IEnumerable<Resident>> FindMany(
    Option<Filter<Resident>> filter,
    Option<Sort<Resident>> sort,
    Option<uint> limit,
    Option<uint> offset
  ) => _repositoryUtils.FindMany(filter, sort, limit, offset);

  public Either<RepositoryError, Resident> FindOne(Option<Filter<Resident>> filter, Option<Sort<Resident>> sort) =>
    _repositoryUtils.FindOne(filter, sort);

  public Either<RepositoryError, uint> Count(Option<Filter<Resident>> filter) =>
    _repositoryUtils.Count(filter);

  public Either<RepositoryError, Unit> Save(Resident entity) =>
    _repositoryUtils.Save(
      QueryBuilder.Build(
        new Filter<Resident>([
          new FilterCriterion<Resident>(e => e.Id, entity.Id)
        ]),
        new Sort<Resident>(e => e.Id),
        1u,
        0u
      ), entity);

  public Either<RepositoryError, Unit> Drop(Filter<Resident> filter) =>
    FindMany(filter, None, None, None)
      .Map(residents => residents.Select(h => h.Id).ToList())
      .Bind(ids => {
        if (ids.Count == 0) return Right(unit);

        return residenceRepository.Drop(new Filter<Residence>([
          new FilterCriterion<Residence>(r => r.ResidentId, ids, FilterOp.In)
        ])).Bind(_ => _repositoryUtils.Drop(filter));
      });

  public Either<RepositoryError, Resident> FindOneById(ResidentId id) =>
    FindOne(
      new Filter<Resident>([
        new FilterCriterion<Resident>(e => e.Id, id)
      ]),
      new Sort<Resident>(e => e.Id)
    ).MapLeft(err => err switch {
      EntityNotFoundByFilter<Residence> => new EntityNotFoundById<ResidentId>(id),
      _ => err
    });

  public Either<RepositoryError, Unit> DropById(ResidentId id) =>
    Drop(
      new Filter<Resident>([
        new FilterCriterion<Resident>(e => e.Id, id)
      ])
    );
}
