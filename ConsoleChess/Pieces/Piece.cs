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
                if (TestMove(moves[i]))
                {
                    moves.RemoveAt(i);
                    i--;
                }
            }
            return moves;
        }

        public bool IsWhite() { return PieceColor == PieceColors.White; }

        public bool TestMove(Point move)
        {
            if (this is King)
            {
                if (Math.Sqrt(Math.Pow(CurrentPosition.X - move.X, 2) + Math.Pow(CurrentPosition.Y - move.Y, 2)) > 1.5)
                {
                    if(move.X == 2)
                    {
                        return TestMove(new Point(3, move.Y));
                    }
                    if(move.X == 6)
                    {
                        return TestMove(new Point(5, move.Y));
                    }
                }
            }
            Piece replace = owningBoard[move];
            Point originalPosition = CurrentPosition;
            CurrentPosition = move;
            owningBoard.GridSquares[CurrentPosition.Y, CurrentPosition.X] = this;
            owningBoard.GridSquares[originalPosition.Y, originalPosition.X] = new EmptyPiece(originalPosition, owningBoard);

            bool inCheck = owningBoard.InCheck(this);

            owningBoard.GridSquares[originalPosition.Y, originalPosition.X] = this;
            owningBoard.GridSquares[CurrentPosition.Y, CurrentPosition.X] = replace;
            CurrentPosition = originalPosition;
            return inCheck;
        }
    }

    public enum PieceColors
    {
        None,
        White,
        Black
    }
}
