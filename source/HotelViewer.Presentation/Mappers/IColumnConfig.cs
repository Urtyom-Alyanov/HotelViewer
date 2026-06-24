using System.Windows.Data;

namespace HotelViewer.Presentation.Mappers;

public interface IColumnConfig {
  string Header { get; }
  string PropertyPath { get; }
  string SortPath { get; }
  IValueConverter? Converter { get; }
}
