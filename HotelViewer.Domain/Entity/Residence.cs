using HotelViewer.Domain.Value;

namespace HotelViewer.Domain.Entity;

/// <summary>
/// Проживание жильца в определённой комнате определённого отеля
/// </summary>
public class Residence
{
    public RoomNumber Number { get; private set; }
    public HostelId HostelId { get; private set; }
    public ResidentId ResidentId { get; private set; }

    public Residence(
        RoomNumber number,
        HostelId hostelId,
        ResidentId residentId)
    {
        HostelId = hostelId;
        Number = number;
        ResidentId = residentId;
    }
}