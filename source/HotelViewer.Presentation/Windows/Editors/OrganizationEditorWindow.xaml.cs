using System.Windows;
using HotelViewer.Domain.Entity;

namespace HotelViewer.Presentation.Windows.Editors;

public partial class OrganizationEditorWindow : Window, IEntityEditor<Organization> {
  private bool _isEditMode = false;

  public OrganizationEditorWindow() {
    InitializeComponent();
  }

  public Organization? Entity { get; private set; }

  public void SetEntity(Organization entity) {
    _isEditMode = true;

    TxtId.Text = entity.Id.Value.ToString();
    TxtId.IsEnabled = false;

    TxtName.Text = entity.Name;

    DataContext = null;
    DataContext = this;
  }

  private void BtnSave_Click(object sender, RoutedEventArgs e) {
    try {
      if (string.IsNullOrWhiteSpace(TxtName.Text))
        throw new Exception("Название не может быть пустым");

      Entity = new Organization(
        new OrganizationId(int.Parse(TxtId.Text == "авто" ? "0" : TxtId.Text)),
        TxtName.Text
      );

      DialogResult = true;
    }
    catch (Exception ex) {
      MessageBox.Show("Ошибка валидации: " + ex.Message);
    }
  }
}

