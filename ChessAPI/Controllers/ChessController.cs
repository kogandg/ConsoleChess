using ChessAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ChessAPI.Controllers
{
    [Route("Chess")]
    public class ChessController : Controller
    {
        [HttpPost("MakeSelection/{gameID}")]
        public ChessSquareResponseModel MakeSelection(Guid gameID, [FromBody] ChessSquareModel chessSquare)
        {
            var game = Games.GamesList[gameID];
            var board = game.ChessBoard;

            bool valid = board.IsOwnPiece(chessSquare, game.IsCurrentWhite);//new ConsoleChess.Point(chessSquare.Col, chessSquare.Row), game.IsCurrentWhite);
            if (valid)
            {
                return new ChessSquareResponseModel(true, board.GetMoves(chessSquare).Select(x => (ChessSquareModel)x).ToList());//.Select(x => new ChessSquareModel { Row = x.Y, Col = x.X }).ToList());
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

            if ((makeMove.PlayerId == game.White.PlayerID && board[makeMove.From].IsWhite()) ||
                (makeMove.PlayerId == game.Black.PlayerID && !board[makeMove.To].IsWhite()))
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
                    return new GameJoinResponseModel(player.PlayerID, game.Key);
                }
                else if (game.Value.Black == null)
                {
                    game.Value.Black = player;
                    return new GameJoinResponseModel(player.PlayerID, game.Key);
                }
            }

            return NotFound();
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
                    return new GameJoinResponseModel(player.PlayerID, gameId);
                }
                if (game.Black == null)
                {
                    Player player = new Player(Guid.NewGuid());
                    game.Black = player;
                    return new GameJoinResponseModel(player.PlayerID, gameId);
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
            //for now ill keep this for testing if you can move
            game.ChessBoard.FromFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");//idk if this is the place to create the base fen of the game
            
            Games.GamesList.Add(gameId, game);

            Random rand = new Random();
            int coinFlip = rand.Next(0, 2);
            if (coinFlip == 0)
            {
                Games.GamesList[gameId].White = player;
            }
            if (coinFlip == 1)
            {
                Games.GamesList[gameId].Black = player;
            }
            return new GameJoinResponseModel(player.PlayerID, gameId);
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
