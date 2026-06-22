using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Helper;
using HotelViewer.Domain.Repository;
using HotelViewer.Infrastructure.Helpers;
using HotelViewer.Infrastructure.Mappers;
using LanguageExt;

namespace HotelViewer.Infrastructure.Repository;

/// <summary>
/// Репозиторий с проживаниями
/// </summary>
/// <param name="db">Доступ к базе данных</param>
public class ResidenceRepository(DataAccess db) : IResidenceRepository {
  private static readonly QueryBuilder<Residence> QueryBuilder = new(
    "SELECT * FROM Проживание",
    ResidenceMapper.MapPropertyIntoDbColumn
  );

  private readonly RepositoryUtils<Residence, ResidenceMapper> _repositoryUtils = new(
    db, QueryBuilder);

  public Either<RepositoryError, IEnumerable<Residence>> FindMany(
    Option<Filter<Residence>> filter,
    Option<Sort<Residence>> sort,
    Option<uint> limit,
    Option<uint> offset
  ) => _repositoryUtils.FindMany(filter, sort, limit, offset);

  public Either<RepositoryError, Residence> FindOne(Option<Filter<Residence>> filter, Option<Sort<Residence>> sort) =>
    _repositoryUtils.FindOne(filter, sort);

  public Either<RepositoryError, uint> Count(Option<Filter<Residence>> filter) =>
    _repositoryUtils.Count(filter);

  public Either<RepositoryError, Unit> Save(Residence entity) =>
    _repositoryUtils.Save(
      QueryBuilder.Build(
        new Filter<Residence>([
          new FilterCriterion<Residence>(e => e.ResidenceId, entity.ResidenceId)
        ]),
        new Sort<Residence>(e => e.ResidenceId),
        1u,
        0u
      ), entity);

  public Either<RepositoryError, Unit> Drop(Filter<Residence> filter) =>
    _repositoryUtils.Drop(filter);

  public Either<RepositoryError, Residence> FindOneById(ResidenceId id) =>
    FindOne(
      new Filter<Residence>([
        new FilterCriterion<Residence>(e => e.ResidenceId, id)
      ]),
      new Sort<Residence>(e => e.ResidenceId)
    ).MapLeft(err => err switch {
      EntityNotFoundByFilter<Residence> => new EntityNotFoundById<ResidenceId>(id),
      _ => err
    });

  public Either<RepositoryError, Unit> DropById(ResidenceId id) =>
    Drop(
      new Filter<Residence>([
        new FilterCriterion<Residence>(e => e.ResidenceId, id)
      ])
    );
}
