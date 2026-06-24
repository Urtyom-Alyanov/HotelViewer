using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Value;
using HotelViewer.Presentation.ViewModels;
using LanguageExt;

namespace HotelViewer.Presentation.Windows.Editors;

public partial class ResidenceEditorWindow : Window, IEntityEditor<Residence> {
  public ObservableCollection<Room> FilteredRooms { get; } = new();

  public ResidenceEditorWindow() {
    InitializeComponent();
    DataContext = this;
  }

  public Residence? Entity { get; private set; }

  private void UpdateRoomFilter(HotelId? selectedHotelId) {
    FilteredRooms.Clear();
    CmbRoom.IsEnabled = false;
    if (selectedHotelId == null) return;

    CmbRoom.IsEnabled = true;

    var rooms = MainViewModel.Instance.Rooms.Items
      .Where(r => r.RoomId.HotelId.Value == selectedHotelId.Value);

    foreach (var room in rooms)
      FilteredRooms.Add(room);
  }

  private void CmbHotel_SelectionChanged(object sender, SelectionChangedEventArgs e) {
    var selectedHotelId = CmbHotel.SelectedValue as HotelId;
    UpdateRoomFilter(selectedHotelId);
  }

  public void SetEntity(Residence entity) {
    TxtId.Text = entity.ResidenceId.Value.ToString();
    TxtId.IsEnabled = false;

    CmbHotel.SelectedValue = entity.HotelId;
    UpdateRoomFilter(entity.HotelId);

    CmbRoom.SelectedValue = entity.Number;
    CmbResident.SelectedValue = entity.ResidentId;

    TxtDaysPerNight.Text = entity.DaysPerNight.ToString();
    DatePicker.SelectedDate = entity.ResidenceAt;
  }

  private void OnlyNumbers_PreviewTextInput(object sender, TextCompositionEventArgs e) {
    e.Handled = !char.IsDigit(e.Text, 0);
  }

  private void BtnSave_Click(object sender, RoutedEventArgs e) {
    try {
      var hotelId = (HotelId)CmbHotel.SelectedValue;
      var roomNumber = (RoomNumber)CmbRoom.SelectedValue;
      var residentId = (ResidentId)CmbResident.SelectedValue;
      var residenceAt = DatePicker.SelectedDate;

      if (residenceAt == null)
        throw new Exception("Не выбрана дата");

      Entity = new Residence(
        new ResidenceId(int.Parse(TxtId.Text == "авто" ? "0" : TxtId.Text)),
        roomNumber,
        hotelId,
        residentId,
        uint.Parse(TxtDaysPerNight.Text),
        DatePicker.SelectedDate!.Value
      );
      DialogResult = true;
    }
    catch (Exception ex) {
      MessageBox.Show("Ошибка валидации: " + ex.Message);
    }
  }
}

