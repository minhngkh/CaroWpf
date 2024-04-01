using System.Windows;

namespace Caro
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        /// <summary>
        /// Occurs when the application is loading.
        /// </summary>
        private void OnStartup(object sender, StartupEventArgs e)
        {
            MainWindow mainWindow = new();
            mainWindow.Show();
        }
    }
}
