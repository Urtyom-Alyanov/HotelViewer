namespace HotelViewer.Domain.Helper;

/// <summary>
/// Критерий сортировки
/// </summary>
public record Sort<TField>(TField Field, bool Ascending = true) where TField : Enum;
