using System.Data;
using System.Linq.Expressions;
using HotelViewer.Domain.Entity;
using LanguageExt;
using static LanguageExt.Prelude;

namespace HotelViewer.Infrastructure.Mappers;

public class UserMapper : IEntityMapper<User> {
  private static readonly HashMap<string, string> PropToCol = HashMap(
    (nameof(User.Username), "ИмяПользователя"),
    (nameof(User.PasswordHash), "ХэшПароля"),
    (nameof(User.PasswordSalt), "СольПароля"),
    (nameof(User.Role), "Роль")
  );

  private static readonly HashMap<string, string> ColToProp = PropToCol.Invert();

  public static User MapFromDb(DataRow dataRow) =>
    new(
      new Username(dataRow.Str<User>(ColToProp, u => u.Username)),
      dataRow.Base64<User>(ColToProp, u => u.PasswordHash),
      dataRow.Base64<User>(ColToProp, u => u.PasswordSalt),
      (UserRole)dataRow.Int<User>(ColToProp, u => u.Role)
    );

  public static DataTable MapIntoDb(User entity, DataTable table) =>
    table.GetOrNewRow()
      .Set<User>(PropToCol, u => u.Username, entity.Username.Value)
      .SetBase64<User>(PropToCol, u => u.PasswordHash, entity.PasswordHash)
      .SetBase64<User>(PropToCol, u => u.PasswordSalt, entity.PasswordSalt)
      .Set<User>(PropToCol, u => u.Role, (int)entity.Role)
      .Table;

  public static Option<string> MapPropertyIntoDbColumn<TValue>(
    Expression<Func<User, TValue>> propertySelector) =>
    PropertyExt.GetPropertyName(propertySelector).Bind(name => PropToCol.Find(name));

  public static Option<string> MapDbColumnIntoProperty(string columnName) =>
    ColToProp.Find(columnName);
}
