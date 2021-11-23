using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleChess.Pieces
{
    class King : Piece
    {
        public King(Color color, Point currentPosition, ChessBoard board) : base(color, currentPosition, board)
        {
            if (IsWhite())
            {
                FENNotation = 'K';
            }
            else
            {
                FENNotation = 'k';
            }
        }
        public override List<Point> PossibleMoves()
        {
            List<Point> moves = new List<Point>();
            for(int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    Point possiblePoint = CurrentPosition + new Point(x, y);
                    if(possiblePoint.IsInside() && possiblePoint != CurrentPosition && (owningBoard[possiblePoint].IsWhite() != IsWhite() || owningBoard[possiblePoint] is EmptyPiece))
                    {
                        moves.Add(possiblePoint);
                    }
                }
            }
            return moves;
        }
    }
}
