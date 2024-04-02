using System.Windows;
using System.Windows.Media;

namespace Caro
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public MediaPlayer BgMusicPlayer = new();
        private string _bgMusicPath = "pack://siteoforigin:,,,/Assets/nggyu.mp3";

        /// <summary>
        /// Occurs when the application is loading.
        /// </summary>
        private void OnStartup(object sender, StartupEventArgs e)
        {
            BgMusicPlayer.Open(new Uri(_bgMusicPath));
            BgMusicPlayer.Volume = 0.1;
            BgMusicPlayer.MediaOpened += (o, args) =>
            {
                BgMusicPlayer.Play();
            };
            BgMusicPlayer.MediaEnded += (o, args) =>
            {
                BgMusicPlayer.Position = TimeSpan.Zero;
                BgMusicPlayer.Play();
            };

            MainWindow mainWindow = new();
            mainWindow.Show();
        }
    }
}
