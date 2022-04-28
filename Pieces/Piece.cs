using System;
using System.Collections.Generic;
using System.Text;
using PointLibrary;
using Interfaces;

namespace Pieces
{
    public abstract class Piece : IPiece
    {
        public char FENNotation { get; protected set; }
        public PieceColors PieceColor { get; protected set; }
        public Point CurrentPosition;

        protected IChessBoard<Piece> owningBoard { get; set; }
        public Piece(PieceColors color, Point currentPosition, IChessBoard<Piece> board)
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

        public bool IsOnWhite()
        {
            int sum = CurrentPosition.X + CurrentPosition.Y;
            if (sum % 2 == 0)
            {
                return false;
            }
            return true;
        }

        public bool TestMove(Point move)
        {
            Pawn enPassantPawn = new Pawn(PieceColors.None, new Point(-1, -1), owningBoard);
            if (this is King)
            {
                if (Math.Sqrt(Math.Pow(CurrentPosition.X - move.X, 2) + Math.Pow(CurrentPosition.Y - move.Y, 2)) > 1.5)
                {
                    if (move.X == 2)
                    {
                        return TestMove(new Point(3, move.Y));
                    }
                    if (move.X == 6)
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

            if (move == owningBoard.EnPassantTargetSquare)
            {
                if (owningBoard[move].IsWhite())
                {
                    enPassantPawn = (Pawn)owningBoard.GridSquares[move.Y - 1, move.X];
                    owningBoard.GridSquares[move.Y - 1, move.X] = new EmptyPiece(new Point(move.Y - 1, move.X), owningBoard);
                }
                else
                {
                    enPassantPawn = (Pawn)owningBoard.GridSquares[move.Y + 1, move.X];
                    owningBoard.GridSquares[move.Y + 1, move.X] = new EmptyPiece(new Point(move.Y + 1, move.X), owningBoard);
                }
                //owningBoard.DrawBoard();
            }

            bool inCheck = owningBoard.InCheck(this);

            owningBoard.GridSquares[originalPosition.Y, originalPosition.X] = this;
            owningBoard.GridSquares[CurrentPosition.Y, CurrentPosition.X] = replace;
            CurrentPosition = originalPosition;

            if (move == owningBoard.EnPassantTargetSquare)
            {
                if (owningBoard[move].IsWhite())
                {
                    owningBoard.GridSquares[move.Y + 1, move.X] = enPassantPawn;
                }
                else
                {
                    owningBoard.GridSquares[move.Y - 1, move.X] = enPassantPawn;
                }
            }

            return inCheck;
        }

        //public abstract List<Point> PosibleMoves();
    }
}

namespace Pieces
{
    public enum PieceColors
    {
        None,
        White,
        Black
    }
}
