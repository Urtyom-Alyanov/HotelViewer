using System.Data;
using System.Data.OleDb;
using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Repository;
using HotelViewer.Domain.Value;
using LanguageExt;
using static LanguageExt.Prelude;

namespace HotelViewer.Infrastructure.Repository;

public class ResidenceRepository(DataAccess db) : IResidenceRepository
{
    private const string Query = "SELECT * FROM Проживание WHERE ИдентификаторПроживания = ?";
    private const string QueryByRoom = "SELECT * FROM Проживание WHERE ИдентификаторПроживания = ? AND Номер = ? AND ИдентификаторГостиницы = ?";

    private Residence ConvertToDomain(DataRow row)
    {
        return new Residence(
            new ResidenceId(Convert.ToInt32(row["ИдентификаторПроживания"])),
            RoomNumber.FromDbValue(Convert.ToInt32(row["Номер"])),
            new HostelId(Convert.ToInt32(row["ИдентификаторГостиницы"])),
            new ResidentId(Convert.ToInt32(row["ИдентификаторПроживающего"])),
            Convert.ToUInt32(row["НаСколько"]),
            Convert.ToDateTime(row["ДатаПрибытия"])
            );
    }
    
    public Either<RepositoryError, Residence> GetById(ResidenceId id)
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
                return Left<RepositoryError, Residence>(new EntityNotFound(id));

            DataRow row = table.Rows[0];
            
            return Right<RepositoryError, Residence>(ConvertToDomain(row));
        }).MapLeft(MapToDomainError);
    }

    public Either<RepositoryError, Residence> Save(Residence entity)
    {
        return db.ExecuteCommand(Query, command =>
            {
                var table = new DataTable();
            
#pragma warning disable CA1416
                command.Parameters.AddWithValue("?", entity.ResidenceId.Value);
            
                using var adapter = new OleDbDataAdapter(command);
                using var builder = new OleDbCommandBuilder(adapter);
#pragma warning restore CA1416
            
                adapter.Fill(table);
            
                DataRow row;
            
                if (table.Rows.Count == 0)
                {
                    row = table.NewRow();
                    row["ИдентификаторПроживания"] = entity.ResidenceId.Value;
                    table.Rows.Add(row);
                }
                else
                {
                    row = table.Rows[0];
                }
            
                row["ДатаПрибытия"] = entity.ResidenceAt;
                row["НаСколько"] = entity.DaysPerNight;
                row["ИдентификаторПроживающего"] = entity.ResidentId.Value;
                row["Номер"] = entity.Number.ToDbValue();
                row["ИдентификаторГостиницы"] = entity.HostelId.Value;
                
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