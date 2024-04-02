using System.ComponentModel;
using System.Windows;
using Wpf.Ui.Controls;

namespace Caro
{
    /// <summary>
    /// Interaction logic for BoardSizeInputWindow.xaml
    /// </summary>
    public partial class BoardSizeInputWindow : FluentWindow, INotifyPropertyChanged
    {
        public BoardSizeInputWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public int BoardSize { get => BoardSizeInput.Value.HasValue ? (int)BoardSizeInput.Value : 10; }

        private void Window_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            (sender as BoardSizeInputWindow)!.SizeToContent = SizeToContent.WidthAndHeight;
            BoardSizeInput.Focus();
        }

        private void OK_Click(object sender, RoutedEventArgs e) => DialogResult = true;

        private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}
