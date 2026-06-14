using System.Text;
using Argon2Sharp;

namespace HotelViewer.Domain.Entity;

/// <summary>
/// Роль пользователя
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Пользователь, который умеет только читать
    /// </summary>
    Reader,
    /// <summary>
    /// Пользователь, что умеет изменять данные
    /// </summary>
    Redactor,
    /// <summary>
    /// Может выдавать роли другим участникам
    /// </summary>
    Admin,
}

public record Username(string Value);

/// <summary>
/// Пользователь админки
/// </summary>
public class User
{
    public Username Username { get; private set; }
    public UserRole Role { get; private set; }

    public byte[] PasswordHash { get; private set; }
    public byte[] PasswordSalt { get; private set; }

    /// <summary>
    /// Сериализация пользователя из инфраструктурного слоя
    /// </summary>
    /// <param name="username">Имя пользователя</param>
    /// <param name="passwordHash">Хэш пароля в байтах. Зараннее декодировать с base64 (или в чём там хранится в БД)</param>
    /// <param name="passwordSalt">Соль пароля в байтах. Зараннее декодировать с base64 (или в чём там хранится в БД)</param>
    /// <param name="role">Роль</param>
    public User(
        Username username,
        byte[] passwordHash,
        byte[] passwordSalt,
        UserRole role)
    {
        Username = username;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
        Role = role;
    }

    /// <summary>
    /// Задать новый пароль пользователю
    /// </summary>
    /// <param name="newPassword">Новый пароль</param>
    public void HashNewPassword(string newPassword)
    {
        var salt = Argon2.GenerateSalt();
        var argonParams = Argon2Parameters.CreateDefault() with { Salt = salt };
        var argon2 = new Argon2(argonParams);

        var passwordBytes = Encoding.UTF8.GetBytes(newPassword).AsSpan();

        var hash = argon2.Hash(passwordBytes);

        PasswordHash = hash;
        PasswordSalt = salt;
    }

    /// <summary>
    /// Проверить пароль
    /// </summary>
    /// <param name="password"></param>
    /// <returns>Результат проверки. true - если хеши совпадают, false - если нет</returns>
    public bool VerifyPassword(string password)
    {
        var argonParams = Argon2Parameters.CreateDefault() with { Salt = PasswordSalt };
        var argon2 = new Argon2(argonParams);

        var passwordByte = Encoding.UTF8.GetBytes(password).AsSpan();

        return argon2.Verify(passwordByte, PasswordHash.AsSpan());
    }
}