namespace HotelViewer.Domain.Value;

public record FullName(string LastName, string FirstName, string MiddleName)
{
    public static FullName FromDbValue(string name)
    {
        var fullName = name.Split();
            
            return new FullName(fullName[0], fullName[1], fullName[2]);
    }
    
    public string ToDbValue()
    {
        return $"{LastName} {FirstName} {MiddleName}";
    }
}