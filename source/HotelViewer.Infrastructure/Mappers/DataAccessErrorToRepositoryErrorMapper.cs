using HotelViewer.Domain.Repository;

namespace HotelViewer.Infrastructure.Mappers;

public class DataAccessErrorToRepositoryErrorMapper
{
    private RepositoryError Map(DataAccessError error) => error switch
    {
        DatabaseConnectionError dbErr => new InfrastructureFault($"Связь с базой данных потеряна! {dbErr.Ex.Message}"),
        QueryExecutionError qErr => new InfrastructureFault($"Неверный SQL запрс. {qErr.Ex.Message}"),
        _ => new InfrastructureFault(error.Message)
    };
}