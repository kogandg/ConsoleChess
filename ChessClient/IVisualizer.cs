using System;
using System.Collections.Generic;
using System.Text;
using ConsoleChess;

namespace ChessClient
{
    public interface IVisualizer
    {
        void DrawBoard(ChessBoard chessBoard, Point current, bool showingMoves, Point currentMove, List<Point> moves);
        void DrawBackgound();
    }
}
