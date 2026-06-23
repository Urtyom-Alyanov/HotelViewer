using System.Windows;
using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Value;

namespace HotelViewer.Presentation.Windows.Editors;

public partial class ResidenceEditorWindow : Window, IEntityEditor<Residence> {
  public ResidenceEditorWindow() {
    InitializeComponent();
  }

  public Residence? Entity { get; private set; }

  public void SetEntity(Residence entity) {
    TxtId.Text = entity.ResidenceId.Value.ToString();
    TxtId.IsEnabled = false;

    TxtRoomNumber.Text = entity.Number.ToDbValue().ToString();
    TxtHotelId.Text = entity.HotelId.Value.ToString();
    TxtResidentId.Text = entity.ResidentId.Value.ToString();
    TxtDaysPerNight.Text = entity.DaysPerNight.ToString();
    TxtDaysPerNight.Text = entity.ResidenceAt.ToString();
  }

  private void BtnSave_Click(object sender, RoutedEventArgs e) {
    try {
      Entity = new Residence(
        new ResidenceId(int.Parse(TxtId.Text)),
        RoomNumber.FromDbValue(int.Parse(TxtRoomNumber.Text)),
        new HotelId(int.Parse(TxtHotelId.Text)),
        new ResidentId(int.Parse(TxtResidentId.Text)),
        uint.Parse(TxtDaysPerNight.Text),
        DateTime.Parse(TxtDateTime.Text)
      );
      DialogResult = true;
    }
    catch (Exception ex) {
      MessageBox.Show("Ошибка валидации: " + ex.Message);
    }
  }
}

