using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace Caro
{
    [Flags]
    public enum Position
    {
        Other = 0,
        Top = 1 << 0,
        Left = 1 << 1,
        Right = 1 << 2,
        Bottom = 1 << 3
    }
    public enum Player
    {
        None,
        X,
        O
    }

    public partial class Cell(Position pos) : ObservableObject
    {
        [ObservableProperty]
        private Position _pos;

        [ObservableProperty]
        private Player _playedBy = Player.None;
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : FluentWindow
    {
        private const int _spacing = 20;
        private Grid? _boardTemplate;

        private const int MinCellSize = 30;
        private const int MaxCellSize = 40;
        private int _maxBoardSize;
        private int _minBoardSize;
        private int _boardDimension = 13;
        private Player _currentPlayer = Player.X;

        public ObservableCollection<Cell> BoardData { get; set; } = [];

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void MainWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Board.ItemsSource = BoardData;
            CreateBoard(_boardDimension);


            this.MinHeight = _minBoardSize + Test.ActualHeight + TitleBar.ActualHeight + _spacing * 4;
        }

        private void CreateBoard(int size)
        {
            var itemsPresenter = Utils.GetVisualChild<ItemsPresenter>(Board)!;
            var grid = Utils.GetVisualChild<Grid>(itemsPresenter)!;

            _maxBoardSize = size * MaxCellSize;
            _minBoardSize = size * MinCellSize;

            for (int i = 0; i < size; i++)
            {
                grid.RowDefinitions.Add(
                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            }


            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    var BorderThickness = new Thickness()
                    {
                        Top = i == 0 ? 1 : 0,
                        Left = j == 0 ? 1 : 0,
                        Right = j == size - 1 ? 1 : 0,
                        Bottom = i == size - 1 ? 1 : 0
                    };
                    Position pos =
                        (i == 0 ? Position.Top : Position.Other)
                        | (j == 0 ? Position.Left : Position.Other)
                        | (j == size - 1 ? Position.Right : Position.Other)
                        | (i == size - 1 ? Position.Bottom : Position.Other);

                    BoardData.Add(new Cell(pos));
                    Grid.SetColumn(grid.Children[i * size + j], j);
                    Grid.SetRow(grid.Children[i * size + j], i);
                }
            }
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double length = Math.Min(Content.ActualHeight - Test.ActualHeight - 20, Content.ActualWidth);
            if (length < _minBoardSize) length = _minBoardSize;
            else if (length > _maxBoardSize) length = _maxBoardSize;

            Board.Width = Board.Height = (int)(length / _boardDimension) * _boardDimension;
        }

        private void Board_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ignore the trigger by the the our reset;
            if (Board.SelectedIndex == -1) return;
            Debug.WriteLine(Board.SelectedIndex);

            var cell = BoardData[Board.SelectedIndex];
            if (cell.PlayedBy != Player.None) return;
            //BoardData[Board.SelectedIndex] = new Cell(cell.Pos) { PlayedBy = _currentPlayer };
            ((Cell)Board.SelectedItem).PlayedBy = _currentPlayer;

            //((ViewModel)DataContext).List[0].PlayedBy = _currentPlayer;

            // reset the selection
            Board.SelectedIndex = -1;
        }

        private void BoardTemplate_Loaded(object sender, RoutedEventArgs e)
        {
            _boardTemplate = sender as Grid;
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            BoardData[0].PlayedBy = _currentPlayer;

        }
    }
}
