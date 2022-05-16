using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class MakeSelectionModel
    {
        public ChessSquareModel ChessSquare { get; set; }
        public Guid PlayerID { get; set; }

        public MakeSelectionModel(Guid playerId, ChessSquareModel chessSquare)
        {
            PlayerID = playerId;
            ChessSquare = chessSquare;
        }
    }
}
