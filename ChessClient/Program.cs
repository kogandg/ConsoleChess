using System;
using System.Text.Json;

namespace ChessClient
{
    class Program
    {
        static HttpClient client = new HttpClient()
        {
            BaseAddress = new Uri("http://localhost:5184/Chess/")
        };
        static async Task Main()
        {
            ConsoleChessVisualizer chessVisualizer = new ConsoleChessVisualizer(7, 3);
            Guid playerId = new Guid();
            Guid gameId = new Guid();

            DisplayDebugStuff(gameId, playerId);

            var test = await client.GetStringAsync("CreateGame");
            ;
        }

        static void DisplayDebugStuff(Guid gameId, Guid playerId)
        {
            Console.SetCursorPosition(0, 25);
            Console.WriteLine(gameId);
            Console.WriteLine(playerId);
        }
    }
}