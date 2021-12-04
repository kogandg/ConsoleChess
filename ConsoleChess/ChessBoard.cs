using System;
using System.Collections.Generic;
using System.Text;
using ConsoleChess.Pieces;

namespace ConsoleChess
{
    public class ChessBoard
    {
        public Piece this[Point p]
        {
            get
            {
                if (!p.IsInside())
                {
                    return null;
                }
                return GridSquares[p.Y, p.X];
            }
        }
        public Piece this[int y, int x]
        {
            get
            {
                if (x > 7 || x < 0 || y > 7 || y < 0)
                {
                    return null;
                }
                return GridSquares[y, x];
            }
        }
        public Piece[,] GridSquares { get; private set; }
        private int squareHeight;
        private int squareWidth;
        private Dictionary<char, string> fenToScreenOutput;
        private Dictionary<char, Func<Color, Point, Piece>> fenToPiece;

        private int currentMoveIndex;
        private Point currentPosition;
        private bool showMoves;
        private bool isPromoting;
        private int promotionIndex;
        private char[] promotionPieces;
        bool once;
        bool hasPromoted;

        bool KCastle;
        bool QCastle;
        bool kCastle;
        bool qCastle;

        private bool isCurrentMoveWhite;
        public Point EnPassantTargetSquare { get; private set; }

        public ChessBoard(int width, int height)
        {
            GridSquares = new Piece[8, 8];
            squareWidth = width;
            squareHeight = height;

            fenToScreenOutput = new Dictionary<char, string>()
            {
                ['.'] = "  ",
                ['P'] = "PN",
                ['R'] = "RK",
                ['N'] = "KN",
                ['B'] = "BP",
                ['Q'] = "QN",
                ['K'] = "KG",
                ['p'] = "pn",
                ['r'] = "rk",
                ['n'] = "kn",
                ['b'] = "bp",
                ['q'] = "qn",
                ['k'] = "kg",
            };

            fenToPiece = new Dictionary<char, Func<Color, Point, Piece>>()
            {
                ['.'] = (Color c, Point p) => new EmptyPiece(p, this),
                ['P'] = (Color c, Point p) => new Pawn(c, p, this),
                ['R'] = (Color c, Point p) => new Rook(c, p, this),
                ['N'] = (Color c, Point p) => new Knight(c, p, this),
                ['B'] = (Color c, Point p) => new Bishop(c, p, this),
                ['Q'] = (Color c, Point p) => new Queen(c, p, this),
                ['K'] = (Color c, Point p) => new King(c, p, this),
            };

            currentMoveIndex = 0;
            currentPosition = new Point(5, 7);

            isCurrentMoveWhite = true;
            EnPassantTargetSquare = new Point(-1, -1);

            promotionIndex = 0;
            promotionPieces = new char[4]
            {
                'R',
                'N',
                'B',
                'Q'
            };
            once = false;
            hasPromoted = false;

            DrawGridOutline();
        }

        #region FEN

        public string ToFEN()
        {
            StringBuilder FEN = new StringBuilder();
            int blankSpace = 0;
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (GridSquares[x, y].FENNotation == ' ' || GridSquares[x, y].FENNotation == '.')
                    {
                        blankSpace++;
                    }
                    else
                    {
                        if (blankSpace != 0)
                        {
                            FEN.Append(blankSpace);
                            blankSpace = 0;
                        }
                        FEN.Append(GridSquares[x, y].FENNotation);
                    }
                }
                if (blankSpace != 0)
                {
                    FEN.Append(blankSpace);
                    blankSpace = 0;
                }
                FEN.Append('/');
            }

            FEN.Remove(FEN.Length - 1, 1);
            FEN.Append(" ");
            if (isCurrentMoveWhite)
            {
                FEN.Append('w');
            }
            else
            {
                FEN.Append('b');
            }

