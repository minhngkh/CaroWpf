using Caro.CaroGame;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Caro.Helpers
{
    [ValueConversion(typeof(Player), typeof(Color))]
    public class PlayerToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null || value is not Player player)
            {
                throw new ArgumentException("Value must be of type Player");
            }

            return player switch
            {
                Player.X => new SolidColorBrush(Colors.LightBlue) { },
                Player.O => new SolidColorBrush(Colors.LightPink) { },
                _ => new SolidColorBrush(Colors.Transparent),
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
