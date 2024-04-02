using Caro.CaroGame;
using System.Windows;
using System.Windows.Controls;

namespace Caro.Helpers
{
    public class BoardCellDataTemplateSelector : DataTemplateSelector
    {
        public required DataTemplate EmptyCellTemplate { get; set; }
        public required DataTemplate XCellTemplate { get; set; }
        public required DataTemplate OCellTemplate { get; set; }

        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if (item != null && item is Cell cell)
            {
                switch (cell.PlayedBy)
                {
                    case Player.None:
                        return EmptyCellTemplate;
                    case Player.X:
                        return XCellTemplate;
                    case Player.O:
                        return OCellTemplate;
                }
            }

            return null;
        }
    }
}
