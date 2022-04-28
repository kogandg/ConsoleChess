using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PointLibrary;

namespace Interfaces
{
    public interface IChessBoard<T> where T : class, IPiece
    {
        T this[Point p] { get; }
        T this[int y, int x] { get; }
        T[,] GridSquares { get; }

        Point EnPassantTargetSquare { get; }

        bool KCastle { get; }
        bool QCastle { get; }
        bool kCastle { get; }
        bool qCastle { get; }

        bool InCheck(T piece);
    }
}
