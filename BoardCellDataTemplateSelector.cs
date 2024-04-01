using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;

namespace Caro
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
                var thickness =
                    (cell.Pos.HasFlag(Position.Left) ? 2 : 1) + ","
                    + (cell.Pos.HasFlag(Position.Top) ? 2 : 1) + ","
                    + (cell.Pos.HasFlag(Position.Right) ? 2 : 1) + ","
                    + (cell.Pos.HasFlag(Position.Bottom) ? 2 : 1);

                switch (cell.PlayedBy)
                {
                    case Player.None:
                        {
                            var templateStr =
                                $@"<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" 
                                                 xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
                                    <Border BorderBrush=""Gray"" BorderThickness=""{thickness}"">
                                    </Border>
                                </DataTemplate>";
                            var stringReader = new StringReader(templateStr);
                            var xmlReader = XmlReader.Create(stringReader);
                            DataTemplate template = (DataTemplate)XamlReader.Load(xmlReader);

                            return template;
                        }
                    case Player.X:
                        {
                            var templateStr =
                                $@"<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" 
                                                 xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
                                                 DataType=""Cell"">
                                    <Border BorderBrush=""Gray"" BorderThickness=""{thickness}"">
                                        <Canvas Name=""parent"" Margin=""10"">
                                            <Line  X1=""0"" Y1=""0"" X2=""{{Binding ElementName='parent', Path='ActualWidth'}}"" Y2=""{{Binding ElementName='parent', Path='ActualHeight'}}"" 
                                                   Stroke=""Blue"" StrokeThickness=""2"" />
                                            <Line  X1=""0"" Y1=""{{Binding ElementName='parent', Path='ActualHeight'}}"" X2=""{{Binding ElementName='parent', Path='ActualWidth'}}"" Y2=""0"" 
                                                   Stroke=""Blue"" StrokeThickness=""2"" />
                                        </Canvas>
                                    </Border>
                                </DataTemplate>";
                            var stringReader = new StringReader(templateStr);
                            var xmlReader = XmlReader.Create(stringReader);
                            DataTemplate template = (DataTemplate)XamlReader.Load(xmlReader);
                            return template;
                        }
                    case Player.O:
                        {
                            var templateStr =
                                $@"<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" 
                                                 xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
                                    <Border BorderBrush=""Gray"" BorderThickness=""{thickness}"">
                                        <Canvas Name=""parent"" Margin=""10"">
                                            <Ellipse Width=""{{Binding ElementName='parent', Path='ActualWidth'}}"" Height=""{{Binding ElementName='parent', Path='ActualHeight'}}"" 
                                                     Stroke=""Red"" StrokeThickness=""2"" />}}
                                        </Canvas>
                                    </Border>
                                </DataTemplate>";
                            var stringReader = new StringReader(templateStr);
                            var xmlReader = XmlReader.Create(stringReader);
                            DataTemplate template = (DataTemplate)XamlReader.Load(xmlReader);
                            return template;
                        }
                }
            }

            return null;
        }
    }
}
