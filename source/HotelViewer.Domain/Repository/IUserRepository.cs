using HotelViewer.Domain.Entity;
using LanguageExt;

namespace HotelViewer.Domain.Repository;

public interface IUserRepository : IRepository<User, Username>
{
    public Either<RepositoryError, User> FindByCredentials(Username username, string password);
}