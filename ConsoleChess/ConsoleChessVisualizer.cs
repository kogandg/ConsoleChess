using ConsoleChess.Pieces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleChess
{
    public class ConsoleChessVisualizer : IVisualizer
    {
        int squareWidth;
        int squareHeight;

        public ConsoleChessVisualizer(int width, int height)
        {
            squareWidth = width;
            squareHeight = height;
        }

        public void DrawBoard(ChessBoard chessBoard)
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;

            //Spaced out with outline
            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    var output = chessBoard[row, column];
                    //if (output.IsWhite())
                    //{
                    //    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    //}
                    //else
                    //{
                    //    Console.ForegroundColor = ConsoleColor.Blue;
                    //}
                    if (output.CurrentPosition == chessBoard.CurrentPosition)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        if (chessBoard.ShowMoves)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                        }
                    }
                    Console.SetCursorPosition(((column + 1) * squareWidth) - 4, ((row + 1) * squareHeight) - 1);
                    Console.Write(chessBoard.FENToScreenOutput[output.FENNotation]);//fenToCentered[GridSquares[row, column]]);
                }
            }

            if (chessBoard.ShowMoves)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                var moves = chessBoard[chessBoard.CurrentPosition.Y, chessBoard.CurrentPosition.X].PossibleMoves();
                for (int i = 0; i < moves.Count; i++)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.SetCursorPosition(((moves[i].X + 1) * squareWidth) - 4, ((moves[i].Y + 1) * squareHeight) - 1);
                    //Console.SetCursorPosition(((0 + 1) * squareWidth) - 4, ((1 + 1) * squareHeight) - 1);
                    if (i == chessBoard.CurrentMoveIndex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    if (chessBoard[moves[i].Y, moves[i].X] is EmptyPiece)
                    {
                        Console.Write("..");
                    }
                    else
                    {
                        Console.Write(chessBoard.FENToScreenOutput[chessBoard[moves[i].Y, moves[i].X].FENNotation]);
                    }
                }
            }

            if (chessBoard.IsPromoting)
            {
                Point startingPoint = new Point(60, 6);
                DrawPromotionMenuOutline(startingPoint);
                for (int i = 0; i < 4; i++)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.SetCursorPosition(startingPoint.X + 3, startingPoint.Y + (squareHeight * i) + 2);
                    if (i == chessBoard.PromotionIndex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    Console.Write(chessBoard.FENToScreenOutput[chessBoard.PromotionPieces[i]]);
                }
            }
            if (chessBoard.HasPromoted)
            {
                ClearPromotion(new Point(60, 6));
                chessBoard.HasPromoted = false;
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
            Console.ForegroundColor = ConsoleColor.White;
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

        void DrawPromotionMenuOutline(Point startingPoint)
        {
            Console.ForegroundColor = ConsoleColor.White;
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

        void ClearPromotion(Point startingPoint)
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
