using Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ChessAPI.Controllers
{
    [Route("Chess")]
    public class ChessController : Controller
    {
        [HttpPost("MakeSelection/{gameID}")]
        public ChessSquareResponseModel MakeSelection(Guid gameID, [FromBody] MakeSelectionModel makeSelection)
        {
            var game = Games.GamesList[gameID];
            var board = game.ChessBoard;
            bool isWhite = game.White.PlayerID == makeSelection.PlayerID;
            bool valid = board.IsOwnPiece(makeSelection.ChessSquare, isWhite);//new ConsoleChess.Point(chessSquare.Col, chessSquare.Row), game.IsCurrentWhite);
            if (valid)
            {
                return new ChessSquareResponseModel(true, board.GetMoves(makeSelection.ChessSquare).Select(x => (ChessSquareModel)x).ToList());//.Select(x => new ChessSquareModel { Row = x.Y, Col = x.X }).ToList());
            }
            return new ChessSquareResponseModel(false, new List<ChessSquareModel>());
        }

        [HttpPost("MakeMove/{gameID}")]
        public ChessMoveResposeModel MakeMove(Guid gameID, [FromBody] MakeMoveModel makeMove)// Guid playerID, ChessSquareModel from, ChessSquareModel to)//to from instead?
        {
            var game = Games.GamesList[gameID];
            var board = game.ChessBoard;

            if (!board.IsValidPiece(makeMove.From) || !board.IsValidSquare(makeMove.To))
            {
                return new ChessMoveResposeModel(false, "");
            }

            var pID = makeMove.PlayerId;
            if ((pID == game.White.PlayerID) == game.IsCurrentWhite)
            {

                if ((pID == game.White.PlayerID && board[makeMove.From].IsWhite()) ||
                    (pID == game.Black.PlayerID && !board[makeMove.From].IsWhite()))
                {

                    var allowedMoves = board[makeMove.From].AllowedMoves();

                    foreach (var move in allowedMoves)
                    {
                        if (makeMove.To == move)
                        {
                            board.Update(makeMove.To, makeMove.From);
                            game.IsCurrentWhite = !game.IsCurrentWhite;
                            return new ChessMoveResposeModel(true, board.ToFEN());
                        }
                    }
                }
            }
            return new ChessMoveResposeModel(false, "");
        }

        [HttpGet("JoinGame")]
        public ActionResult<GameJoinResponseModel> JoinGame()
        {
            Player player = new Player(Guid.NewGuid());

            foreach (var game in Games.GamesList)
            {
                if (game.Value.White == null)
                {
                    game.Value.White = player;
                    return new GameJoinResponseModel() { PlayerID = player.PlayerID, GameID = game.Key, FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", IsWhite = true };
                }
                else if (game.Value.Black == null)
                {
                    game.Value.Black = player;
                    return new GameJoinResponseModel() { PlayerID = player.PlayerID, GameID = game.Key, FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", IsWhite = false };
                }
            }

            return new GameJoinResponseModel() { PlayerID = Guid.Empty, GameID = Guid.Empty, FEN = "" };
        }

        [HttpGet("JoinGame/{gameId}")]
        public GameJoinResponseModel JoinGame(Guid gameId)
        {
            if (Games.GamesList.ContainsKey(gameId))
            {
                var game = Games.GamesList[gameId];
                if (game.White == null)
                {
                    Player player = new Player(Guid.NewGuid());
                    game.White = player;
                    return new GameJoinResponseModel() { PlayerID = player.PlayerID, GameID = gameId, FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", IsWhite = true };
                }
                if (game.Black == null)
                {
                    Player player = new Player(Guid.NewGuid());
                    game.Black = player;
                    return new GameJoinResponseModel() { PlayerID = player.PlayerID, GameID = gameId, FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", IsWhite = false };
                }
            }
            return null;
        }


        [HttpGet("CreateGame")]
        public GameJoinResponseModel CreateGame()
        {
            Player player = new Player(Guid.NewGuid());
            var gameId = Guid.NewGuid();
            var game = new Game();

            string defualtBoardFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            string testingTakingFen = "rnbqkbnr/ppp1pppp/8/3p4/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 1";
            string testingEnpassantFen = "rnbqkbnr/ppp1pppp/8/8/3p4/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            game.ChessBoard.FromFEN(testingEnpassantFen);//idk if this is the place to create the base fen of the game

            Games.GamesList.Add(gameId, game);

            Random rand = new Random();
            int coinFlip = rand.Next(0, 2);
            bool isWhite = false;
            //if (coinFlip == 0)
            //{
            //    Games.GamesList[gameId].White = player;
            //    isWhite = true;
            //}
            if (true)//(coinFlip == 1)
            {
                Games.GamesList[gameId].Black = player;
            }
            return new GameJoinResponseModel() { PlayerID = player.PlayerID, GameID = gameId, FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", IsWhite = isWhite };
        }

        [HttpGet("InFullGame/{gameId}")]
        public ActionResult<bool> InFullGame(Guid gameId)
        {
            if (!Games.GamesList.ContainsKey(gameId)) return NotFound();
            var game = Games.GamesList[gameId];
            //if(game.White != null && game.Black != null)
            //{
            //    ;
            //    return true;
            //}
            return game.White != null && game.Black != null;
        }

        [HttpGet("GetFen/{gameId}")]
        public ActionResult<string> GetFen(Guid gameId)
        {
            if (!Games.GamesList.ContainsKey(gameId)) return NotFound();
            var board = Games.GamesList[gameId].ChessBoard;

            //Console.WriteLine(board.ToFEN());
            return board.ToFEN();
        }


        //[HttpGet("CreateGame/{fen}")]
        //public GameJoinResponseModel CreateGame(string fen)//maybe have a seperate list for custom games or dont allow it at all, maybe in a similar thing to chess variants if i ever do that
        //{
        //    Player player = new Player(Guid.NewGuid());
        //    var gameId = Guid.NewGuid();
        //    var game = new Game();
        //    game.ChessBoard.FromFEN(fen);
        //    Games.GamesList.Add(gameId, game);

        //    Random rand = new Random();
        //    int coinFlip = rand.Next(0, 2);
        //    if (coinFlip == 0)
        //    {
        //        Games.GamesList[gameId].White = player;
        //    }
        //    if (coinFlip == 1)
        //    {
        //        Games.GamesList[gameId].Black = player;
        //    }
        //    return new GameJoinResponseModel(player.PlayerID, gameId);
        //}
    }
}
