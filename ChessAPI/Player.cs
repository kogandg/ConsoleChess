namespace ChessAPI
{
    public class Player
    {
        public Guid PlayerID { get; }

        public Player(Guid id)
        {
            PlayerID = id;
        }
    }
}
