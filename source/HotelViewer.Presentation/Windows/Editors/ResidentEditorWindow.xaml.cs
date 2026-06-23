using System.Windows;
using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Value;

namespace HotelViewer.Presentation.Windows.Editors;

public partial class ResidentEditorWindow : Window, IEntityEditor<Resident> {
  public IEnumerable<Sex> AllSexes => Enum.GetValues(typeof(Sex)).Cast<Sex>();
  public Sex SelectedSex { get; private set; } = Sex.Male;

  public ResidentEditorWindow() {
    InitializeComponent();
  }

  public Resident? Entity { get; private set; }

  public void SetEntity(Resident entity) {
    TxtId.Text = entity.Id.Value.ToString();
    TxtId.IsEnabled = false;

    TxtName.Text = entity.Name.ToDbValue();
    TxtAddress.Text = entity.Address.Value;
    SelectedSex = entity.Sex;
    TxtPhone.Text = entity.PhoneNumber.Value;
  }

  private void BtnSave_Click(object sender, RoutedEventArgs e) {
    try {
      Entity = new Resident(
        new ResidentId(int.Parse(TxtId.Text)),
        FullName.FromDbValue(TxtName.Text),
        new Address(TxtAddress.Text),
        SelectedSex,
        new PhoneNumber(TxtPhone.Text)
      );
      DialogResult = true;
    }
    catch (Exception ex) {
      MessageBox.Show("Ошибка валидации: " + ex.Message);
    }
  }
}

