using System.Data;

namespace HotelViewer.Infrastructure;

public static class DataRowExtensions
{
    public static string Str(this DataRow row, string col) => row[col]?.ToString() ?? "";
    public static int Int(this DataRow row, string col) => Convert.ToInt32(row[col]);
    public static uint UInt(this DataRow row, string col) => Convert.ToUInt32(row[col]);
    public static DateTime DateTime(this DataRow row, string col) => Convert.ToDateTime(row[col]);
    public static byte[] Base64(this DataRow row, string col) => Convert.FromBase64String(row.Str(col));
    public static DataRow Set(this DataRow row, string col, object val)
    {
        row[col] = val ?? DBNull.Value;
        return row;
    }
    public static DataRow SetBase64(this DataRow row, string col, byte[] data) =>
        row.Set(col, Convert.ToBase64String(data));
    public static DataRow GetOrNewRow(this DataTable table)
    {
        if (table.Rows.Count > 0) return table.Rows[0];

        var row = table.NewRow();
        table.Rows.Add(row);
        return row;
    }
}