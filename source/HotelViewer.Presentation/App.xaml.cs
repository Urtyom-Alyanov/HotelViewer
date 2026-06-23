using System.Windows;
using HotelViewer.ApplicationLayer.Services;
using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Repository;
using HotelViewer.Infrastructure;
using HotelViewer.Infrastructure.Repository;
using HotelViewer.Presentation.Infrastructure;
using HotelViewer.Presentation.ViewModels;
using HotelViewer.Presentation.Windows;
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
    services.AddSingleton(
      _ =>
        DataAccess.CreateConnection(dbPath)
        .IfLeft(err => {
          MessageBox.Show(err.Message, "Подключение к базе данных");
          throw new Exception(err.Message);
        })
      );

    services.AddSingleton<DbSeeder>();

    // Инициализация репозиториев
    services.AddSingleton<IResidenceRepository, ResidenceRepository>();
    services.AddSingleton<IRoomRepository, RoomRepository>();
    services.AddSingleton<IResidentRepository, ResidentRepository>();
    services.AddSingleton<IHotelRepository, HotelRepository>();
    services.AddSingleton<IOrganizationRepository, OrganizationRepository>();
    services.AddSingleton<IUserRepository, UserRepository>();

    services.AddSingleton<IRepository<Hotel, HotelId>>(s => s.GetRequiredService<IHotelRepository>());
    services.AddSingleton<IRepository<Resident, ResidentId>>(s => s.GetRequiredService<IResidentRepository>());
    services.AddSingleton<IRepository<Residence, ResidenceId>>(s => s.GetRequiredService<IResidenceRepository>());
    services.AddSingleton<IRepository<Organization, OrganizationId>>(s => s.GetRequiredService<IOrganizationRepository>());
    services.AddSingleton<IRepository<Room, RoomId>>(s => s.GetRequiredService<IRoomRepository>());
    services.AddSingleton<IRepository<User, Username>>(s => s.GetRequiredService<IUserRepository>());

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
