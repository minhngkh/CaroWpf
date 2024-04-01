using System.Windows;
using System.Windows.Media;

namespace Caro
{
    public class Utils
    {
        public static T? GetVisualChild<T>(DependencyObject parent) where T : Visual
        {
            T? child = default;

            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                child ??= GetVisualChild<T>(v);

                if (child != null)
                {
                    break;
                }
            }
            return child;
        }
    }
}
