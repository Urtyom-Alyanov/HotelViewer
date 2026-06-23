using Microsoft.Win32;

namespace HotelViewer.Presentation.Infrastructure;

public interface IDatabaseLocator {
  string? Locate();
}

public class DatabaseLocator : IDatabaseLocator {
  public string? Locate() {
    var dialog = new OpenFileDialog {
      Title = "Выберите базу данных MS Access",
      Filter = "MS Access Database (*.accdb)|*.accdb",
      CheckFileExists = true
    };

    return dialog.ShowDialog() == true ? dialog.FileName : null;
  }
}
