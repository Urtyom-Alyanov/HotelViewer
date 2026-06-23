using HotelViewer.Domain.Entity;

namespace HotelViewer.Domain.Tests.Entity;

public class UserTest {
  [Fact]
  public void HashNewPassword_ShouldGenerateHashAndSalt()
  {
    // Arrange
    var user = new User(
      new Username("admin"),
      Array.Empty<byte>(),
      Array.Empty<byte>(),
      UserRole.Admin
    );
    string password = "MySecurePassword123";

    // Act
    user.HashNewPassword(password);

    // Assert
    Assert.NotEmpty(user.PasswordHash);
    Assert.NotEmpty(user.PasswordSalt);
    Assert.NotEqual(user.PasswordHash, user.PasswordSalt);
  }

  [Theory]
  [InlineData("password123")]
  [InlineData("!@#$%^&*()_+")]
  [InlineData("Кириллица_Тоже_Работает")]
  public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue(string password)
  {
    // Arrange
    var user = new User(new Username("user"), [], [], UserRole.Reader);
    user.HashNewPassword(password);

    // Act
    bool result = user.VerifyPassword(password);

    // Assert
    Assert.True(result, "Пароль должен был пройти проверку, но не прошел.");
  }

  [Fact]
  public void VerifyPassword_WithWrongPassword_ShouldReturnFalse()
  {
    // Arrange
    var user = new User(new Username("user"), [], [], UserRole.Reader);
    user.HashNewPassword("CorrectPassword");

    // Act
    bool result = user.VerifyPassword("WrongPassword");

    // Assert
    Assert.False(result, "Проверка должна была провалиться, но вернула true.");
  }

  [Fact]
  public void HashNewPassword_ShouldProduceDifferentHashesForSamePassword()
  {
    // Arrange
    var user1 = new User(new Username("u1"), [], [], UserRole.Reader);
    var user2 = new User(new Username("u2"), [], [], UserRole.Reader);
    string commonPassword = "same_password";

    // Act
    user1.HashNewPassword(commonPassword);
    user2.HashNewPassword(commonPassword);

    // Assert
    Assert.NotEqual(user1.PasswordSalt, user2.PasswordSalt);
    Assert.NotEqual(user1.PasswordHash, user2.PasswordHash);
  }
}
