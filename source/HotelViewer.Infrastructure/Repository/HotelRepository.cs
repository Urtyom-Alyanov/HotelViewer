using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Helper;
using HotelViewer.Domain.Repository;
using HotelViewer.Infrastructure.Helpers;
using HotelViewer.Infrastructure.Mappers;
using LanguageExt;
using static LanguageExt.Prelude;

namespace HotelViewer.Infrastructure.Repository;

/// <summary>
/// Репозиторий с гостиницами
/// </summary>
/// <param name="db">Доступ к базе данных</param>
/// <param name="roomRepository">Репозиторий с номерами для каскадных операций</param>
public class HotelRepository(
  DataAccess db,
  IRoomRepository roomRepository
  ) : IHotelRepository {
  private static readonly QueryBuilder<Hotel> QueryBuilder = new(
    "SELECT * FROM Гостиница",
    HotelMapper.MapPropertyIntoDbColumn
  );

  private readonly RepositoryUtils<Hotel, HotelMapper> _repositoryUtils = new(
    db, QueryBuilder);

  public Either<RepositoryError, IEnumerable<Hotel>> FindMany(
    Option<Filter<Hotel>> filter,
    Option<Sort<Hotel>> sort,
    Option<uint> limit,
    Option<uint> offset
  ) => _repositoryUtils.FindMany(filter, sort, limit, offset);

  public Either<RepositoryError, Hotel> FindOne(Option<Filter<Hotel>> filter, Option<Sort<Hotel>> sort) =>
    _repositoryUtils.FindOne(filter, sort);

  public Either<RepositoryError, uint> Count(Option<Filter<Hotel>> filter) =>
    _repositoryUtils.Count(filter);

  public Either<RepositoryError, Unit> Save(Hotel entity) =>
    _repositoryUtils.Save(
      QueryBuilder.Build(
        new Filter<Hotel>([
          new FilterCriterion<Hotel>(e => e.Id, entity.Id)
        ]),
        new Sort<Hotel>(e => e.Id),
        1u,
        0u
      ), entity);

  public Either<RepositoryError, Unit> Drop(Filter<Hotel> filter) =>
    FindMany(filter, None, None, None)
      .Map(hotels => hotels.Select(h => h.Id).ToList())
      .Bind(ids => {
        if (ids.Count == 0) return Right(unit);

        return roomRepository.Drop(new Filter<Room>([
          new FilterCriterion<Room>(r => r.RoomId.HotelId, ids, FilterOp.In)
        ])).Bind(_ => _repositoryUtils.Drop(filter));
      });

  public Either<RepositoryError, Hotel> FindOneById(HotelId id) =>
    FindOne(
      new Filter<Hotel>([
        new FilterCriterion<Hotel>(e => e.Id, id)
      ]),
      new Sort<Hotel>(e => e.Id)
    ).MapLeft(err => err switch {
      EntityNotFoundByFilter<Hotel> => new EntityNotFoundById<HotelId>(id),
      _ => err
    });

  public Either<RepositoryError, Unit> DropById(HotelId id) =>
    Drop(
      new Filter<Hotel>([
        new FilterCriterion<Hotel>(e => e.Id, id)
      ])
    );
}
