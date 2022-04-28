using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PointLibrary;

namespace ChessBoardInterface
{
    public interface IChessBoard
    {
        public bool InCheck();
    }
}
