using System;
using System.Collections.Generic;
using System.Text;
using PointLibrary;
using Interfaces;

namespace Pieces
{
    public class Knight : Piece
    {
        public Knight(PieceColors color, Point currentPosition, IChessBoard<Piece> board) : base(color, currentPosition, board)
        {
            if (IsWhite())
            {
                FENNotation = 'N';
            }
            else
            {
                FENNotation = 'n';
            }
        }
        public override List<Point> PossibleMoves()
        {
            List<Point> moves = new List<Point>();
            
            for(int i = -2; i < 3; i+=4)
            {
                for (int j = -1; j < 2; j+=2)
                {
                    Point currentHorizonal = new Point(CurrentPosition.X + i, CurrentPosition.Y + j);
                    Point currentVerical = new Point(CurrentPosition.X + j, CurrentPosition.Y + i);
                    if (currentHorizonal.IsInside() && (owningBoard[currentHorizonal].IsWhite() != IsWhite() || owningBoard[currentHorizonal] is EmptyPiece))
                    {
                        moves.Add(currentHorizonal);
                    }
                    if (currentVerical.IsInside() && (owningBoard[currentVerical].IsWhite() != IsWhite() || owningBoard[currentVerical] is EmptyPiece))
                    {
                        moves.Add(currentVerical);
                    }
                }
            }

            return moves;
        }
    }
}
