using System;
using System.Collections.Generic;

namespace ConsoleChess
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            ChessBoard chessBoard = new ChessBoard(7, 3);
            //chessBoard.FromFEN("rnbqkbnr/1pp1pppp/8/8/8/p2p4/PPPPPPPP/RNBQKBNR w");
            //chessBoard.FromFEN("N7/8/8/8/4b3/8/8/8");

            RunGame(chessBoard);

            //Point currentCursor = new Point(Console.CursorLeft, Console.CursorTop);
            //Point newCursor = currentCursor;
            ////while(true)
            //{
            //    newCursor = new Point(Console.CursorLeft, Console.CursorTop);
            //    if (currentCursor != newCursor)
            //    {
            //        Console.WriteLine($"{Console.CursorLeft}, {Console.CursorTop}");
            //    }
            //    currentCursor = newCursor;
            //}
            //Console.WriteLine($"{Console.CursorLeft}, {Console.CursorTop}");
        }

        static Dictionary<ConsoleKey, ChessBoard.KeyPressed> keys = new Dictionary<ConsoleKey, ChessBoard.KeyPressed>()
        {
            [ConsoleKey.NoName] = ChessBoard.KeyPressed.NoName,
            [ConsoleKey.RightArrow] = ChessBoard.KeyPressed.Right,
            [ConsoleKey.LeftArrow] = ChessBoard.KeyPressed.Left,
            [ConsoleKey.UpArrow] = ChessBoard.KeyPressed.Up,
            [ConsoleKey.DownArrow] = ChessBoard.KeyPressed.Down,
            [ConsoleKey.Enter] = ChessBoard.KeyPressed.Enter,
            [ConsoleKey.Escape] = ChessBoard.KeyPressed.Escape
        };


        private static void RunGame(ChessBoard chessBoard)
        {
            var keyPressed = ConsoleKey.NoName;
            string currentFEN = "rnbqkbnr/1pp1pppp/8/8/8/p2p4/PPPPPPPP/RNBQKBNR w KQkq -";
            while (true)
            {
                chessBoard.FromFEN(currentFEN);
                chessBoard.Update(keys[keyPressed]);
                chessBoard.DrawBoard();
                

                currentFEN = chessBoard.ToFEN();


                keyPressed = Console.ReadKey(true).Key;
            }
        }
    }
}
