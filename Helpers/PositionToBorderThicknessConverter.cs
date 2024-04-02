using Caro.CaroGame;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Caro.Helpers
{
    [ValueConversion(typeof(Position), typeof(Thickness))]
    public class PositionToBorderThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null || value is not Position pos)
            {
                return new Thickness(0);
            }

            return new Thickness(
                pos.HasFlag(Position.Left) ? 2 : 1,
                pos.HasFlag(Position.Top) ? 2 : 1,
                pos.HasFlag(Position.Right) ? 2 : 1,
                pos.HasFlag(Position.Bottom) ? 2 : 1
            );
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
