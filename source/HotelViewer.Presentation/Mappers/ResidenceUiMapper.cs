using HotelViewer.Domain.Entity;
using HotelViewer.Presentation.Converters;

namespace HotelViewer.Presentation.Mappers;

public class ResidenceUiMapper {
  public static List<IColumnConfig> Columns => new() {
    new ColumnInfo<Residence>("ID", e => e.ResidenceId, e => e.ResidenceId),
    new ColumnInfo<Residence>("Номер", e => e.Number, e => e.Number, new DomainObjectConverter()),
    new ColumnInfo<Residence>("Отель", e => e.HotelId, e => e.HotelId, new LookupConverter()),
    new ColumnInfo<Residence>("На сколько дней", e => e.DaysPerNight, e => e.DaysPerNight),
    new ColumnInfo<Residence>("Время прибытия", e => e.ResidenceAt, e => e.ResidenceAt),
    new ColumnInfo<Residence>("Жилец", e => e.ResidentId, e => e.ResidentId, new LookupConverter()),
  };
}
