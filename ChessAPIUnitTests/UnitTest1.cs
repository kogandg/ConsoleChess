using System.Net.Http;
using Xunit;
using System.Text.Json;
using System.Collections.Generic;
using ChessAPI;
using Models;

namespace ChessAPIUnitTests
{
    public class UnitTest1
    {
        [Fact]
        public void TestGameCreate()
        {
            var controller = new ChessAPI.Controllers.ChessController();
            List<GameJoinResponseModel> responses = new List<GameJoinResponseModel>();
            for (int i = 0; i < 10; i++)
            {
                responses.Add(controller.CreateGame());
            }

            foreach (var response in responses)
            {
                foreach (var game in Games.GamesList)
                {
                    if (response.GameID == game.Key)
                    {
                        var white = game.Value.White;
                        var black = game.Value.Black;
                        if (white != null) Assert.True(white.PlayerID == response.PlayerID);
                        if (black != null) Assert.True(black.PlayerID == response.PlayerID);
                    }
                }
            }
        }

        [Fact]
        public void TestJoinGame()
        {
            //confirmed worked via other test
            var controller = new ChessAPI.Controllers.ChessController();
            for (int i = 0; i < 10; i++)
            {
                controller.CreateGame();
            }
            //end of confirmed working code

            var response = controller.JoinGame();

            foreach (var game in Games.GamesList)
            {
                if (response.Value.GameID == game.Key)
                {
                    var white = game.Value.White;
                    var black = game.Value.Black;
                    if (white != null && black != null)
                    {
                        Assert.True(white.PlayerID == response.Value.PlayerID || black.PlayerID == response.Value.PlayerID);
                        return;
                    }
                }
            }
            Assert.True(false);
        }

        [Fact]
        public void TestJoinGameWithoutEnoughGames()
        {
            var controller = new ChessAPI.Controllers.ChessController();
            controller.CreateGame();

            List<GameJoinResponseModel> responses = new List<GameJoinResponseModel>();
            responses.Add(controller.JoinGame().Value);
            responses.Add(controller.JoinGame().Value);
            foreach (var game in Games.GamesList)
            {
                Assert.True(game.Value.White != null && game.Value.Black != null);
            }
            Assert.True(responses[1] == null);
        }

        [Fact]
        public void TestJoinSpecificGame()
        {
            var controller = new ChessAPI.Controllers.ChessController();
            var gameResponse = controller.CreateGame();

            var response = controller.JoinGame(gameResponse.GameID);
            var game = Games.GamesList[gameResponse.GameID];

            Assert.True(game.White != null && game.Black != null);
        }

        [Fact]
        public void TestMakingSelection()
        {
            var controller = new ChessAPI.Controllers.ChessController();
            var gameResponse = controller.CreateGame();
            var joinResponse = controller.JoinGame();

            var game = Games.GamesList[gameResponse.GameID];

            var squareResponse = controller.MakeSelection(gameResponse.GameID, new ChessSquareModel(1, 0));//should be white pawn

            //var piece = game.ChessBoard[new ChessSquareModel(6, 0)];

            Assert.True(squareResponse.Valid == true);
        }

        [Fact]
        public void TestMakingMove()
        {
            var controller = new ChessAPI.Controllers.ChessController();
            var gameResponse = controller.CreateGame();
            var joinResponse = controller.JoinGame();

            var game = Games.GamesList[gameResponse.GameID];

            var squareResponse = controller.MakeSelection(gameResponse.GameID, new ChessSquareModel(1, 0));//should be white pawn

            var makeMoveModel = new MakeMoveModel(game.White.PlayerID, new ChessSquareModel(1, 0), squareResponse.Moves[1]);
            var moveResponse = controller.MakeMove(gameResponse.GameID, makeMoveModel);
            Assert.True(moveResponse.Valid == true);
        }
    }
}
