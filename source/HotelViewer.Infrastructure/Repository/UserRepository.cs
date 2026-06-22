using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Helper;
using HotelViewer.Domain.Repository;
using HotelViewer.Infrastructure.Helpers;
using HotelViewer.Infrastructure.Mappers;
using LanguageExt;
using static LanguageExt.Prelude;

namespace HotelViewer.Infrastructure.Repository;

/// <summary>
/// Репозиторий с пользователями
/// </summary>
/// <param name="db">Доступ к базе данных</param>
public class UserRepository(DataAccess db) : IUserRepository {
  private static readonly QueryBuilder<User> QueryBuilder = new(
    "SELECT * FROM Пользователь",
    UserMapper.MapPropertyIntoDbColumn
  );

  private readonly RepositoryUtils<User, UserMapper> _repositoryUtils = new(
    db, QueryBuilder);

  public Either<RepositoryError, IEnumerable<User>> FindMany(
    Option<Filter<User>> filter,
    Option<Sort<User>> sort,
    Option<uint> limit,
    Option<uint> offset
  ) => _repositoryUtils.FindMany(filter, sort, limit, offset);

  public Either<RepositoryError, User> FindOne(Option<Filter<User>> filter, Option<Sort<User>> sort) =>
    _repositoryUtils.FindOne(filter, sort);

  public Either<RepositoryError, uint> Count(Option<Filter<User>> filter) =>
    _repositoryUtils.Count(filter);

  public Either<RepositoryError, Unit> Save(User entity) =>
    _repositoryUtils.Save(
      QueryBuilder.Build(
        new Filter<User>([
          new FilterCriterion<User>(e => e.Username, entity.Username)
        ]),
        new Sort<User>(e => e.Username),
        1u,
        0u
      ), entity);

  public Either<RepositoryError, Unit> Drop(Filter<User> filter) =>
    _repositoryUtils.Drop(filter);

  public Either<RepositoryError, User> FindOneById(Username username) =>
    FindOne(
      new Filter<User>([
        new FilterCriterion<User>(e => e.Username, username)
      ]),
      new Sort<User>(e => e.Username)
    ).MapLeft(err => err switch {
      EntityNotFoundByFilter<User> => new EntityNotFoundById<Username>(username),
      _ => err
    });

  public Either<RepositoryError, Unit> DropById(Username username) =>
    Drop(
      new Filter<User>([
        new FilterCriterion<User>(e => e.Username, username)
      ])
    );

  public Either<RepositoryError, User> FindByCredentials(Username username, string password) =>
    FindOneById(username).Bind(user =>
      user.VerifyPassword(password)
        ? Right<RepositoryError, User>(user)
        : Left<RepositoryError, User>(new EntityNotFoundById<Username>(username))
        );
}
