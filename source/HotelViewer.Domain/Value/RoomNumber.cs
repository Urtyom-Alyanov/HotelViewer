namespace HotelViewer.Domain.Value;

public record RoomNumber(int Stage, int Room) {
  /// <summary>
  /// Превращает объект-значение в чистое число для базы данных (например, Stage: 1, Room: 1 -> 101)
  /// </summary>
  public int ToDbValue() {
    return (Stage * 100) + Room;
  }

  /// <summary>
  /// Математически парсит число из БД обратно в доменный record (например, 101 -> Stage: 1, Room: 1)
  /// </summary>
  public static RoomNumber FromDbValue(int dbValue) {
    int stage = dbValue / 100;
    int room = dbValue % 100;

    return new RoomNumber(stage, room);
  }
}
