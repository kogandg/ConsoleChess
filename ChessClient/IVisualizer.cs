using System;
using System.Collections.Generic;
using System.Text;
using ChessLibrary;
using PointLibrary;

namespace ChessClient
{
    public interface IVisualizer
    {
        void DrawBoard(ChessBoard chessBoard, Point current, Point currentMove, List<Point> moves);
        void DrawBackgound();
    }
}
