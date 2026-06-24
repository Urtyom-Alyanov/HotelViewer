using System.Linq.Expressions;
using System.Windows.Data;
using HotelViewer.Presentation.Extensions;

namespace HotelViewer.Presentation.Mappers;

public class ColumnInfo<TEntity>(
  string header,
  Expression<Func<TEntity, object>> propertySelector,
  Expression<Func<TEntity, object>>? sortSelector = null,
  IValueConverter? converter = null
) : IColumnConfig {
  public string Header => header;
  public string PropertyPath => PropertyExt.GetPath(propertySelector);
  public string SortPath => sortSelector != null ? PropertyExt.GetPath(sortSelector) : PropertyPath;
  public IValueConverter? Converter => converter;
}
