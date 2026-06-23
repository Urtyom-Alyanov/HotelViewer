using System.Windows;
using HotelViewer.Presentation.ViewModels;

namespace HotelViewer.Presentation.Windows;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {
  public MainWindow(MainViewModel viewModel) {
    InitializeComponent();
    DataContext = viewModel;
  }
}
