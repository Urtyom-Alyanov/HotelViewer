using System.Data;
using System.Data.OleDb;
using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Repository;
using LanguageExt;
using static LanguageExt.Prelude;

namespace HotelViewer.Infrastructure.Repository;

public class UserRepository(DataAccess db) : IUserRepository
{
    private const string Query = "SELECT * FROM Пользователь WHERE ИмяПользователя = ?";
    
    private User ConvertToDomain(DataRow row)
    {
        return new User(
            new Username(row["ИмяПользователя"].ToString() ?? ""),
            Convert.FromBase64String(row["ХэшПароля"].ToString() ?? ""),
            Convert.FromBase64String(row["СольПароля"].ToString() ?? ""),
            (UserRole)Convert.ToInt32(row["Роль"].ToString())
        );
    }
    
    public Either<RepositoryError, User> GetById(Username username)
    {
        return db.ExecuteCommand(Query, command =>
            {
                var table = new DataTable();
#pragma warning disable CA1416
                command.Parameters.AddWithValue("?", username.Value);

                using var adapter = new OleDbDataAdapter(command);
#pragma warning restore CA1416
                adapter.Fill(table);

                if (table.Rows.Count == 0)
                    return Left<RepositoryError, User>(new EntityNotFound(username));

                DataRow row = table.Rows[0];

                return Right<RepositoryError, User>(ConvertToDomain(row));
            })
            .MapLeft(MapToDomainError);
    }

    public Either<RepositoryError, User> Save(User entity)
    {
        return db.ExecuteCommand(Query, command =>
            {
                var table = new DataTable();
            
#pragma warning disable CA1416
                command.Parameters.AddWithValue("?", entity.Username.Value);
            
                using var adapter = new OleDbDataAdapter(command);
                using var builder = new OleDbCommandBuilder(adapter);
#pragma warning restore CA1416
            
                adapter.Fill(table);
            
                DataRow row;
            
                if (table.Rows.Count == 0)
                {
                    row = table.NewRow();
                    row["ИмяПользователя"] = entity.Username.Value;
                    table.Rows.Add(row);
                }
                else
                {
                    row = table.Rows[0];
                }
            
                row["ХэшПароля"] = entity.PasswordHash;
                row["СольПароля"] = entity.PasswordSalt;
                row["Роль"] = (int)entity.Role;
            
                return db.SaveData(table, Query)
                    .MapLeft(MapToDomainError)
                    .Map(_ => entity);
            })
            .MapLeft(MapToDomainError)
            .Bind(identity => identity);
    }

    public Either<RepositoryError, User> FindByCredentials(Username username, string password)
    {
        return GetById(username).Bind(user =>
        {
            if (user.VerifyPassword(password))
            {
                return Right(user);
            }
            return Left<RepositoryError, User>(new EntityNotFound(username));
        });
    }
    
    private RepositoryError MapToDomainError(DataAccessError error) => error switch
    {
        DatabaseConnectionError dbErr => new InfrastructureFault($"Нет связи с файлом Access. {dbErr.Ex.Message}"),
        QueryExecutionError qErr => new InfrastructureFault($"Кривой SQL запрос. {qErr.Ex.Message}"),
        _ => new InfrastructureFault(error.Message)
    };
}