using System.Text.Json;
using System.Text.Json.Serialization;

namespace Models
{
    public class GameJoinResponseModel
    {
        //[JsonPropertyName("playerID")]
        public Guid PlayerID { get; init; }

        //[JsonPropertyName("gameID")]
        public Guid GameID { get; init; }

        public string FEN { get; init; }

        public bool IsWhite { get; init; }
        public GameJoinResponseModel(Guid playerID, Guid gameID, string fen, bool isWhite)
        {
            PlayerID = playerID;
            GameID = gameID;
            FEN = fen;
            IsWhite = isWhite;

        }
        public GameJoinResponseModel()
        {

        }
    }
}
