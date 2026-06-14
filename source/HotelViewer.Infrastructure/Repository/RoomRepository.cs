using System.Data;
using System.Data.OleDb;
using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Repository;
using HotelViewer.Domain.Value;
using LanguageExt;
using static LanguageExt.Prelude;

namespace HotelViewer.Infrastructure.Repository;

public class RoomRepository(DataAccess db) : IRoomRepository
{
    private const string Query = "SELECT * FROM Номер WHERE Номер = ? AND ИдентификаторГостиницы = ?";

    private Room ConvertToDomain(DataRow row)
    {
        var id = new RoomId(
            RoomNumber.FromDbValue(Convert.ToInt32(row["Номер"])),
            new HostelId(Convert.ToInt32(row["ИдентификаторГостиницы"]))
        );
        
        return new Room(
            id, (RoomType)Convert.ToInt32(row["ТипНомера"])
        );
    }
    
    /// <summary>
    /// Получить комнату по идентификатору
    /// </summary>
    /// <param name="id">Сложный идентификатор состоящий из номера и идентификатора гостиницы</param>
    /// <returns>Комната</returns>
    public Either<RepositoryError, Room> GetById(RoomId id)
    {
        var number = id.Number;
        var hostelId = id.HostelId;

        return db.ExecuteCommand(Query, command =>
            {
                var table = new DataTable();
#pragma warning disable CA1416
                command.Parameters.AddWithValue("?", number.ToDbValue());
                command.Parameters.AddWithValue("?", hostelId.Value);

                using var adapter = new OleDbDataAdapter(command);
#pragma warning restore CA1416
                adapter.Fill(table);

                if (table.Rows.Count == 0)
                    return Left<RepositoryError, Room>(new EntityNotFound($"Номер {number} в гостинице {hostelId}"));

                DataRow row = table.Rows[0];

                return Right<RepositoryError, Room>(ConvertToDomain(row));
            })
            .MapLeft(MapToDomainError)
            .Bind(identity => identity);
    }

    /// <summary>
    /// Сохранение комнаты
    /// </summary>
    /// <param name="entity">Комната в гостинице</param>
    /// <returns>Изменённая комната</returns>
    public Either<RepositoryError, Room> Save(Room entity)
    {
        return db.ExecuteCommand(Query, command =>
        {
            var table = new DataTable();
            
#pragma warning disable CA1416
            command.Parameters.AddWithValue("?", entity.RoomId.Number.ToDbValue());
            command.Parameters.AddWithValue("?", entity.RoomId.HostelId.Value);
            
            using var adapter = new OleDbDataAdapter(command);
            using var builder = new OleDbCommandBuilder(adapter);
#pragma warning restore CA1416
            
            adapter.Fill(table);
            
            DataRow row;
            
            if (table.Rows.Count == 0)
            {
                row = table.NewRow();
                row["Номер"] = entity.RoomId.Number.ToDbValue();
                row["ИдентификаторГостиницы"] = entity.RoomId.HostelId.Value;
                table.Rows.Add(row);
            }
            else
            {
                row = table.Rows[0];
            }
            
            row["ТипНомера"] = (int)entity.Type;
            
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