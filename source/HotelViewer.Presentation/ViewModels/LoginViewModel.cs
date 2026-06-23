using System.Windows;
using HotelViewer.ApplicationLayer.Services;
using HotelViewer.Presentation.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace HotelViewer.Presentation.ViewModels;

public class LoginViewModel(AuthService authService, IServiceProvider serviceProvider) : ViewModelBase {
  private string _username = "ARTEMOS";
  public string Username { get => _username; set { _username = value; OnPropertyChanged(); } }

  public RelayCommand LoginCommand => new(obj => {
    var password = (obj as System.Windows.Controls.PasswordBox)?.Password ?? "";

    authService.Login(_username, password).Match(
      err => MessageBox.Show(err.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error),
      user => {
        var sessionContext = serviceProvider.GetRequiredService<SessionContext>();
        sessionContext.SetUser(user);

        var mainWin = serviceProvider.GetRequiredService<MainWindow>();
        mainWin.Show();
        Application.Current.Windows.OfType<LoginWindow>().FirstOrDefault()?.Close();
      }
    );
  });
}
