using HotelViewer.Domain.Value;

namespace HotelViewer.Domain.Entity;

public record ResidentId(int Value);

/// <summary>
/// Жилец
/// </summary>
public class Resident
{
    public ResidentId Id { get; private set; }
    public FullName Name { get; private set; }
    public Address Address { get; private set; }
    public Sex Sex { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }

    public Resident(ResidentId id, FullName name, Address address, Sex sex, PhoneNumber phoneNumber)
    {
        Id = id;
        Name = name;
        Address = address;
        Sex = sex;
        PhoneNumber = phoneNumber;
    }
}