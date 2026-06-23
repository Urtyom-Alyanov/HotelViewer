using HotelViewer.ApplicationLayer.Errors;
using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Repository;
using LanguageExt;

namespace HotelViewer.ApplicationLayer.Services;

public class AuthService(IUserRepository userRepository, SessionContext sessionContext) {
  public Either<ApplicationError, User> Login(string login, string password) {
    return userRepository.FindByCredentials(new Username(login), password)
      .MapLeft(err => (ApplicationError)new RepositoryFailure(err))
      .Map(user => {
        sessionContext.SetUser(user);
        return user;
      });
  }

  public void Logout() => sessionContext.Clear();
}
