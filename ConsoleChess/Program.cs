using System;
using System.Collections.Generic;

namespace ConsoleChess
{
    class Program
    {
        public static ChessBoard chessBoard;
        public static ConsoleChessInputManager consoleChessInputManager;
        public static ConsoleChessVisualizer consoleChessVisualizer = new ConsoleChessVisualizer(7, 3);
        
        static void Main(string[] args)
        {
            //CurrentClientShownPosition = new Point(0, 0);

            Console.CursorVisible = false;
            chessBoard = new ChessBoard();//, 7, 3);
            //chessBoard.FromFEN("rnbqkbnr/1pp1pppp/8/8/8/p2p4/PPPPPPPP/RNBQKBNR w");
            //chessBoard.FromFEN("N7/8/8/8/4b3/8/8/8");
            consoleChessInputManager = new ConsoleChessInputManager(Validate);
            RunGame(chessBoard);
        }


        public static Dictionary<ConsoleKey, ChessBoard.KeyPressed> Keys = new Dictionary<ConsoleKey, ChessBoard.KeyPressed>()
        {
            [ConsoleKey.NoName] = ChessBoard.KeyPressed.NoName,
            [ConsoleKey.RightArrow] = ChessBoard.KeyPressed.Right,
            [ConsoleKey.LeftArrow] = ChessBoard.KeyPressed.Left,
            [ConsoleKey.UpArrow] = ChessBoard.KeyPressed.Up,
            [ConsoleKey.DownArrow] = ChessBoard.KeyPressed.Down,
            [ConsoleKey.Enter] = ChessBoard.KeyPressed.Enter,
            [ConsoleKey.Escape] = ChessBoard.KeyPressed.Escape
        };


        private static async void RunGame(ChessBoard chessBoard)
        {
            ////var keyPressed = ConsoleKey.NoName;
            //string checkTesting = "2r5/pp4pp/3b2k1/pP1Np3/4P3/8/P1n2PPP/R5K1 w - 05";//"2r5/pp4pp/3b2k1/1P1Np3/4P3/8/P1n2PPP/R5K1 w - 05";//"k7/7R/8/8/8/8/8/1R1K4 b - - 0 1";//"8/8/5R2/kp6/p1p2r2/P7/1P6/8 w - - 0 1";//"r3k2r/8/7B/B7/b7/7b/8/R3K2R w KQkq - 0 1";//"8/8/8/8/5b2/8/8/R2K4 w - - 0 1";// "ppQ1kppp/pppppppp/8/8/8/8/8/8 b - - 0 1";//"rnbq1bnr/ppp2kpp/5p2/3Pp3/2B5/5Q2/PPPP1PPP/RNB1K1NR w KQ - 0 1";//"r1bqkbnr/pppp1Qpp/2n5/4p3/2B1P3/8/PPPP1PPP/RNB1K1NR b KQkq - 0 1";//"rnbqkbnr/1ppB2pp/8/p3pp2/3P4/4P3/PPP2PPP/RNBQK1NR b KQkq -0 1";//"8/8/8/7b/8/8/4P3/3K4 w - - 0 1";//"8/8/8/7b/8/8/8/4K3 w - - 0 1";//"8/8/8/7b/8/8/4P3/3K4 w - - 0 1";//"8/8/8/7b/8/8/8/3K4 w - - 0 1";//;
            //string currentFEN = checkTesting;//"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq -";
            ////string castleingTestingBoard = "r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1";
            //Point currentPosition = new Point(0, 0);
            //Point currentMove = new Point(0, 0);
            //List<Point> currentMoves = new List<Point>();

            //chessBoard.Visualizer.DrawBackgound();
            //while (true)
            //{
            //    chessBoard.FromFEN(currentFEN);
            //    consoleChessVisualizer.DrawBoard(chessBoard, currentPosition, chessBoard.ShowMoves, currentMove, currentMoves);
            //    var inputInfo = consoleChessInputManager.GetInput(currentPosition);

            //    if(chessBoard.ShowMoves)
            //    {
            //        if(inputInfo.isSelected)
            //        {
            //            chessBoard.Update(inputInfo.square);
            //            if (inputInfo.square != null)
            //            {
            //                currentPosition = inputInfo.square;
            //            }
            //        }
            //        else
            //        {
            //            currentMove = inputInfo.square;
            //        }
            //    }
            //    else
            //    {
            //        if (inputInfo.isSelected)
            //        {
            //            chessBoard.Update(currentPosition);
            //            consoleChessInputManager.Moves = chessBoard[currentPosition].AllowedMoves();
            //            currentMoves = consoleChessInputManager.Moves;
            //        }
            //        else
            //        {
            //            currentPosition = inputInfo.square;
            //        }
            //    }

            //    currentFEN = chessBoard.ToFEN();

            //    //keyPressed = Console.ReadKey(true).Key;
            //}
        }

        public static bool Validate((Point point, bool thing) stuff)
        {
            return stuff.point.IsInside() && !(chessBoard[stuff.point] is Pieces.EmptyPiece) && chessBoard[stuff.point].IsWhite() == chessBoard.IsCurrentMoveWhite;
        }
    }
}
