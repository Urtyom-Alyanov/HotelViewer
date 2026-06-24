namespace HotelViewer.ApplicationLayer;

public class DomainObjectFormatter {
  public static string Format(object? value) {
    if (value == null) return "";

    var type = value.GetType();

    if (type.IsPrimitive || value is string || value is DateTime || value is decimal)
      return value.ToString() ?? "";

    var toDbMethod = type.GetMethod("ToDbValue");
    if (toDbMethod != null)
      return toDbMethod.Invoke(value, null)?.ToString() ?? "";

    var valueProp = type.GetProperty("Value");
    if (valueProp != null)
      return valueProp.GetValue(value)?.ToString() ?? "";

    var props = type.GetProperties();
    if (props.Length > 0) {
      var parts = props.Select(p => Format(p.GetValue(value)));
      return string.Join(" | ", parts.Where(s => !string.IsNullOrEmpty(s)));
    }

    return value.ToString() ?? "";
  }
}
