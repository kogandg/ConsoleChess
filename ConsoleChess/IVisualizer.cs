using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleChess
{
    public interface IVisualizer
    {
        void DrawBoard(ChessBoard chessBoard);
        void DrawBackgound();
    }
}
