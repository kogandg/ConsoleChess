using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleChess.Pieces
{
    class Rook : Piece
    {
        public Rook(Color color, Point currentPosition, ChessBoard board) : base(color, currentPosition, board)
        {
            if (IsWhite())
            {
                FENNotation = 'R';
            }
            else
            {
                FENNotation = 'r';
            }
        }
        public override List<Point> PossibleMoves()
        {
            List<Point> moves = new List<Point>();
            var directions = new[] { new Point(0, 1), new Point(0, -1), new Point(1, 0), new Point(-1, 0) };

            foreach (var direction in directions)
            {
                Point testPoint = CurrentPosition + direction;
                while (testPoint.IsInside() && owningBoard[testPoint] is EmptyPiece)
                {
                    moves.Add(testPoint);
                    testPoint += direction;
                }
                if (testPoint.IsInside() && owningBoard[testPoint].IsWhite() != IsWhite())
                {
                    moves.Add(testPoint);
                }

            }

            return moves;
        }
    }
}
