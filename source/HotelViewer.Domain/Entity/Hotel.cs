using HotelViewer.Domain.Value;

namespace HotelViewer.Domain.Entity;

public record HostelId(int Value);

/// <summary>
/// Отель
/// </summary>
public class Hotel
{
    public HostelId Id { get; private set; }
    public string Name { get; private set; }
    public PhoneNumber Number { get; private set; }
    public Address Address { get; private set; }
    public OrganizationId OrganizationId { get; private set; }

    public Hotel(
        HostelId id,
        string name,
        PhoneNumber number,
        Address address,
        OrganizationId organizationId)
    {
        Id = id;
        Name = name;
        Number = number;
        Address = address;
        OrganizationId = organizationId;
    }
}