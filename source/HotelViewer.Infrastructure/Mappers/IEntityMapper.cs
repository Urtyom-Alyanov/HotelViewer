using System.Data;

namespace HotelViewer.Infrastructure.Mappers;

/// <summary>
/// Интерфейс для преобразования доменной сущности <typeparamref name="TEntity"/> в формат понятный ADO.NET
/// </summary>
/// <typeparam name="TEntity">Тип доменной сущности</typeparam>
public interface IEntityMapper<TEntity>
{
    /// <summary>
    /// Преобразование ответа базы данных в доменную сущность
    /// </summary>
    /// <param name="dataRow">Сущнность из базы данных</param>
    /// <returns>Доменная сущность</returns>
    public TEntity MapFromDb(DataRow dataRow);

    /// <summary>
    /// Преобразование доменной сущности в формат понятной базе данных
    /// </summary>
    /// <param name="entity">Доменная сущность</param>
    /// <param name="table">Тапблица из базы данных</param>
    /// <returns>Сырые данные для базы данных</returns>
    public DataRow MapIntoDb(TEntity entity, DataTable table);
}