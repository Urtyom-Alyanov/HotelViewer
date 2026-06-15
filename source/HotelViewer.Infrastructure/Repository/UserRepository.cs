using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Repository;
using HotelViewer.Infrastructure.Mappers;
using LanguageExt;
using static LanguageExt.Prelude;

namespace HotelViewer.Infrastructure.Repository;

public class UserRepository(DataAccess db) : IUserRepository {
  private const string Query = "SELECT * FROM Пользователь WHERE ИмяПользователя = ?";

  /// <summary>
  /// Поиск по имени пользователя
  /// </summary>
  /// <param name="username">Имя пользователя</param>
  /// <returns>Пользователь</returns>
  public Either<RepositoryError, User> FindById(Username username) {
    return db
      .LoadTable(Query, username.Value)
      .MapLeft(DataAccessErrorToRepositoryErrorMapper.Map)
      .Bind(table =>
        table.Rows.Count == 0
          ? Left<RepositoryError, User>(new EntityNotFound<Username>(username))
          : Right<RepositoryError, User>(UserMapper.MapFromDb(table.Rows[0]))
      );
  }

  /// <summary>
  /// Сохранить пользователя
  /// </summary>
  /// <param name="entity">Пользователь</param>
  /// <returns>Ошибка или пустота</returns>
  public Either<RepositoryError, Unit> Save(User entity) {
    return db
      .LoadTable(Query, entity.Username.Value)
      .Map(table => UserMapper.MapIntoDb(entity, table))
      .Bind(table => db.SaveTable(Query, table))
      .MapLeft(DataAccessErrorToRepositoryErrorMapper.Map);
  }

  /// <summary>
  /// Поиск пользователя по учётным данным
  /// </summary>
  /// <param name="username">Имя пользователя</param>
  /// <param name="password">Пароль</param>
  /// <returns>Пользователь</returns>
  public Either<RepositoryError, User> FindByCredentials(Username username, string password) {
    return FindById(username).Bind(user => {
      if (user.VerifyPassword(password))
        return Right(user);
      return Left<RepositoryError, User>(new EntityNotFound<Username>(username));
    });
  }
}
