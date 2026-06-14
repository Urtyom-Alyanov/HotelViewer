using System.Data;
using System.Data.OleDb;
using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Repository;
using LanguageExt;
using static LanguageExt.Prelude;

namespace HotelViewer.Infrastructure.Repository;

public class OrganizationRepository(DataAccess db) : IOrganizationRepository
{
    private const string Query = "SELECT * FROM Организация WHERE ИдентификаторОрганизации = ?";
    
    private Organization ConvertToDomain(DataRow row)
    {
        return new Organization(
            new OrganizationId(Convert.ToInt32(row["ИдентификаторОрганизации"])),
            row["Название"].ToString() ?? ""
        );
    }
    
    public Either<RepositoryError, Organization> GetById(OrganizationId id)
    {
        return db.ExecuteCommand(Query, command =>
        {
            var table = new DataTable();
#pragma warning disable CA1416
            command.Parameters.AddWithValue("?", id.Value);
                
            using var adapter = new OleDbDataAdapter(command);
#pragma warning restore CA1416
            adapter.Fill(table);
            
            if (table.Rows.Count == 0)
                return Left<RepositoryError, Organization>(new EntityNotFound(id));

            DataRow row = table.Rows[0];
            
            return Right<RepositoryError, Organization>(ConvertToDomain(row));
        }).MapLeft(MapToDomainError)
        .Bind(identity => identity);
    }

    public Either<RepositoryError, Organization> Save(Organization entity)
    {
        return db.ExecuteCommand(Query, command =>
            {
                var table = new DataTable();
            
#pragma warning disable CA1416
                command.Parameters.AddWithValue("?", entity.Id.Value);
            
                using var adapter = new OleDbDataAdapter(command);
                using var builder = new OleDbCommandBuilder(adapter);
#pragma warning restore CA1416
            
                adapter.Fill(table);
            
                DataRow row;
            
                if (table.Rows.Count == 0)
                {
                    row = table.NewRow();
                    row["ИдентификаторОрганизации"] = entity.Id.Value;
                    table.Rows.Add(row);
                }
                else
                {
                    row = table.Rows[0];
                }
            
                row["Название"] = entity.Name;
                
                return db.SaveData(table, Query)
                    .MapLeft(MapToDomainError)
                    .Map(_ => entity);
            })
            .MapLeft(MapToDomainError)
            .Bind(identity => identity);
    }
    
    private RepositoryError MapToDomainError(DataAccessError error) => error switch
    {
        DatabaseConnectionError dbErr => new InfrastructureFault($"Нет связи с файлом Access. {dbErr.Ex.Message}"),
        QueryExecutionError qErr => new InfrastructureFault($"Кривой SQL запрос. {qErr.Ex.Message}"),
        _ => new InfrastructureFault(error.Message)
    };
}