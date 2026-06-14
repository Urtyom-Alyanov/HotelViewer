using System.Data;
using System.Data.OleDb;
using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Repository;
using HotelViewer.Domain.Value;
using LanguageExt;
using static LanguageExt.Prelude;

namespace HotelViewer.Infrastructure.Repository;

public class HotelRepository(DataAccess db) : IHotelRepository
{
    private const string Query = "SELECT * FROM Гостиница WHERE ИдентификаторГостиницы = ?";

    private Hotel ConvertToDomain(DataRow row)
    {
        return new Hotel(
            id: new HostelId(Convert.ToInt32(row["ИдентификаторГостиницы"])),
            name: row["ИдентификаторГостиницы"].ToString() ?? "",
            address: new Address(row["Адрес"].ToString() ?? ""),
            number: new PhoneNumber(row["ТелефонДежурной"].ToString() ?? ""),
            organizationId: new OrganizationId(Convert.ToInt32(row["ИдентификаторОрганизации"]))
            );
    }
    
    public Either<RepositoryError, Hotel> GetById(HostelId id)
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
                return Left<RepositoryError, Hotel>(new EntityNotFound(id));

            DataRow row = table.Rows[0];
            
            return Right<RepositoryError, Hotel>(ConvertToDomain(row));
        }).MapLeft(MapToDomainError)
            .Bind(identity => identity);
    }

    public Either<RepositoryError, Hotel> Save(Hotel entity)
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
                    row["ИдентификаторГостиницы"] = entity.Id.Value;
                    table.Rows.Add(row);
                }
                else
                {
                    row = table.Rows[0];
                }
            
                row["ТелефонДежурной"] = entity.Number.Value;
                row["Адрес"] = entity.Address.Value;
                row["Название"] = entity.Name;
                row["ИдентификторОрганизации"] = entity.OrganizationId.Value;
            
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