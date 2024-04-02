using System.ComponentModel;

namespace Caro.CaroGame
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

    public partial class Cell(Position pos, int row, int col) : INotifyPropertyChanged
    {
        public Position Pos { get; set; } = pos;

        public Player PlayedBy { get; set; } = Player.None;

        public int Row { get; set; } = row;

        public int Col { get; set; } = col;

        public bool IsHighlighted { get; set; } = false;
    }

    public class GameSave
    {
        public Player CurrentPlayer;
        public int BoardSize;
        public required Player[] Board;
    }
}
