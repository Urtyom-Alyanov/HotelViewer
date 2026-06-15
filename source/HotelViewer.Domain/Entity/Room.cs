using HotelViewer.Domain.Value;

namespace HotelViewer.Domain.Entity;

/// <summary>
/// Вложенный ключ
/// </summary>
/// <param name="Number">Номер комнаты в отеле</param>
/// <param name="HostelId">Идентификатор отеля</param>
public record RoomId(RoomNumber Number, HotelId HotelId);

/// <summary>
/// Тип комнаты
/// </summary>
public enum RoomType {
  Standard = 1,
  Suite = 2,
  JuniorSuite = 3,
  Presidential = 4
}

/// <summary>
/// Комната в отеле
/// </summary>
public class Room {
  public RoomId RoomId { get; init; }
  public RoomType Type { get; private set; }

  public Room(RoomId roomId, RoomType type) {
    RoomId = roomId;
    Type = type;
  }
}
