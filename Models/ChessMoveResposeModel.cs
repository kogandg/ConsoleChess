namespace Models
{
    public class ChessMoveResposeModel
    {
        public bool Valid { get; }
        public string FEN { get; }

        public ChessMoveResposeModel(bool valid, string fen)
        {
            Valid = valid;
            FEN = fen;
        }
    }
}
