using System.Windows;
using HotelViewer.Domain.Entity;

namespace HotelViewer.Presentation.Windows.Editors;

public partial class UserEditorWindow : Window, IEntityEditor<User> {
  public IEnumerable<UserRole> AllRoles => Enum.GetValues(typeof(UserRole)).Cast<UserRole>();
  public UserRole SelectedRole { get; set; } = UserRole.Reader;

  private User? _originalUser;

  public UserEditorWindow() {
    InitializeComponent();
    DataContext = this;
  }

  public User? Entity { get; private set; }

  public void SetEntity(User entity) {
    TxtUsername.Text = entity.Username.Value;
    TxtUsername.IsEnabled = false;

    SelectedRole = entity.Role;

    DataContext = null;
    DataContext = this;
  }

  private void BtnSave_Click(object sender, RoutedEventArgs e) {
    try {
      if (string.IsNullOrWhiteSpace(TxtUsername.Text))
        throw new Exception("Имя пользователя не может быть пустым.");

      User user;

      if (_originalUser != null) {
        user = new User(
          _originalUser.Username,
          _originalUser.PasswordHash,
          _originalUser.PasswordSalt,
          SelectedRole
        );
      }
      else {
        if (string.IsNullOrWhiteSpace(TxtNewPassword.Text))
          throw new Exception("Для нового пользователя необходимо задать пароль.");

        user = new User(
          new Username(TxtUsername.Text),
          Array.Empty<byte>(),
          Array.Empty<byte>(),
          SelectedRole
        );
      }

      if (!string.IsNullOrWhiteSpace(TxtNewPassword.Text)) {
        user.HashNewPassword(TxtNewPassword.Text);
      }

      Entity = user;
      DialogResult = true;
    }
    catch (Exception ex) {
      MessageBox.Show("Ошибка: " + ex.Message, "Валидация");
    }
  }
}

