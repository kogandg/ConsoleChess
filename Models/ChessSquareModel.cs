namespace Models
{
    public class ChessSquareModel
    {
        public int Row { get; set;  }
        public int Col { get; set;  }

        public ChessSquareModel()
        {

        }
        public ChessSquareModel(int row, int col)
        {
            Row = row;
            Col = col;
        }

        
    }
}
