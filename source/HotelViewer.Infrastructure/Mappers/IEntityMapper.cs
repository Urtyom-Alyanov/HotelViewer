using System.Data;
using System.Linq.Expressions;
using LanguageExt;

namespace HotelViewer.Infrastructure.Mappers;

/// <summary>
/// Интерфейс для преобразования доменной сущности <typeparamref name="TEntity"/> в формат понятный ADO.NET
/// </summary>
/// <typeparam name="TEntity">Тип доменной сущности</typeparam>
public interface IEntityMapper<TEntity> {
  /// <summary>
  /// Преобразование ответа базы данных в доменную сущность
  /// </summary>
  /// <param name="dataRow">Сущнность из базы данных</param>
  /// <returns>Доменная сущность</returns>
  public static abstract TEntity MapFromDb(DataRow dataRow);

  /// <summary>
  /// Преобразование доменной сущности в формат понятной базе данных
  /// </summary>
  /// <param name="entity">Доменная сущность</param>
  /// <param name="table">Тапблица из базы данных</param>
  /// <returns>Сырые данные для базы данных</returns>
  public static abstract DataTable MapIntoDb(TEntity entity, DataTable table);

  /// <summary>
  /// Получить имя столбца в базе данных
  /// </summary>
  /// <param name="propertyName">Имя свойства</param>
  /// <returns>Имя колонки</returns>
  public static abstract Option<string> MapPropertyIntoDbColumn<TValue>(Expression<Func<TEntity, TValue>> propertySelector);

  /// <summary>
  /// Получить имя свойства домена
  /// </summary>
  /// <param name="columnName">Имя колонки</param>
  /// <returns>Имя свойства</returns>
  public static abstract Option<string> MapDbColumnIntoProperty(string columnName);
}
