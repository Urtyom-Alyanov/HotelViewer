using System.Windows;
using HotelViewer.Presentation.ViewModels;

namespace HotelViewer.Presentation;

public partial class LoginWindow : Window {
  public LoginWindow(LoginViewModel viewModel) {
    InitializeComponent();
    DataContext = viewModel;
  }
}

