using HotelViewer.Domain.Value;

namespace HotelViewer.Domain.Entity;

public enum RoomType
{
    Looks
}

/// <summary>
/// Комната в отеле
/// </summary>
public class Room
{
    public RoomNumber Number { get; private set; }
    public RoomType Type { get; private set; }
    public HostelId HostelId { get; private set; }

    public Room(RoomNumber number, RoomType type, HostelId id)
    {
        Number = number;
        Type = type;
        HostelId = id;
    }
}