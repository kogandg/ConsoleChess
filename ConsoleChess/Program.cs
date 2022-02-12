using System;
using System.Collections.Generic;

namespace ConsoleChess
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            ConsoleChessVisualizer consoleChessVisualizer = new ConsoleChessVisualizer(7, 3);
            ChessBoard chessBoard = new ChessBoard(consoleChessVisualizer);//, 7, 3);
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
            string checkTesting = "3q4/8/8/8/8/8/8/3Q4 w - - 0 1";//"k7/7R/8/8/8/8/8/1R1K4 b - - 0 1";//"8/8/5R2/kp6/p1p2r2/P7/1P6/8 w - - 0 1";//"r3k2r/8/7B/B7/b7/7b/8/R3K2R w KQkq - 0 1";//"8/8/8/8/5b2/8/8/R2K4 w - - 0 1";// "ppQ1kppp/pppppppp/8/8/8/8/8/8 b - - 0 1";//"rnbq1bnr/ppp2kpp/5p2/3Pp3/2B5/5Q2/PPPP1PPP/RNB1K1NR w KQ - 0 1";//"r1bqkbnr/pppp1Qpp/2n5/4p3/2B1P3/8/PPPP1PPP/RNB1K1NR b KQkq - 0 1";//"rnbqkbnr/1ppB2pp/8/p3pp2/3P4/4P3/PPP2PPP/RNBQK1NR b KQkq -0 1";//"8/8/8/7b/8/8/4P3/3K4 w - - 0 1";//"8/8/8/7b/8/8/8/4K3 w - - 0 1";//"8/8/8/7b/8/8/4P3/3K4 w - - 0 1";//"8/8/8/7b/8/8/8/3K4 w - - 0 1";//;
            string currentFEN = checkTesting;//"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq -";
            //string castleingTestingBoard = "r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1";

            chessBoard.Visualizer.DrawBackgound();
            while (true)
            {
                //if (keys.ContainsValue(keyPressed)) contine;
                chessBoard.FromFEN(currentFEN);
                chessBoard.Update(keys[keyPressed]);
                chessBoard.DrawBoard();


                currentFEN = chessBoard.ToFEN();


                keyPressed = Console.ReadKey(true).Key;
            }
        }
    }
}
