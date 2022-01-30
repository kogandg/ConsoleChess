using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleChess.Pieces
{
    class EmptyPiece : Piece
    {
        public EmptyPiece(Point currentPosition, ChessBoard board):base(PieceColors.None, currentPosition, board)
        {
            FENNotation = '.';
        }
        public override List<Point> PossibleMoves()
        {
            return new List<Point>();
        }
    }
}
