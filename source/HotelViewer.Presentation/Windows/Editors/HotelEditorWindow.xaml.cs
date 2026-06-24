using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Value;
using HotelViewer.Presentation.Windows.Editors;

namespace HotelViewer.Presentation.Windows.Editors;

public partial class HotelEditorWindow : Window, IEntityEditor<Hotel> {
  private bool _isEditMode = false;

  public HotelEditorWindow() {
    InitializeComponent();
  }

  public Hotel? Entity { get; private set; }

  public void SetEntity(Hotel entity) {
    _isEditMode = true;

    TxtId.Text = entity.Id.Value.ToString();
    TxtId.IsEnabled = false;

    TxtName.Text = entity.Name;
    TxtPhone.Text = entity.Number.Value;
    TxtAddress.Text = entity.Address.Value;
    CmbOrganization.SelectedItem = entity.OrganizationId;
  }

  private void TxtPhone_PreviewTextInput(object sender, TextCompositionEventArgs e) {
    if (!char.IsDigit(e.Text, 0)) {
      e.Handled = true;
      return;
    }

    var tb = (TextBox)sender;
    string text = tb.Text;

    if (text.Length == 0) tb.Text = "+7 (";
    if (text.Length == 7) tb.Text += ") ";
    if (text.Length == 12) tb.Text += "-";
    if (text.Length == 15) tb.Text += "-";

    tb.CaretIndex = tb.Text.Length;
  }

  private void BtnSave_Click(object sender, RoutedEventArgs e) {
    try {
      var selectedOrgId = CmbOrganization.SelectedValue as OrganizationId;
      if (selectedOrgId == null) throw new Exception("Выберите организацию!");

      if (string.IsNullOrWhiteSpace(TxtName.Text))
        throw new Exception("Название не может быть пустым");

      Entity = new Hotel(
        new HotelId(int.Parse(TxtId.Text == "авто" ? "0" : TxtId.Text)),
        TxtName.Text,
        new PhoneNumber(TxtPhone.Text),
        new Address(TxtAddress.Text),
        selectedOrgId
      );
      DialogResult = true;
    }
    catch (Exception ex) {
      MessageBox.Show("Ошибка валидации: " + ex.Message);
    }
  }
}

