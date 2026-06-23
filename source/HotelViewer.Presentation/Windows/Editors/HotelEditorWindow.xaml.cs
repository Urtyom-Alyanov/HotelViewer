using System.Windows;
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
    TxtOrgId.Text = entity.OrganizationId.Value.ToString();
  }

  private void BtnSave_Click(object sender, RoutedEventArgs e) {
    try {
      if (string.IsNullOrWhiteSpace(TxtName.Text))
        throw new Exception("Название не может быть пустым");

      Entity = new Hotel(
        new HotelId(int.Parse(TxtId.Text)),
        TxtName.Text,
        new PhoneNumber(TxtPhone.Text),
        new Address(TxtAddress.Text),
        new OrganizationId(int.Parse(TxtOrgId.Text))
      );
      DialogResult = true;
    }
    catch (Exception ex) {
      MessageBox.Show("Ошибка валидации: " + ex.Message);
    }
  }
}

