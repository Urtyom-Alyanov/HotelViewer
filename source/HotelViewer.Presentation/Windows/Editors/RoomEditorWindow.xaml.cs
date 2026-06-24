using System.Windows;
using System.Windows.Input;
using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Value;

namespace HotelViewer.Presentation.Windows.Editors;

public partial class RoomEditorWindow : Window, IEntityEditor<Room> {
  public IEnumerable<RoomType> AllTypes => Enum.GetValues(typeof(RoomType)).Cast<RoomType>();
  public RoomType SelectedRoomType { get; set; } = RoomType.Standard;

  public RoomEditorWindow() {
    InitializeComponent();
    DataContext = this;
  }

  public Room? Entity { get; private set; }

  public void SetEntity(Room entity) {
    TxtRoomNumber.Text = entity.RoomId.Number.Room.ToString();
    TxtRoomNumber.IsEnabled = false;

    TxtRoomStage.Text = entity.RoomId.Number.Stage.ToString();
    TxtRoomStage.IsEnabled = false;

    CmbHotel.SelectedValue = entity.RoomId.HotelId;
    CmbHotel.IsEnabled = false;

    SelectedRoomType = entity.Type;

    DataContext = null;
    DataContext = this;
  }

  private void OnlyNumbers_PreviewTextInput(object sender, TextCompositionEventArgs e) {
    e.Handled = !char.IsDigit(e.Text, 0) && !(e.Text.Length <= 1);
  }

  private void BtnSave_Click(object sender, RoutedEventArgs e) {
    try {
      var selectedHotelId = CmbHotel.SelectedValue as HotelId;
      if (selectedHotelId == null) throw new Exception("Выберите отель!");

      Entity = new Room(
        new RoomId(
          new RoomNumber(int.Parse(TxtRoomStage.Text), int.Parse(TxtRoomNumber.Text)),
          selectedHotelId
          ),
        SelectedRoomType
      );
      DialogResult = true;
    }
    catch (Exception ex) {
      MessageBox.Show("Ошибка валидации: " + ex.Message);
    }
  }
}

