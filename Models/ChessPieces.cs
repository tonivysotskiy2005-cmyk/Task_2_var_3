using System;

namespace ChessApp.Models
{
    /// <summary>
    /// Представляет позицию на шахматной доске (столбец A-H, строка 1-8).
    /// </summary>
    public struct BoardPosition
    {
        public int Column { get; set; }
        public int Row { get; set; }

        public BoardPosition(int column, int row)
        {
            Column = column;
            Row = row;
        }

        public bool IsValid => Column >= 1 && Column <= 8 && Row >= 1 && Row <= 8;

        public override string ToString()
        {
            char colChar = (char)('A' + Column - 1);
            return $"{colChar}{Row}";
        }

        public static bool TryParse(string text, out BoardPosition position)
        {
            position = default;
            if (string.IsNullOrWhiteSpace(text) || text.Length != 2)
                return false;

            char colChar = char.ToUpper(text[0]);
            if (colChar < 'A' || colChar > 'H')
                return false;

            if (!char.IsDigit(text[1]))
                return false;

            int row = text[1] - '0';
            if (row < 1 || row > 8)
                return false;

            position = new BoardPosition(colChar - 'A' + 1, row);
            return true;
        }
    }

    /// <summary>
    /// Цвет шахматной фигуры.
    /// </summary>
    public enum PieceColor
    {
        White,
        Black
    }

    /// <summary>
    /// Базовый абстрактный класс — шахматная фигура.
    /// </summary>
    public abstract class ChessPiece
    {
        public PieceColor Color { get; }
        public BoardPosition Position { get; private set; }
        public abstract string Name { get; }

        protected ChessPiece(PieceColor color, BoardPosition startPosition)
        {
            Color = color;
            Position = startPosition;
        }

        /// <summary>
        /// Пытается сделать ход в указанную позицию.
        /// Возвращает true, если ход совершён.
        /// </summary>
        public bool TryMove(BoardPosition target)
        {
            if (!target.IsValid)
                return false;

            if (target.Column == Position.Column && target.Row == Position.Row)
                return false;

            if (!CanMoveTo(target))
                return false;

            Position = target;
            return true;
        }

        /// <summary>
        /// Абстрактный защищённый метод — проверяет допустимость хода
        /// по правилам конкретной фигуры. Перекрывается в производных классах.
        /// </summary>
        protected abstract bool CanMoveTo(BoardPosition target);

        public override string ToString() =>
            $"{Name} ({Color}), позиция: {Position}";
    }

    /// <summary>
    /// Ладья — ходит по горизонтали или вертикали.
    /// </summary>
    public class Rook : ChessPiece
    {
        public override string Name => "Ладья";

        public Rook(PieceColor color, BoardPosition startPosition)
            : base(color, startPosition) { }

        protected override bool CanMoveTo(BoardPosition target)
        {
            return target.Column == Position.Column || target.Row == Position.Row;
        }
    }

    /// <summary>
    /// Слон — ходит по диагонали.
    /// </summary>
    public class Bishop : ChessPiece
    {
        public override string Name => "Слон";

        public Bishop(PieceColor color, BoardPosition startPosition)
            : base(color, startPosition) { }

        protected override bool CanMoveTo(BoardPosition target)
        {
            int deltaCol = Math.Abs(target.Column - Position.Column);
            int deltaRow = Math.Abs(target.Row - Position.Row);
            return deltaCol == deltaRow;
        }
    }

    /// <summary>
    /// Ферзь — ходит по горизонтали, вертикали и диагонали.
    /// </summary>
    public class Queen : ChessPiece
    {
        public override string Name => "Ферзь";

        public Queen(PieceColor color, BoardPosition startPosition)
            : base(color, startPosition) { }

        protected override bool CanMoveTo(BoardPosition target)
        {
            bool straight = target.Column == Position.Column || target.Row == Position.Row;
            int deltaCol = Math.Abs(target.Column - Position.Column);
            int deltaRow = Math.Abs(target.Row - Position.Row);
            bool diagonal = deltaCol == deltaRow;
            return straight || diagonal;
        }
    }
}
