using Caro.CaroGame;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using Wpf.Ui.Controls;

namespace Caro
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : FluentWindow, INotifyPropertyChanged
    {
        private const int _spacing = 20;
        private const int ConsecutiveCellsToWin = 5;
        private Grid? _boardTemplate;

        private const int MinCellSize = 30;
        private const int MaxCellSize = 40;
        private const Player FirstPlayer = Player.X;
        private int _maxBoardSize;
        private int _minBoardSize;
        private int _boardDimension = 13;
        private Cell? _highlightedCell = null;

        private readonly MediaPlayer _soundPlayer = new MediaPlayer();
        private readonly Uri _selectSoundUri = new("pack://siteoforigin:,,,/Assets/bong.ogg");
        private readonly Uri _startSoundUri = new("pack://siteoforigin:,,,/Assets/start.ogg");
        private readonly Uri _endSoundUri = new("pack://siteoforigin:,,,/Assets/end.mp3");


        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public ObservableCollection<Cell> BoardData { get; set; } = [];
        public Player CurrentPlayer { get; set; }
        public Player Winner { get; set; }
        public bool GameOver { get; set; } = false;
        public bool GameStarted { get; set; } = false;
        public bool GameSavable => GameStarted && !GameOver;

        private void CreateGame(int size, GameSave? save = null)
        {
            var itemsPresenter = Utils.GetVisualChild<ItemsPresenter>(Board)!;
            var grid = Utils.GetVisualChild<Grid>(itemsPresenter)!;

            _boardDimension = size;
            _maxBoardSize = size * MaxCellSize;
            _minBoardSize = size * MinCellSize;
            this.MinHeight = _minBoardSize + Other.ActualHeight + TitleBar.ActualHeight + _spacing * 4;
            this.MinWidth = _minBoardSize + _spacing * 4;
            //ResizeBoard();

            BoardData.Clear();
            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();
            Winner = Player.None;

            if (save is null)
            {
                CurrentPlayer = FirstPlayer;
            }
            else
            {
                CurrentPlayer = save.CurrentPlayer;
            }


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
                    Position pos =
                        (i == 0 ? Position.Top : Position.Other)
                        | (j == 0 ? Position.Left : Position.Other)
                        | (j == size - 1 ? Position.Right : Position.Other)
                        | (i == size - 1 ? Position.Bottom : Position.Other);

                    int idx = i * size + j;

                    if (save is null)
                    {
                        BoardData.Add(new Cell(pos, i, j));
                    }
                    else
                    {
                        BoardData.Add(new Cell(pos, i, j) { PlayedBy = save.Board[idx] });
                    }

                    Grid.SetColumn(grid.Children[idx], j);
                    Grid.SetRow(grid.Children[idx], i);
                }
            }
        }

        private void SwitchPlayer()
        {
            CurrentPlayer = CurrentPlayer == Player.X ? Player.O : Player.X;
        }

        private bool CheckGameOver(int row, int col)
        {


            if (CheckRow(row, col)
                || CheckColumn(row, col)
                || CheckLeftDiagonal(row, col)
                || CheckRightDiagonal(row, col))
            {
                Winner = CurrentPlayer;
                CurrentPlayer = Player.None;
                GameOver = true;

                _soundPlayer.Open(_endSoundUri);
                _soundPlayer.Play();

                System.Windows.MessageBox.Show($"{Winner} wins");

                return true;
            }

            if (BoardData.All(cell => cell.PlayedBy != Player.None))
            {
                Winner = Player.None;
                CurrentPlayer = Player.None;
                GameOver = true;

                _soundPlayer.Open(_endSoundUri);
                _soundPlayer.Play();

                System.Windows.MessageBox.Show("Draw");

                return true;
            }

            return false;
        }

        private bool CheckGameOver()
        {
            if (BoardData.All(cell => cell.PlayedBy != Player.None))
            {
                return true;
            }

            for (int i = 0; i < _boardDimension; i++)
            {
                for (int j = 0; j < _boardDimension; j++)
                {
                    if (CheckGameOver(i, j)) return true;
                }
            }

            return false;
        }

        private bool CheckRow(int row, int col)
        {
            Player player = BoardData[row * _boardDimension + col].PlayedBy;
            if (player == Player.None) return false;

            int count = 1;
            for (int i = col + 1; i <= col + ConsecutiveCellsToWin - 1; i++)
            {
                if (i >= _boardDimension) break;
                if (BoardData[row * _boardDimension + i].PlayedBy != player) break;

                count++;
            }
            for (int i = col - 1; i >= col - ConsecutiveCellsToWin + 1; i--)
            {
                if (i < 0) break;
                if (BoardData[row * _boardDimension + i].PlayedBy != player) break;

                count++;
            }
            Debug.WriteLine($"Row {row} - {count}");

            return count >= ConsecutiveCellsToWin;
        }

        private bool CheckColumn(int row, int col)
        {
            Player player = BoardData[row * _boardDimension + col].PlayedBy;
            if (player == Player.None) return false;

            int count = 1;
            for (int i = row + 1; i <= row + ConsecutiveCellsToWin - 1; i++)
            {
                if (i >= _boardDimension) break;
                if (BoardData[i * _boardDimension + col].PlayedBy != player) break;

                count++;
            }
            for (int i = row - 1; i >= row - ConsecutiveCellsToWin + 1; i--)
            {
                if (i < 0) break;
                if (BoardData[i * _boardDimension + col].PlayedBy != player) break;

                count++;
            }
            Debug.WriteLine($"Col {col} - {count}");


            return count >= ConsecutiveCellsToWin;
        }

        private bool CheckLeftDiagonal(int row, int col)
        {
            Player player = BoardData[row * _boardDimension + col].PlayedBy;
            if (player == Player.None) return false;

            int count = 1;
            for (int i = 1; i < ConsecutiveCellsToWin; i++)
            {
                int curRow = row + i, curCol = col + i;

                if (curRow >= _boardDimension || curCol >= _boardDimension) break;
                if (BoardData[curRow * _boardDimension + curCol].PlayedBy != player) break;

                count++;

            }

            for (int i = 1; i < ConsecutiveCellsToWin; i++)
            {
                int curRow = row - i, curCol = col - i;

                if (curRow < 0 || curCol < 0) break;
                if (BoardData[curRow * _boardDimension + curCol].PlayedBy != player) break;

                count++;
            }


            return count >= ConsecutiveCellsToWin;
        }

        private bool CheckRightDiagonal(int row, int col)
        {
            Player player = BoardData[row * _boardDimension + col].PlayedBy;
            if (player == Player.None) return false;

            int count = 1;
            for (int i = 1; i < ConsecutiveCellsToWin; i++)
            {
                int curRow = row - i, curCol = col + i;

                if (curRow < 0 || curCol >= _boardDimension) break;
                if (BoardData[curRow * _boardDimension + curCol].PlayedBy != player) break;

                count++;

            }

            for (int i = 1; i < ConsecutiveCellsToWin; i++)
            {
                int curRow = row + i, curCol = col - i;

                if (curRow >= _boardDimension || curCol < 0) break;
                if (BoardData[curRow * _boardDimension + curCol].PlayedBy != player) break;

                count++;
            }


            return count >= ConsecutiveCellsToWin;
        }

        private void ResizeBoard()
        {
            double length = Math.Min(MainContent.ActualHeight - Other.ActualHeight - 20, MainContent.ActualWidth);
            if (length < _minBoardSize) length = _minBoardSize;
            else if (length > _maxBoardSize) length = _maxBoardSize;

            Board.Width = Board.Height = (int)(length / _boardDimension) * _boardDimension;
        }

        private void SelectCell(int row, int col)
        {
            if (CurrentPlayer == Player.None) return; // Game ended

            var cell = BoardData[row * _boardDimension + col];

            // Select cell
            if (cell.PlayedBy != Player.None) return;
            cell.PlayedBy = CurrentPlayer;

            _soundPlayer.Open(_selectSoundUri);
            _soundPlayer.Play();

            // Switch highlighted cell
            SwitchHighlightedCell(row, col);

            // Check for game over
            if (CheckGameOver(cell.Row, cell.Col)) return;

            // Switch turn
            SwitchPlayer();
        }

        private void SwitchHighlightedCell(int row, int col)
        {
            if (_highlightedCell is not null)
            {
                _highlightedCell.IsHighlighted = false;
            }

            _highlightedCell = BoardData[row * _boardDimension + col];
            _highlightedCell.IsHighlighted = true;
        }

        private void SaveGame(string filePath)
        {
            var save = new GameSave()
            {
                CurrentPlayer = CurrentPlayer,
                BoardSize = _boardDimension,
                Board = BoardData.Select(cell => cell.PlayedBy).ToArray()
            };

            XmlSerializer serializer = new(typeof(GameSave));

            using var streamWriter = new StreamWriter(filePath);
            serializer.Serialize(streamWriter, save);
        }

        private GameSave SaveGame()
        {
            return new GameSave()
            {
                CurrentPlayer = CurrentPlayer,
                BoardSize = _boardDimension,
                Board = BoardData.Select(cell => cell.PlayedBy).ToArray()
            };
        }

        private void LoadGame(string filePath)
        {

            GameSave? save;
            try
            {
                var serializer = new XmlSerializer(typeof(GameSave));
                using var fileStream = new FileStream(filePath, FileMode.Open);
                save = (GameSave)serializer.Deserialize(fileStream)!;

                // validate
                if (save.BoardSize * save.BoardSize != save.Board.Length)
                    throw new Exception();

                var xCount = save.Board.Count(player => player == Player.X);
                var oCount = save.Board.Count(player => player == Player.O);
                if (Math.Abs(xCount - oCount) > 1)
                    throw new Exception();
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("Invalid save file");
                return;
            }


            var currentGame = GameStarted ? SaveGame() : null;

            Board.Visibility = Visibility.Hidden;
            CreateGame(save.BoardSize, save);

            if (CheckGameOver())
            {
                System.Windows.MessageBox.Show("Invalid save file");
                if (currentGame is not null)
                {
                    CreateGame(currentGame.BoardSize, currentGame);
                    Board.Visibility = Visibility.Visible;
                }
            }
            else
            {
                Board.Visibility = Visibility.Visible;
                StartGame();
            }
        }

        public void StartGame()
        {
            GameOver = false;
            GameStarted = true;
            ResizeBoard();
            _soundPlayer.Open(_startSoundUri);
            _soundPlayer.Play();
        }

        #region Event Handlers
        private void Window_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Board.ItemsSource = BoardData;
            //CreateBoard(_boardDimension);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e) => ResizeBoard();

        private void BoardTemplate_Loaded(object sender, RoutedEventArgs e)
        {
            _boardTemplate = sender as Grid;
        }

        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            var inputDialog = new BoardSizeInputWindow();
            if (inputDialog.ShowDialog() == true)
            {
                CreateGame(inputDialog.BoardSize);
                StartGame();
            }
        }

        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var cell = ((ListViewItem)sender).Content as Cell;
            if (cell is null) return;

            SelectCell(cell.Row, cell.Col);
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    if (_highlightedCell is null) break;
                    SwitchHighlightedCell(_highlightedCell.Row, Math.Max(0, _highlightedCell.Col - 1));

                    return;
                case Key.Right:
                    if (_highlightedCell is null) break;
                    else
                        SwitchHighlightedCell(_highlightedCell.Row, Math.Min(_boardDimension - 1, _highlightedCell.Col + 1));

                    return;
                case Key.Up:
                    if (_highlightedCell is null) break;
                    SwitchHighlightedCell(Math.Max(0, _highlightedCell.Row - 1), _highlightedCell.Col);

                    return;
                case Key.Down:
                    if (_highlightedCell is null) break;
                    SwitchHighlightedCell(Math.Min(_boardDimension - 1, _highlightedCell.Row + 1), _highlightedCell.Col);

                    return;
                case Key.Enter:
                    if (_highlightedCell is null) break;
                    SelectCell(_highlightedCell.Row, _highlightedCell.Col);

                    return;
                default:
                    return;
            }

            var selectedItem = Board.SelectedItem as Cell;
            if (selectedItem is not null)
            {
                SwitchHighlightedCell(selectedItem.Row, selectedItem.Col);
                return;
            }

            SwitchHighlightedCell(0, 0);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new()
            {
                Filter = "Game data (*.data)|*.data",
                DefaultExt = "data",
                AddExtension = true
            };

            if (saveDialog.ShowDialog() == true)
            {
                SaveGame(saveDialog.FileName);
            }
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new()
            {
                Filter = "Game data (*.data)|*.data",
                DefaultExt = "data",
                AddExtension = true
            };

            if (openDialog.ShowDialog() == true)
            {
                LoadGame(openDialog.FileName);
            }
        }

        #endregion Event Handlers
    }
}
