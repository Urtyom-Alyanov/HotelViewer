using System.Data;
using System.Linq.Expressions;
using LanguageExt;
using static LanguageExt.Prelude;
using Array = System.Array;

namespace HotelViewer.Infrastructure;

public static class DataRowExtensions {
  public static string Str(this DataRow row, string col) => row[col]?.ToString() ?? "";
  public static int Int(this DataRow row, string col) => Convert.ToInt32(row[col]);
  public static uint UInt(this DataRow row, string col) => Convert.ToUInt32(row[col]);
  public static DateTime DateTime(this DataRow row, string col) => Convert.ToDateTime(row[col]);
  public static byte[] Base64(this DataRow row, string col) => Convert.FromBase64String(row.Str(col));

  public static string Str(this DataRow row, Option<string> col) => col.Match(row.Str, () => "");
  public static int Int(this DataRow row, Option<string> col) => col.Match(row.Int, () => 0);
  public static uint UInt(this DataRow row, Option<string> col) => col.Match(row.UInt, () => 0u);
  public static byte[] Base64(this DataRow row, Option<string> col) => col.Match(c => Convert.FromBase64String(row.Str(c)), () => Array.Empty<byte>());
  public static DateTime DateTime(this DataRow row, Option<string> col) => col.Match(row.DateTime, () => new DateTime());

  public static string Str<TEntity>(this DataRow row, HashMap<string, string> map, Expression<Func<TEntity, object>> selector) => row.Str(map.GetCol(selector));
  public static int Int<TEntity>(this DataRow row, HashMap<string, string> map, Expression<Func<TEntity, object>> selector) => row.Int(map.GetCol(selector));
  public static uint UInt<TEntity>(this DataRow row, HashMap<string, string> map, Expression<Func<TEntity, object>> selector) => row.UInt(map.GetCol(selector));
  public static byte[] Base64<TEntity>(this DataRow row, HashMap<string, string> map, Expression<Func<TEntity, object>> selector) => row.Base64(map.GetCol(selector));
  public static DateTime DateTime<TEntity>(this DataRow row, HashMap<string, string> map, Expression<Func<TEntity, object>> selector) => row.DateTime(map.GetCol(selector));

  // В обратную сторону, проще говоря
  public static DataRow Set(this DataRow row, Option<string> col, object val) =>
    col.Match(c => {
      if (row.Table.Columns.Contains(c)) row[c] = val ?? DBNull.Value;
      return row;
    }, () => row);

  public static DataRow SetBase64(this DataRow row, Option<string> col, byte[] data) =>
    row.Set(col, data != null ? Convert.ToBase64String(data) : DBNull.Value);

  public static DataRow Set<TEntity>(this DataRow row, HashMap<string, string> map, Expression<Func<TEntity, object>> selector, object val) =>
    row.Set(map.GetCol(selector), val);

  public static DataRow SetBase64<TEntity>(this DataRow row, HashMap<string, string> map, Expression<Func<TEntity, object>> selector, byte[] data) =>
    row.SetBase64(map.GetCol(selector), data);

  public static DataRow GetOrNewRow(this DataTable table) {
    if (table.Rows.Count > 0) return table.Rows[0];

    var row = table.NewRow();
    table.Rows.Add(row);
    return row;
  }
}
