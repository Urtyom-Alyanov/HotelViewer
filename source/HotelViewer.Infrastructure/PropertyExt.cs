using System.Linq.Expressions;
using System.Reflection;
using LanguageExt;
using static LanguageExt.Prelude;

namespace HotelViewer.Infrastructure;

public static class PropertyExt {
  /// <summary>
  /// Получить название свойства из выражения
  /// </summary>
  /// <param name="selector">Выражение со свойством</param>
  /// <typeparam name="TEntity">Субъект</typeparam>
  /// <typeparam name="TValue">Значение</typeparam>
  /// <returns>Название свойства</returns>
  public static Option<string> GetPropertyName<TEntity, TValue>(Expression<Func<TEntity, TValue>> selector) =>
    (selector.Body switch {
      MemberExpression m => Some(m),
      UnaryExpression u => Some(u.Operand as MemberExpression)!,
      _ => None
    }).Map(
      m => m.Member.Name
    );

  /// <summary>
  /// Получить название из карты
  /// </summary>
  /// <param name="map">Карта</param>
  /// <param name="selector">функция с получением</param>
  /// <typeparam name="TEntity">Субъект</typeparam>
  /// <typeparam name="TValue">Значение</typeparam>
  /// <returns>Название свойства</returns>
  public static Option<string> GetCol<TEntity, TValue>(this HashMap<string, string> map, Expression<Func<TEntity, TValue>> selector) =>
    GetPropertyName(selector).Bind(map.Find);

  /// <summary>
  /// Создаёт на базе существующего HashMap инвертированный HashMap
  /// </summary>
  /// <param name="hashMap">Существующий HashMap</param>
  /// <typeparam name="TKey">Ключ</typeparam>
  /// <typeparam name="TValue">Значение</typeparam>
  /// <returns>Инвертированный HashMap</returns>
  public static HashMap<TValue, TKey> Invert<TKey, TValue>(this HashMap<TKey, TValue> hashMap)
    where TKey : notnull => hashMap
      .ToDictionary(pair => pair.Value, pair => pair.Key)
      .ToHashMap();
}
