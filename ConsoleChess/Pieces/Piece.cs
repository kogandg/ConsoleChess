using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleChess.Pieces
{
    public abstract class Piece
    {
        public char FENNotation { get; protected set; }
        public PieceColors PieceColor { get; protected set; }
        public Point CurrentPosition;

        protected ChessBoard owningBoard { get; set; }
        public Piece(PieceColors color, Point currentPosition, ChessBoard board)
        {
            CurrentPosition = currentPosition;
            PieceColor = color;
            owningBoard = board;
        }
        public abstract List<Point> PossibleMoves();
        public List<Point> AllowedMoves()
        {
            var moves = PossibleMoves();
            for (int i = 0; i < moves.Count; i++)
            {
                var tempMove = moves[i];
                var originalPosition = CurrentPosition;
                owningBoard.MovePiece(CurrentPosition, tempMove);
                if (owningBoard.InCheck(this))
                {
                    moves.RemoveAt(i);
                    i--;
                }
                owningBoard.MovePiece(tempMove, originalPosition);
            }
            return moves;
        }

        public bool IsWhite() { return PieceColor == PieceColors.White; }
    }

    public enum PieceColors
    {
        None,
        White,
        Black
    }
}
