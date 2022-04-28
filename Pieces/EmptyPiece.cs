using System;
using System.Collections.Generic;
using System.Text;
using Interfaces;
using PointLibrary;

namespace Pieces
{
    public class EmptyPiece : Piece
    {
        public EmptyPiece(Point currentPosition, IChessBoard<Piece> board):base(PieceColors.None, currentPosition, board)
        {
            FENNotation = '.';
        }
        public override List<Point> PossibleMoves()
        {
            return new List<Point>();
        }
    }
}
