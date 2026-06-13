using HotelViewer.Domain.Value;

namespace HotelViewer.Domain.Entity;

public enum RoomType
{
    Standard = 1,
    Suite = 2,
    JuniorSuite = 3,
    Presidential = 4
}

/// <summary>
/// Комната в отеле
/// </summary>
public class Room
{
    public RoomNumber Number { get; private set; }
    public RoomType Type { get; private set; }
    public HostelId HostelId { get; private set; }

    public Room(RoomNumber number, HostelId hostelId, RoomType type)
    {
        Number = number;
        Type = type;
        HostelId = hostelId;
    }
}