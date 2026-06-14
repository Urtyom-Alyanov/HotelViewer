using System.Data;
using System.Data.OleDb;
using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Repository;
using HotelViewer.Domain.Value;
using LanguageExt;
using static LanguageExt.Prelude;

namespace HotelViewer.Infrastructure.Repository;

public class ResidentRepository(DataAccess db) : IResidentRepository
{
    private const string Query = "SELECT * FROM Проживающий WHERE ИдентификаторПроживающего = ?";
    
    private Resident ConvertToDomain(DataRow row)
    {
        return new Resident(
            new ResidentId(Convert.ToInt32(row["ИдентификаторПроживающего"])),
            FullName.FromDbValue(row["ФИО"].ToString() ?? ""),
            new Address(row["Адрес"].ToString() ?? ""),
            (Sex)Convert.ToInt32(row["Пол"]),
            new PhoneNumber(row["НомерТелефона"].ToString() ?? "")
            );
    }

    public Either<RepositoryError, Resident> GetById(ResidentId id)
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
                return Left<RepositoryError, Resident>(new EntityNotFound(id));

            DataRow row = table.Rows[0];
            
            return Right<RepositoryError, Resident>(ConvertToDomain(row));
        }).MapLeft(MapToDomainError);
    }

    public Either<RepositoryError, Resident> Save(Resident entity)
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
                    row["ИдентификаторПроживающего"] = entity.Id.Value;
                    table.Rows.Add(row);
                }
                else
                {
                    row = table.Rows[0];
                }
            
                row["НомерТелефона"] = entity.PhoneNumber.Value;
                row["Адрес"] = entity.Address.Value;
                row["ФИО"] = entity.Name.ToDbValue();
                row["Пол"] = (int)entity.Sex;
            
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