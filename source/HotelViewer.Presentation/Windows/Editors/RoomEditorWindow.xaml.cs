using System.Windows;
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
    TxtRoomNumber.Text = entity.RoomId.Number.ToDbValue().ToString();
    TxtRoomNumber.IsEnabled = false;

    TxtHotelId.Text = entity.RoomId.HotelId.Value.ToString();
    TxtHotelId.IsEnabled = false;

    SelectedRoomType = entity.Type;
  }

  private void BtnSave_Click(object sender, RoutedEventArgs e) {
    try {
      Entity = new Room(
        new RoomId(
          RoomNumber.FromDbValue(int.Parse(TxtRoomNumber.Text)),
          new HotelId(int.Parse(TxtHotelId.Text))
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

