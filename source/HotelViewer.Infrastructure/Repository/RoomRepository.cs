using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Helper;
using HotelViewer.Domain.Repository;
using HotelViewer.Infrastructure.Helpers;
using HotelViewer.Infrastructure.Mappers;
using LanguageExt;
using static LanguageExt.Prelude;

namespace HotelViewer.Infrastructure.Repository;

/// <summary>
/// Репозиторий с номерами
/// </summary>
/// <param name="db">Доступ к базе данных</param>
/// <param name="residenceRepository">Репозиторий с проживаниями для каскадных операций</param>
public class RoomRepository(
  DataAccess db,
  IResidenceRepository residenceRepository
  ) : IRoomRepository {
  private static readonly QueryBuilder<Room> QueryBuilder = new(
    "SELECT * FROM Номер",
    RoomMapper.MapPropertyIntoDbColumn
  );

  private readonly RepositoryUtils<Room, RoomMapper> _repositoryUtils = new(
    db, QueryBuilder);

  public Either<RepositoryError, IEnumerable<Room>> FindMany(
    Option<Filter<Room>> filter,
    Option<Sort<Room>> sort,
    Option<uint> limit,
    Option<uint> offset
  ) => _repositoryUtils.FindMany(filter, sort, limit, offset);

  public Either<RepositoryError, Room> FindOne(Option<Filter<Room>> filter, Option<Sort<Room>> sort) =>
    _repositoryUtils.FindOne(filter, sort);

  public Either<RepositoryError, uint> Count(Option<Filter<Room>> filter) =>
    _repositoryUtils.Count(filter);

  public Either<RepositoryError, Unit> Save(Room entity) =>
    _repositoryUtils.Save(
      QueryBuilder.Build(
        new Filter<Room>([
          new FilterCriterion<Room>(e => e.RoomId.HotelId, entity.RoomId.HotelId),
          new FilterCriterion<Room>(e => e.RoomId.Number, entity.RoomId.Number),
        ]),
        new Sort<Room>(e => e.RoomId.Number),
        1u,
        0u
      ), entity);

  public Either<RepositoryError, Unit> Drop(Filter<Room> filter) =>
    FindMany(filter, None, None, None)
      .Map(room => (room.Select(r => r.RoomId.HotelId).ToList(), room.Select(r => r.RoomId.Number).ToList()))
      .Bind(ids => {
        var (hotelIds, roomNumbers) = ids;
        if (hotelIds.Count == 0 || roomNumbers.Count == 0) return Right(unit);

        return residenceRepository.Drop(new Filter<Residence>([
          new FilterCriterion<Residence>(r => r.HotelId, hotelIds, FilterOp.In),
          new FilterCriterion<Residence>(r => r.Number, roomNumbers, FilterOp.In)
        ])).Bind(_ => _repositoryUtils.Drop(filter));
      });

  public Either<RepositoryError, Room> FindOneById(RoomId id) =>
    FindOne(
      new Filter<Room>([
        new FilterCriterion<Room>(e => e.RoomId.HotelId, id.HotelId),
        new FilterCriterion<Room>(e => e.RoomId.Number, id.Number),
      ]),
      new Sort<Room>(e => e.RoomId.Number)
    ).MapLeft(err => err switch {
      EntityNotFoundByFilter<Room> => new EntityNotFoundById<RoomId>(id),
      _ => err
    });

  public Either<RepositoryError, Unit> DropById(RoomId id) =>
    Drop(
      new Filter<Room>([
        new FilterCriterion<Room>(e => e.RoomId.HotelId, id.HotelId),
        new FilterCriterion<Room>(e => e.RoomId.Number, id.Number),
      ])
    );
}
