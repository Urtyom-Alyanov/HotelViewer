using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using HotelViewer.ApplicationLayer.Services;
using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Repository;
using HotelViewer.Infrastructure;
using HotelViewer.Infrastructure.Repository;
using HotelViewer.Presentation.Infrastructure;
using HotelViewer.Presentation.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace HotelViewer.Presentation;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application {
  public static IServiceProvider ServiceProvider { get; private set; } = null!;

  protected override void OnStartup(StartupEventArgs e) {
    var services = new ServiceCollection();

    var locator = new DatabaseLocator();
    string? dbPath = locator.Locate();

    if (string.IsNullOrEmpty(dbPath)) {
      MessageBox.Show("Работа приложения невозможна без выбора базы данных.");
      Shutdown();
      return;
    }

    ConfigureServices(services, dbPath);

    ServiceProvider = services.BuildServiceProvider();

    var loginWindow = ServiceProvider.GetRequiredService<LoginWindow>();
    loginWindow.Show();
  }

  private void ConfigureServices(IServiceCollection services, string dbPath) {
    services.AddSingleton(s => DataAccess.CreateConnection(dbPath)
      .IfLeft(err => throw new Exception(err.Message)));

    // Инициализация репозиториев
    services.AddSingleton<IResidenceRepository, ResidenceRepository>();
    services.AddSingleton<IRoomRepository, RoomRepository>();
    services.AddSingleton<IResidentRepository, ResidentRepository>();
    services.AddSingleton<IHotelRepository, HotelRepository>();
    services.AddSingleton<IOrganizationRepository, OrganizationRepository>();
    services.AddSingleton<IUserRepository, UserRepository>();

    // Аутентификация
    services.AddSingleton<SessionContext>();
    services.AddSingleton<AuthService>();

    services.AddTransient(typeof(EntityService<,>));
    services.AddTransient(typeof(ExportService<,>));

    services.AddTransient<LoginViewModel>();
    services.AddTransient<MainViewModel>();

    services.AddTransient<LoginWindow>();
    services.AddTransient<MainWindow>();
  }
}
