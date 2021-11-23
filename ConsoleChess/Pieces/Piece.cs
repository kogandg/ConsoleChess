using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleChess.Pieces
{
    public abstract class Piece
    {
        public char FENNotation { get; protected set; }
        public Color PieceColor { get; protected set; }
        public Point CurrentPosition;

        protected ChessBoard owningBoard { get; set; }
        public Piece(Color color, Point currentPosition, ChessBoard board)
        {
            CurrentPosition = currentPosition;
            PieceColor = color;
            owningBoard = board;
        }
        public abstract List<Point> PossibleMoves();

        public bool IsWhite() { return PieceColor == Color.White; }
    }

    public enum Color
    {
        None,
        White,
        Black
    }
}
