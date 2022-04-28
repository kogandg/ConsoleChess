namespace ChessAPI
{
    public class Game
    {
        public ConsoleChess.ChessBoard ChessBoard { get; set; }
        public Player White { get; set; }
        public Player Black { get; set; }
        public bool IsCurrentWhite { get; set; }

        public Game()//, ConsoleChess.ChessBoard board = new ConsoleChess.ChessBoard(new ConsoleChess.ConsoleChessVisualizer(7, 3)))
        {
            ChessBoard = new ConsoleChess.ChessBoard();
            IsCurrentWhite = ChessBoard.IsCurrentMoveWhite;
        }
    }
    public static class Games
    {
        public static Dictionary<Guid, Game> GamesList = new Dictionary<Guid, Game>();
    }

}
