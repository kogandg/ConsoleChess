namespace Models
{
    public class MakeMoveModel
    {
        public Guid PlayerId { get; set; }
        public ChessSquareModel From { get; set; }
        public ChessSquareModel To { get; set; }

        public MakeMoveModel(Guid playerId, ChessSquareModel from, ChessSquareModel to)
        {
            PlayerId = playerId;
            From = from;
            To = to;
        }
    }
}
