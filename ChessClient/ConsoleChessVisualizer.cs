//using ConsoleChess.Pieces;
using System;
using System.Collections.Generic;
using System.Text;
using Console = Colorful.Console;
using System.Drawing;
using ChessLibrary;
using PointLibrary;
using Pieces;

namespace ChessClient
{
    public class ConsoleChessVisualizer : IVisualizer
    {
        int squareWidth;
        int squareHeight;

        public ConsoleChessVisualizer(int width, int height)
        {
            squareWidth = width;
            squareHeight = height;
            int c = 95;
            Color color = Color.FromArgb(255, c, c, c);
            Console.BackgroundColor = color;
            Console.Clear();
            //Console.SetWindowSize(1, 35);
        }

        public void DrawBoard(ChessBoard chessBoard, PointLibrary.Point current, PointLibrary.Point currentMove, List<PointLibrary.Point> moves)
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;

            int currentYValue;

            //Spaced out with outline//Actual drawing of pieces
            for (int row = 0; row < 8; row++)
            {
                currentYValue = Math.Abs(row - 7);
                for (int column = 0; column < 8; column++)
                {
                    Console.ForegroundColor = Color.White;
                    var output = chessBoard[currentYValue, column];
                    if (output.IsWhite())
                    {
                        Console.ForegroundColor = Color.White;
                    }
                    else
                    {
                        Console.ForegroundColor = Color.Black;
                    }
                    if (output.CurrentPosition == current)
                    {
                        Console.ForegroundColor = Color.Red;
                        if (chessBoard.ShowMoves)
                        {
                            Console.ForegroundColor = Color.Yellow;//Color.FromArgb(255, 249, 241, 165);
                        }
                    }
                    if (chessBoard.IsWhite)
                    {
                        Console.SetCursorPosition(((column + 1) * squareWidth) - 4, ((row + 1) * squareHeight) - 1);
                    }
                    else
                    {
                        Console.SetCursorPosition(((Math.Abs(column-8)) * squareWidth) - 4, ((Math.Abs(row-8)) * squareHeight) - 1);
                    }
                    Console.Write(chessBoard.FENToScreenOutput[output.FENNotation]);//fenToCentered[GridSquares[row, column]]);
                }
            }

            if (chessBoard.ShowMoves)
            {
                //Console.ForegroundColor = Color.Green;
                //var moves = chessBoard.DrawingPoints;
                //var moves = chessBoard[chessBoard.CurrentPosition.Y, chessBoard.CurrentPosition.X].AllowedMoves();
                //if(chessBoard[chessBoard.CurrentPosition] is King)
                //{
                //    moves = ((King)chessBoard[chessBoard.CurrentPosition]).AllowedMoves();
                //}
                for (int i = 0; i < moves.Count; i++)
                {
                    Console.ForegroundColor = Color.Blue;
                    if (chessBoard.IsWhite)
                    {
                        Console.SetCursorPosition(((moves[i].X + 1) * squareWidth) - 4, ((Math.Abs(moves[i].Y - 8)) * squareHeight) - 1);
                    }
                    else
                    {
                        Console.SetCursorPosition((Math.Abs(moves[i].X -8) * squareWidth) - 4, ((Math.Abs(moves[i].Y +1)) * squareHeight) - 1);
                    }
                    //Console.SetCursorPosition(((0 + 1) * squareWidth) - 4, ((1 + 1) * squareHeight) - 1);
                    if (moves[i] == currentMove)
                    {
                        Console.ForegroundColor = Color.Red;
                    }
                    if (chessBoard[moves[i].Y, moves[i].X] is EmptyPiece)
                    {
                        Console.Write("▓▓");
                    }
                    else
                    {
                        Console.Write(chessBoard.FENToScreenOutput[chessBoard[moves[i].Y, moves[i].X].FENNotation]);
                    }
                }
            }

            if (chessBoard.IsPromoting)
            {
                PointLibrary.Point startingPoint = new PointLibrary.Point(60, 6);
                DrawPromotionMenuOutline(startingPoint);
                for (int i = 0; i < 4; i++)
                {
                    Console.ForegroundColor = Color.White;
                    Console.SetCursorPosition(startingPoint.X + 3, startingPoint.Y + (squareHeight * i) + 2);
                    if (i == chessBoard.PromotionIndex)
                    {
                        Console.ForegroundColor = Color.Red;
                    }
                    Console.Write(chessBoard.FENToScreenOutput[chessBoard.PromotionPieces[i]]);
                }
            }
            if (chessBoard.HasPromoted)
            {
                ClearPromotion(new PointLibrary.Point(60, 6));
                chessBoard.HasPromoted = false;
                chessBoard.Once = false;
            }
            return;
            #region SmallBoard

            ////smallMode

            //for (int row = 0; row < 8; row++)
            //{
            //    for (int column = 0; column < 8; column++)
            //    {
            //        Console.ForegroundColor = ConsoleColor.White;
            //        if (GridSquares[row, column].FENNotation == '.')
            //        {
            //            Console.ForegroundColor = ConsoleColor.DarkGray;
            //        }
            //        if (currentPosition.Y == row && currentPosition.X == column)
            //        {
            //            Console.ForegroundColor = ConsoleColor.Red;
            //        }

            //        Console.SetCursorPosition(column * 2, row);
            //        Console.Write(GridSquares[row, column].FENNotation);
            //    }
            //}

            //if (showMoves)
            //{
            //    Console.ForegroundColor = ConsoleColor.Cyan;
            //    var moves = GridSquares[currentPosition.Y, currentPosition.X].PossibleMoves();
            //    for (int i = 0; i < moves.Count; i++)
            //    {
            //        Console.ForegroundColor = ConsoleColor.Cyan;
            //        if (i == currentMoveIndex)
            //        {
            //            Console.ForegroundColor = ConsoleColor.Red;
            //        }
            //        Console.SetCursorPosition(moves[i].X * 2, moves[i].Y);
            //        Console.Write('.');
            //    }
            //}
            #endregion
        }
        public void DrawBackgound()
        {
            Console.ForegroundColor = Color.MediumSeaGreen;//Color.LightGreen;
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            for (int y = 0; y < (8 * squareHeight) + 1; y++)
            {
                for (int x = 0; x < (8 * squareWidth) + 1; x++)
                {
                    if (y % squareHeight == 0 && x != 0 && x != (8 * squareWidth))
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write('_');//\u035F');
                    }
                    if (x % squareWidth == 0 && y != 0)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write('|');
                    }
                }
            }
        }

        void DrawPromotionMenuOutline(PointLibrary.Point startingPoint)
        {
            Console.ForegroundColor = Color.Black;
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            for (int y = 0; y < (4 * squareHeight) + 1; y++)
            {
                for (int x = 0; x < (1 * squareWidth) + 1; x++)
                {
                    if (y % squareHeight == 0 && x != 0 && x != (4 * squareWidth))
                    {
                        Console.SetCursorPosition(x + startingPoint.X, y + startingPoint.Y);
                        Console.Write('_');//\u035F');
                    }
                    if (x % squareWidth == 0 && y != 0)
                    {
                        Console.SetCursorPosition(x + startingPoint.X, y + startingPoint.Y);
                        Console.Write('|');
                    }
                }
            }
        }

        void ClearPromotion(PointLibrary.Point startingPoint)
        {
            for (int y = 0; y < (4 * squareHeight) + 1; y++)
            {
                for (int x = 0; x < (1 * squareWidth) + 1; x++)
                {
                    Console.SetCursorPosition(x + startingPoint.X, y + startingPoint.Y);
                    Console.Write(" ");
                }
            }
        }
    }
}
