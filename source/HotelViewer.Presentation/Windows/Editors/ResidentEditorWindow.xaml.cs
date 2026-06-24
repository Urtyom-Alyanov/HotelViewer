using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Value;

namespace HotelViewer.Presentation.Windows.Editors;

public partial class ResidentEditorWindow : Window, IEntityEditor<Resident> {
  public IEnumerable<Sex> AllSexes => Enum.GetValues(typeof(Sex)).Cast<Sex>();
  public Sex SelectedSex { get; set; } = Sex.Male;

  public ResidentEditorWindow() {
    InitializeComponent();
    DataContext = this;
  }

  public Resident? Entity { get; private set; }

  public void SetEntity(Resident entity) {
    TxtId.Text = entity.Id.Value.ToString();
    TxtId.IsEnabled = false;

    TxtFirstName.Text = entity.Name.FirstName;
    TxtLastName.Text = entity.Name.LastName;
    TxtMiddleName.Text = entity.Name.MiddleName;

    TxtAddress.Text = entity.Address.Value;
    SelectedSex = entity.Sex;
    TxtPhone.Text = entity.PhoneNumber.Value;
  }

  private void BtnSave_Click(object sender, RoutedEventArgs e) {
    try {
      Entity = new Resident(
        new ResidentId(int.Parse(TxtId.Text)),
        new FullName(
          TxtLastName.Text,
          TxtFirstName.Text,
          TxtMiddleName.Text
          ),
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
}

