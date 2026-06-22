using HotelViewer.Domain.Entity;
using LanguageExt;

namespace HotelViewer.Domain.Repository;

public interface IUserRepository : IRepository<User, Username> {
  /// <summary>
  /// Получить пользователя из учётных данных
  /// </summary>
  /// <param name="username">Имя пользователя</param>
  /// <param name="password">Пароль</param>
  /// <returns>Пользователь или ошибка репозитория</returns>
  public Either<RepositoryError, User> FindByCredentials(Username username, string password);
}
