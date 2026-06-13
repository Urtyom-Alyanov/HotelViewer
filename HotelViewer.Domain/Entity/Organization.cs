namespace HotelViewer.Domain.Entity;

public record OrganizationId(int Value);

/// <summary>
/// Организация с отелями
/// </summary>
public class Organization
{
    public OrganizationId Id { get; private set; }
    public string Name { get; private set; }

    public Organization(OrganizationId id, string name)
    {
        Id = id;
        Name = name;
    }
}