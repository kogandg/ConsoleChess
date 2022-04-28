namespace Models
{
    public class ChessSquareResponseModel
    {
        public bool Valid { get; }
        public List<ChessSquareModel> Moves { get; }

        public ChessSquareResponseModel(bool valid, List<ChessSquareModel> moves)
        {
            Valid = valid;
            Moves = moves;
        }
    }
}
