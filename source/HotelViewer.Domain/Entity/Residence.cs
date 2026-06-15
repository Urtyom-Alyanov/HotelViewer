using HotelViewer.Domain.Value;

namespace HotelViewer.Domain.Entity;

public record ResidenceId(int Value);

/// <summary>
/// Проживание жильца в определённой комнате определённого отеля
/// </summary>
public class Residence {
  public ResidenceId ResidenceId { get; private set; }

  public RoomNumber Number { get; private set; }
  public HotelId HotelId { get; private set; }
  public ResidentId ResidentId { get; private set; }

  public uint DaysPerNight { get; private set; }
  public DateTime ResidenceAt { get; private set; }

  public Residence(
      ResidenceId residenceId,
      RoomNumber number,
      HotelId hotelId,
      ResidentId residentId,
      uint daysPerNight,
      DateTime residenceAt) {
    ResidenceId = residenceId;
    HotelId = hotelId;
    Number = number;
    ResidentId = residentId;
    DaysPerNight = daysPerNight;
    ResidenceAt = residenceAt;
  }
}
