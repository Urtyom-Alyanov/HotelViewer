using System.Data;
using HotelViewer.Domain.Entity;

namespace HotelViewer.Infrastructure.Mappers;

public class UserMapper : IEntityMapper<User> {
  public static User MapFromDb(DataRow dataRow) {
    return new User(
        new Username(dataRow.Str("ИмяПользователя")),
        dataRow.Base64("ХэшПароля"),
        dataRow.Base64("СольПароля"),
        (UserRole)dataRow.Int("Роль")
    );
  }

  public static DataTable MapIntoDb(User entity, DataTable table) {
    return table.GetOrNewRow()
        .Set("ИмяПользователя", entity.Username.Value)
        .SetBase64("ХэшПароля", entity.PasswordHash)
        .SetBase64("СольПароля", entity.PasswordSalt)
        .Set("Роль", (int)entity.Role)
        .Table;
  }
}
