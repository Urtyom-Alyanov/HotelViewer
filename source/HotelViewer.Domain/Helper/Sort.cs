using System.Linq.Expressions;

namespace HotelViewer.Domain.Helper;

/// <summary>
/// Критерий сортировки
/// </summary>
public record Sort<TEntity>(Expression<Func<TEntity, object>> PropertySelector, bool Ascending = true);