            FEN.Append(" ");
            if (!EnPassantTargetSquare.IsInside())
            {
                FEN.Append('-');
            }
            else
            {
                FEN.Append(EnPassantTargetSquare.X);
                FEN.Append(EnPassantTargetSquare.Y);
            }
            return FEN.ToString();
        }

        //public void FromFEN(string fen)
        //{
        //    string[] rows = fen.Split("/");
        //    int blankSpace = 0;
        //    int currentColumnPlus = 0;
        //    for (int row = 0; row < rows.Length; row++)
        //    {
        //        for (int column = 0; column < rows[row].Length; column++)
        //        {

        //            blankSpace = 0;
        //            if (rows[row] == "3p4")
        //            {
        //                ;
        //            }
        //            if (char.IsDigit(rows[row][column]))
        //            {
        //                blankSpace = int.Parse(rows[row][column].ToString());
        //            }
        //            else
        //            {
        //                GridSquares[row, column + currentColumnPlus] = rows[row][column];
        //            }

        //            for (int i = 0; i < blankSpace; i++)
        //            {
        //                GridSquares[row, column + currentColumnPlus] = '.';
        //                currentColumnPlus++;
        //            }
        //            //if(thing)
        //        }
        //        currentColumnPlus = 0;
        //    }
        //}

        public void FromFEN(string fen)
        {
            string[] rows = fen.Split("/");
            int currentColumnValue;
            List<string> suffixes = new List<string>(rows[7].Split(" "));
            rows[7] = suffixes[0];
            suffixes.RemoveAt(0);

            for (int row = 0; row < rows.Length; row++)
            {
                currentColumnValue = 0;
                for (int fenColumn = 0; fenColumn < rows[row].Length; fenColumn++)
                {
                    if (char.IsDigit(rows[row][fenColumn]))
                    {
                        int blankSpace = int.Parse(rows[row][fenColumn].ToString());
                        for (int i = 0; i < blankSpace; i++)
                        {
                            GridSquares[row, currentColumnValue] = new EmptyPiece(new Point(currentColumnValue, row), this);
                            currentColumnValue++;
                        }
                    }
                    else
                    {
                        if (char.IsUpper(rows[row][fenColumn]))
                        {
                            GridSquares[row, currentColumnValue] = fenToPiece[rows[row][fenColumn]]?.Invoke(Color.White, new Point(currentColumnValue, row));
                        }
                        else
                        {
                            GridSquares[row, currentColumnValue] = fenToPiece[rows[row][fenColumn].ToString().ToUpper()[0]]?.Invoke(Color.Black, new Point(currentColumnValue, row));
                        }
                        //GridSquares[row, currentColumnValue] = rows[row][fenColumn];
                        currentColumnValue++;
                    }
                }
            }

            isCurrentMoveWhite = false;
            if (suffixes[0] == "w")
            {
                isCurrentMoveWhite = true;
            }

            if (suffixes[1] != "-")
            {
                //EnPassantTargetSquare = new Point(suffixes[1][0] - 'a', suffixes[1][1] - 1);
                EnPassantTargetSquare = new Point(suffixes[1][0] - '0', suffixes[1][1] - '0');
                Console.SetCursorPosition(0, 28);
                Console.Write(suffixes[1]);
            }
        }

        #endregion

        #region DrawingBoard
        public void DrawBoard()
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;

            //Spaced out with outline

            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    var output = GridSquares[row, column];
                    //if (output.IsWhite())
                    //{
                    //    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    //}
                    //else
                    //{
                    //    Console.ForegroundColor = ConsoleColor.Blue;
                    //}
                    if (output.CurrentPosition == currentPosition)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        if (showMoves)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                        }
                    }
                    Console.SetCursorPosition(((column + 1) * squareWidth) - 4, ((row + 1) * squareHeight) - 1);
                    Console.Write(fenToScreenOutput[output.FENNotation]);//fenToCentered[GridSquares[row, column]]);
                }
            }

            if (showMoves)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                var moves = GridSquares[currentPosition.Y, currentPosition.X].PossibleMoves();
                for (int i = 0; i < moves.Count; i++)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.SetCursorPosition(((moves[i].X + 1) * squareWidth) - 4, ((moves[i].Y + 1) * squareHeight) - 1);
                    //Console.SetCursorPosition(((0 + 1) * squareWidth) - 4, ((1 + 1) * squareHeight) - 1);
                    if (i == currentMoveIndex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    if (GridSquares[moves[i].Y, moves[i].X] is EmptyPiece)
                    {
                        Console.Write("..");
                    }
                    else
                    {
                        Console.Write(fenToScreenOutput[GridSquares[moves[i].Y, moves[i].X].FENNotation]);
                    }
                }
            }

            if (isPromoting)
            {
                once = true;
                Point startingPoint = new Point(60, 6);
                DrawPromotionMenuOutline(startingPoint);
                for (int i = 0; i < 4; i++)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.SetCursorPosition(startingPoint.X + 3, startingPoint.Y + (squareHeight * i) + 2);
                    if (i == promotionIndex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    Console.Write(fenToScreenOutput[promotionPieces[i]]);
                }
            }
            if (hasPromoted)
            {
                ClearPromotion(new Point(60, 6));
                hasPromoted = false;
            }
            return;
            #region SmallBoard

            //smallMode

            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    if (GridSquares[row, column].FENNotation == '.')
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    }
                    if (currentPosition.Y == row && currentPosition.X == column)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }

                    Console.SetCursorPosition(column * 2, row);
                    Console.Write(GridSquares[row, column].FENNotation);
                }
            }

            if (showMoves)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                var moves = GridSquares[currentPosition.Y, currentPosition.X].PossibleMoves();
                for (int i = 0; i < moves.Count; i++)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    if (i == currentMoveIndex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    Console.SetCursorPosition(moves[i].X * 2, moves[i].Y);
                    Console.Write('.');
                }
            }
            #endregion
        }
        void DrawGridOutline()
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
        #endregion

        public enum KeyPressed
        {
            NoName,
            Right,
            Left,
            Up,
            Down,
            Enter,
            Escape
        }

        public void Update(KeyPressed keyPressed)
        {
            if (showMoves && !isPromoting)
            {
                var moves = GridSquares[currentPosition.Y, currentPosition.X].PossibleMoves();
                if (keyPressed == KeyPressed.Right)
                {
                    moveMoveHelper(new Point(1, 0), moves.Count);
                }
                if (keyPressed == KeyPressed.Left)
                {
                    moveMoveHelper(new Point(-1, 0), moves.Count);
                }
                if (keyPressed == KeyPressed.Escape)
                {
                    showMoves = false;
                    currentMoveIndex = 0;
                }
                if (keyPressed == KeyPressed.Enter)
                {
                    showMoves = false;
                    if (moves.Count != 0)
                    {
                        Point prevPawnMove = new Point(-1, -1);
                        if (this[currentPosition] is Pawn)
                        {
                            prevPawnMove = currentPosition;
                        }

                        Point currentMove = moves[currentMoveIndex];
                        GridSquares[currentMove.Y, currentMove.X] = GridSquares[currentPosition.Y, currentPosition.X];
                        GridSquares[currentPosition.Y, currentPosition.X] = new EmptyPiece(currentPosition, this);
                        GridSquares[currentMove.Y, currentMove.X].CurrentPosition = currentMove;
                        currentMoveIndex = 0;
                        currentPosition = currentMove;

                        if (currentMove == EnPassantTargetSquare)
                        {
                            if (this[currentMove].IsWhite())
                            {
                                GridSquares[currentMove.Y + 1, currentMove.X] = fenToPiece['.']?.Invoke(Color.White, new Point(currentMove.Y + 1, currentMove.X));
                            }
                            else
                            {
                                GridSquares[currentMove.Y - 1, currentMove.X] = fenToPiece['.']?.Invoke(Color.White, new Point(currentMove.Y - 1, currentMove.X));
                            }
                        }

                        if (prevPawnMove.IsInside() && currentPosition.Y - 2 == prevPawnMove.Y)
                        {
                            EnPassantTargetSquare = prevPawnMove;
                            EnPassantTargetSquare.Y++;
                        }
                        else if (prevPawnMove.IsInside() && currentPosition.Y + 2 == prevPawnMove.Y)
                        {
                            EnPassantTargetSquare = prevPawnMove;
                            EnPassantTargetSquare.Y--;
                        }
                        else
                        {
                            EnPassantTargetSquare.X = -1;
                            EnPassantTargetSquare.Y = -1;
                        }

                        if (this[currentMove] is Pawn)
                        {
                            if (currentMove.Y == 0 || currentMove.Y == 7)
                            {
                                isPromoting = true;
                            }
                        }

                        if(!(GridSquares[4, 0] is King))
                        {
                            kCastle = false;
                            qCastle = false;
                        }
                        if(!(GridSquares[4, 7] is King))
                        {
                            KCastle = false;
                            QCastle = false;
                        }

                        isCurrentMoveWhite = !isCurrentMoveWhite;
                    }
                }
            }

            else if (!isPromoting)
            {
                Point tempPoint;
                if (keyPressed == KeyPressed.Left)
                {
                    showMoves = false;
                    tempPoint = currentPositionMoveHelper(new Point(-1, 0));
                    if (this[tempPoint].IsWhite() == isCurrentMoveWhite)
                    {
                        currentPosition = tempPoint;
                    }
                }
                else if (keyPressed == KeyPressed.Right)
                {
                    showMoves = false;
                    tempPoint = currentPositionMoveHelper(new Point(1, 0));
                    if (this[tempPoint].IsWhite() == isCurrentMoveWhite)
                    {
                        currentPosition = tempPoint;
                    }
                }
                else if (keyPressed == KeyPressed.Up)
                {
                    showMoves = false;
                    tempPoint = currentPositionMoveHelper(new Point(0, -1));//currentPositionMoveHelper(new Point(0, -1));
                    if (this[tempPoint].IsWhite() == isCurrentMoveWhite)
                    {
                        currentPosition = tempPoint;
                    }
                }
                else if (keyPressed == KeyPressed.Down)
                {
                    showMoves = false;
                    tempPoint = currentPositionMoveHelper(new Point(0, 1));//currentPositionMoveHelper(new Point(0, 1));
                    if (this[tempPoint].IsWhite() == isCurrentMoveWhite)
                    {
                        currentPosition = tempPoint;
                    }
                }
                else if (keyPressed == KeyPressed.Enter)
                {
                    showMoves = true;
                }
            }


            if (isPromoting)
            {
                if (keyPressed == KeyPressed.Up && promotionIndex > 0)
                {
                    promotionIndex--;
                }
                if (keyPressed == KeyPressed.Down && promotionIndex < 4)
                {
                    promotionIndex++;
                }
                if (keyPressed == KeyPressed.Enter && once)
                {
                    GridSquares[currentPosition.Y, currentPosition.X] = fenToPiece[promotionPieces[promotionIndex]](this[currentPosition].PieceColor, currentPosition);
                    promotionIndex = 0;
                    isPromoting = false;
                    once = false;
                    hasPromoted = true;
                }
            }
        }


        #region MoveHelpers

        public Point yChangeCurrentPositionMoveHelper(Point direction)
        {
            Point leftPoint = currentPosition;
            Point rightPoint = currentPosition;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    leftPoint += direction;
                    rightPoint += direction;
                    if (leftPoint.IsInside() && !(this[leftPoint] is EmptyPiece))
                    {
                        return leftPoint;
                    }
                    if (rightPoint.IsInside() && !(this[rightPoint] is EmptyPiece))
                    {
                        return rightPoint;
                    }
                }
                leftPoint.Y = currentPosition.Y;
                leftPoint.X -= 1;
                rightPoint.Y = currentPosition.Y;
                rightPoint.X += 1;
            }
            return currentPosition;
        }

        public Point xChangeCurrentPositionMoveHelper(Point direction)
        {
            Point topPoint = currentPosition;
            Point bottomPoint = currentPosition;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    topPoint += direction;
                    bottomPoint += direction;
                    if (topPoint.IsInside() && !(this[topPoint] is EmptyPiece))
                    {
                        return topPoint;
                    }
                    if (bottomPoint.IsInside() && !(this[bottomPoint] is EmptyPiece))
                    {
                        return bottomPoint;
                    }
                }
                topPoint.X = currentPosition.X;
                topPoint.Y -= 1;
                bottomPoint.X = currentPosition.X;
                bottomPoint.Y += 1;
            }
            return currentPosition;
        }

        public Point currentPositionMoveHelper(Point direction)
        {
            if (direction.X != 0)
            {
                return xChangeCurrentPositionMoveHelper(direction);
            }
            else
            {
                return yChangeCurrentPositionMoveHelper(direction);
            }
            //Point newPosition = currentPosition;
            //newPosition += direction;
            //if (newPosition.Y > 7 || newPosition.X > 7 || newPosition.Y < 0 || newPosition.X < 0) return currentPosition;

            //while (GridSquares[newPosition.Y, newPosition.X] is EmptyPiece)
            //{
            //    newPosition += direction;
            //    if (newPosition.Y > 7 || newPosition.X > 7 || newPosition.Y < 0 || newPosition.X < 0) return currentPosition;
            //}
            //return newPosition;
        }

        public int moveMoveHelper(Point direction, int size)
        {
            if (currentMoveIndex + direction.X > size - 1)
            {
                currentMoveIndex = 0;
            }
            else if (currentMoveIndex + direction.X < 0)
            {
                currentMoveIndex = size - 1;
            }
            else
            {
                currentMoveIndex += direction.X;
            }
            return currentMoveIndex;
        }

        #endregion
    }
}
