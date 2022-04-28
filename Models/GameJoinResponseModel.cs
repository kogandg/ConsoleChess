namespace Models
{
    public class GameJoinResponseModel
    {       
        public Guid PlayerID { get; }
        public Guid GameID { get; }
        public GameJoinResponseModel(Guid playerID, Guid gameID)
        {
            PlayerID = playerID;
            GameID = gameID;
        }
    }
}
