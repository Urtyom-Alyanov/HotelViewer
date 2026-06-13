using System.Data;
using System.Data.OleDb;
using LanguageExt;
using static LanguageExt.Prelude;

namespace HotelViewer.Infrastructure;

public abstract record DataAccessError(string Message);

public record DriverNotInstalled() : DataAccessError("Драйвер базы данных не установлен (Требуется Microsoft.ACE.OLEDB.12.0)");
public record NormalOperatingSystem() : DataAccessError("Установлена нормальная ОС! Установите ОС Windows и запустите приложение там.");
public record DatabaseConnectionError(Exception Ex) : DataAccessError($"Ошибка подключения к БД: {Ex.Message}");
public record QueryExecutionError(string Query, Exception Ex) : DataAccessError($"Ошибка выполнения запроса [{Query}]: {Ex.Message}");
public record UnknownConnectionError(Exception Ex) : DataAccessError($"Критическая ошибка инфраструктуры: {Ex.Message}");
public record FileNotFoundError(string path) : DataAccessError($"Файл базы данных не найден в {path}");

/// <summary>
/// Инициализация базы данных и низкоуровневый интерфейс
/// </summary>
public class DataAccess
{
    private const string DatabaseProvider = "Microsoft.ACE.OLEDB.12.0";
    private readonly string _connectionString;

    private DataAccess(string databasePath)
    {
        _connectionString = $"Provider={DatabaseProvider};Data Source={databasePath};Persist Security Info=False;";
    }


    /// <summary>
    /// Создание подключения к базе данных.
    /// На нормальных ОС не работает, только на Windows.
    /// </summary>
    /// <param name="databasePath">Путь к базе данных MS Access</param>
    /// <returns>Доступ или ошибка</returns>
    public static Either<DataAccessError, DataAccess> CreateConnection(string databasePath)
    {
        if (!File.Exists(databasePath))
            return Left<DataAccessError, DataAccess>(new FileNotFoundError(databasePath));

        if (!OperatingSystem.IsWindows())
            return Left<DataAccessError, DataAccess>(new NormalOperatingSystem());

        try
        {
            var enumerator = new OleDbEnumerator();
            var dataTable = enumerator.GetElements();

            var isInstalled = dataTable.AsEnumerable()
                .Any(dataRow => dataRow["SOURCES_NAME"]?.ToString() == DatabaseProvider);

            return isInstalled
                ? Right<DataAccessError, DataAccess>(new DataAccess(databasePath))
                : Left<DataAccessError, DataAccess>(new DriverNotInstalled());
        }
        catch (Exception ex)
        {
            return Left<DataAccessError, DataAccess>(new UnknownConnectionError(ex));
        }
    }

    /// <summary>
    /// Исполнить низкоуровневую команду
    /// </summary>
    /// <param name="query">Запрос</param>
    /// <param name="mapFunction">Функция с дальнейшим парсингом</param>
    /// <typeparam name="T">Возвращаемый тип</typeparam>
    /// <returns>Результат исполнения команды</returns>
    public Either<DataAccessError, T> ExecuteCommand<T>(string query, Func<OleDbCommand, T> mapFunction)
    {
        if (!OperatingSystem.IsWindows())
            return Left<DataAccessError, T>(new NormalOperatingSystem());

        try
        {
            using var connection = new OleDbConnection(_connectionString);
            connection.Open();

            using var command = new OleDbCommand(query, connection);

            var result = mapFunction(command);

            return Right<DataAccessError, T>(result);
        }
        catch (OleDbException ex)
        {
            if (ex.Message.Contains("формат базы данных") || ex.Message.Contains("не удалось подключиться"))
            {
                return Left<DataAccessError, T>(new DatabaseConnectionError(ex));
            }

            return Left<DataAccessError, T>(new QueryExecutionError(query, ex));
        }
        catch (Exception ex)
        {
            return Left<DataAccessError, T>(new UnknownConnectionError(ex));
        }
    }

    /// <summary>
    /// Получить данные из запроса
    /// </summary>
    /// <param name="query">SELECT запрос</param>
    /// <returns>Результат получения данных
    /// </returns>
    public Either<DataAccessError, DataTable> GetData(string query) => ExecuteCommand(query, command =>
        {
            var result = new DataTable();

#pragma warning disable CA1416
            using var adapter = new OleDbDataAdapter(command);
#pragma warning restore CA1416
            adapter.Fill(result);

            return result;
        });

    /// <summary>
    /// Сохранить данные в базе данных
    /// </summary>
    /// <param name="data">Данные</param>
    /// <param name="query">SELECT запрос</param>
    /// <returns>Изменённые данные</returns>
    public Either<DataAccessError, DataTable> SaveData(DataTable data, string query) => ExecuteCommand(query, command =>
        {
#pragma warning disable CA1416
            using var adapter = new OleDbDataAdapter(command);
            using var builder = new OleDbCommandBuilder(adapter);

            adapter.InsertCommand = builder.GetInsertCommand();
            adapter.UpdateCommand = builder.GetUpdateCommand();
            adapter.DeleteCommand = builder.GetDeleteCommand();
#pragma warning restore CA1416

            adapter.Update(data);

            return data;
        });
}