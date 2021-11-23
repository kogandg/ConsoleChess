using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleChess.Pieces
{
    class Queen : Piece
    {
        public Queen(Color color, Point currentPosition, ChessBoard board) : base(color, currentPosition, board)
        {
            if (IsWhite())
            {
                FENNotation = 'Q';
            }
            else
            {
                FENNotation = 'q';
            }
        }
        public override List<Point> PossibleMoves()
        {
            List<Point> moves = new List<Point>();
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    if (x == 0 && y == 0) continue;

                    Point add = new Point(x, y);
                    Point currentPoint = CurrentPosition + add;
                    while (currentPoint.IsInside() && owningBoard[currentPoint] is EmptyPiece)
                    {
                        moves.Add(currentPoint);
                        currentPoint += add;
                    }
                    if (currentPoint.IsInside() && owningBoard[currentPoint].IsWhite() != IsWhite())
                    {
                        moves.Add(currentPoint);
                    }
                }
            }

            return moves;
        }
    }
}
