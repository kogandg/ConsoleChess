using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        //private int squareHeight;
        //private int squareWidth;
        public Dictionary<char, string> FENToScreenOutput { get; private set; }
        private Dictionary<char, Func<PieceColors, Point, Piece>> fenToPiece;

        public int CurrentMoveIndex { get; private set; }
        public Point CurrentPosition { get; private set; }
        public bool ShowMoves { get; private set; }
        public bool IsPromoting { get; private set; }
        public int PromotionIndex { get; private set; }
        public char[] PromotionPieces { get; private set; }
        public bool Once;
        public bool HasPromoted { get; set; }

        public bool KCastle { get; private set; }
        public bool QCastle { get; private set; }
        public bool kCastle { get; private set; }
        public bool qCastle { get; private set; }

        private bool isCurrentMoveWhite;
        private bool justMoved;
        public Point EnPassantTargetSquare { get; private set; }
        public List<Point> DrawingPoints;
        public IVisualizer Visualizer { get; private set; }
        static int counter = 0;
        public ChessBoard(IVisualizer visualizer)//, int width, int height)
        {
            Visualizer = visualizer;
            GridSquares = new Piece[8, 8];
            //squareWidth = width;
            //squareHeight = height;

            FENToScreenOutput = new Dictionary<char, string>()
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

            fenToPiece = new Dictionary<char, Func<PieceColors, Point, Piece>>()
            {
                ['.'] = (PieceColors c, Point p) => new EmptyPiece(p, this),
                ['P'] = (PieceColors c, Point p) => new Pawn(c, p, this),
                ['R'] = (PieceColors c, Point p) => new Rook(c, p, this),
                ['N'] = (PieceColors c, Point p) => new Knight(c, p, this),
                ['B'] = (PieceColors c, Point p) => new Bishop(c, p, this),
                ['Q'] = (PieceColors c, Point p) => new Queen(c, p, this),
                ['K'] = (PieceColors c, Point p) => new King(c, p, this),
            };

            CurrentMoveIndex = 0;
            CurrentPosition = new Point(0, 0);

            isCurrentMoveWhite = true;
            EnPassantTargetSquare = new Point(-1, -1);

            PromotionIndex = 0;
            PromotionPieces = new char[4]
            {
                'R',
                'N',
                'B',
                'Q'
            };
            Once = false;
            HasPromoted = false;

            justMoved = false;
            DrawingPoints = new List<Point>();
        }

        public void DrawBoard()
        {
            Visualizer.DrawBoard(this);
        }


        #region FEN

        public string ToFEN()
        {
            StringBuilder FEN = new StringBuilder();
            int blankSpace = 0;
            for (int x = 7; x >= 0; x--)
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
            if (!KCastle && !QCastle && !kCastle && !qCastle) FEN.Append("-");
            else
            {
                FEN.Append(KCastle ? "K" : "");
                FEN.Append(QCastle ? "Q" : "");
                FEN.Append(kCastle ? "k" : "");
                FEN.Append(qCastle ? "q" : "");
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

        public void FromFEN(string fen)
        {
            string[] fenRows = fen.Split("/");
            int currentColumnValue;
            int currentFENRowValue;
            List<string> suffixes = new List<string>(fenRows[7].Split(" "));
            fenRows[7] = suffixes[0];
            suffixes.RemoveAt(0);

            for (int row = 0; row < fenRows.Length; row++)
            {
                currentFENRowValue = Math.Abs(row - 7);
                currentColumnValue = 0;
                for (int fenColumn = 0; fenColumn < fenRows[currentFENRowValue].Length; fenColumn++)
                {
                    if (char.IsDigit(fenRows[currentFENRowValue][fenColumn]))
                    {
                        int blankSpace = int.Parse(fenRows[currentFENRowValue][fenColumn].ToString());
                        for (int i = 0; i < blankSpace; i++)
                        {
                            GridSquares[row, currentColumnValue] = new EmptyPiece(new Point(currentColumnValue, row), this);
                            currentColumnValue++;
                        }
                    }
                    else
                    {
                        if (char.IsUpper(fenRows[currentFENRowValue][fenColumn]))
                        {
                            GridSquares[row, currentColumnValue] = fenToPiece[fenRows[currentFENRowValue][fenColumn]]?.Invoke(PieceColors.White, new Point(currentColumnValue, row));
                        }
                        else
                        {
                            GridSquares[row, currentColumnValue] = fenToPiece[fenRows[currentFENRowValue][fenColumn].ToString().ToUpper()[0]]?.Invoke(PieceColors.Black, new Point(currentColumnValue, row));
                        }
                        //GridSquares[row, currentColumnValue] = rows[row][fenColumn];
                        currentColumnValue++;
                    }
                }
            }
            ///////this line
            isCurrentMoveWhite = false;
            if (suffixes[0] == "w")
            {
                isCurrentMoveWhite = true;
            }

            KCastle = suffixes[1].Contains('K');
            QCastle = suffixes[1].Contains('Q');
            kCastle = suffixes[1].Contains('k');
            qCastle = suffixes[1].Contains('q');

            if (suffixes[2] != "-")
            {
                //EnPassantTargetSquare = new Point(suffixes[1][0] - 'a', suffixes[1][1] - 1);
                EnPassantTargetSquare = new Point(suffixes[2][0] - '0', suffixes[2][1] - '0');
            }
            DebugOut(suffixes, fen);
        }

        #endregion

        //#region DrawingBoard
        //public void DrawBoard()
        //{
        //    Console.OutputEncoding = System.Text.Encoding.Unicode;

        //    //Spaced out with outline

        //    for (int row = 0; row < 8; row++)
        //    {
        //        for (int column = 0; column < 8; column++)
        //        {
        //            Console.ForegroundColor = ConsoleColor.White;
        //            var output = GridSquares[row, column];
        //            //if (output.IsWhite())
        //            //{
        //            //    Console.ForegroundColor = ConsoleColor.DarkMagenta;
        //            //}
        //            //else
        //            //{
        //            //    Console.ForegroundColor = ConsoleColor.Blue;
        //            //}
        //            if (output.CurrentPosition == CurrentPosition)
        //            {
        //                Console.ForegroundColor = ConsoleColor.Red;
        //                if (ShowMoves)
        //                {
        //                    Console.ForegroundColor = ConsoleColor.Yellow;
        //                }
        //            }
        //            Console.SetCursorPosition(((column + 1) * squareWidth) - 4, ((row + 1) * squareHeight) - 1);
        //            Console.Write(FENToScreenOutput[output.FENNotation]);//fenToCentered[GridSquares[row, column]]);
        //        }
        //    }

        //    if (ShowMoves)
        //    {
        //        Console.ForegroundColor = ConsoleColor.Cyan;
        //        var moves = GridSquares[CurrentPosition.Y, CurrentPosition.X].PossibleMoves();
        //        for (int i = 0; i < moves.Count; i++)
        //        {
        //            Console.ForegroundColor = ConsoleColor.Cyan;
        //            Console.SetCursorPosition(((moves[i].X + 1) * squareWidth) - 4, ((moves[i].Y + 1) * squareHeight) - 1);
        //            //Console.SetCursorPosition(((0 + 1) * squareWidth) - 4, ((1 + 1) * squareHeight) - 1);
        //            if (i == CurrentMoveIndex)
        //            {
        //                Console.ForegroundColor = ConsoleColor.Red;
        //            }
        //            if (GridSquares[moves[i].Y, moves[i].X] is EmptyPiece)
        //            {
        //                Console.Write("..");
        //            }
        //            else
        //            {
        //                Console.Write(FENToScreenOutput[GridSquares[moves[i].Y, moves[i].X].FENNotation]);
        //            }
        //        }
        //    }

        //    if (IsPromoting)
        //    {
        //        Point startingPoint = new Point(60, 6);
        //        DrawPromotionMenuOutline(startingPoint);
        //        for (int i = 0; i < 4; i++)
        //        {
        //            Console.ForegroundColor = ConsoleColor.White;
        //            Console.SetCursorPosition(startingPoint.X + 3, startingPoint.Y + (squareHeight * i) + 2);
        //            if (i == PromotionIndex)
        //            {
        //                Console.ForegroundColor = ConsoleColor.Red;
        //            }
        //            Console.Write(FENToScreenOutput[PromotionPieces[i]]);
        //        }
        //    }
        //    if (HasPromoted)
        //    {
        //        ClearPromotion(new Point(60, 6));
        //        HasPromoted = false;
        //    }
        //    return;
        //    #region SmallBoard

        //    //smallMode

        //    for (int row = 0; row < 8; row++)
        //    {
        //        for (int column = 0; column < 8; column++)
        //        {
        //            Console.ForegroundColor = ConsoleColor.White;
        //            if (GridSquares[row, column].FENNotation == '.')
        //            {
        //                Console.ForegroundColor = ConsoleColor.DarkGray;
        //            }
        //            if (CurrentPosition.Y == row && CurrentPosition.X == column)
        //            {
        //                Console.ForegroundColor = ConsoleColor.Red;
        //            }

        //            Console.SetCursorPosition(column * 2, row);
        //            Console.Write(GridSquares[row, column].FENNotation);
        //        }
        //    }

        //    if (ShowMoves)
        //    {
        //        Console.ForegroundColor = ConsoleColor.Cyan;
        //        var moves = GridSquares[CurrentPosition.Y, CurrentPosition.X].PossibleMoves();
        //        for (int i = 0; i < moves.Count; i++)
        //        {
        //            Console.ForegroundColor = ConsoleColor.Cyan;
        //            if (i == CurrentMoveIndex)
        //            {
        //                Console.ForegroundColor = ConsoleColor.Red;
        //            }
        //            Console.SetCursorPosition(moves[i].X * 2, moves[i].Y);
        //            Console.Write('.');
        //        }
        //    }
        //    #endregion
        //}
        //void DrawGridOutline()
        //{
        //    Console.ForegroundColor = ConsoleColor.White;
        //    Console.OutputEncoding = System.Text.Encoding.UTF8;
        //    for (int y = 0; y < (8 * squareHeight) + 1; y++)
        //    {
        //        for (int x = 0; x < (8 * squareWidth) + 1; x++)
        //        {
        //            if (y % squareHeight == 0 && x != 0 && x != (8 * squareWidth))
        //            {
        //                Console.SetCursorPosition(x, y);
        //                Console.Write('_');//\u035F');
        //            }
        //            if (x % squareWidth == 0 && y != 0)
        //            {
        //                Console.SetCursorPosition(x, y);
        //                Console.Write('|');
        //            }
        //        }
        //    }
        //}

        //void DrawPromotionMenuOutline(Point startingPoint)
        //{
        //    Console.ForegroundColor = ConsoleColor.White;
        //    Console.OutputEncoding = System.Text.Encoding.UTF8;
        //    for (int y = 0; y < (4 * squareHeight) + 1; y++)
        //    {
        //        for (int x = 0; x < (1 * squareWidth) + 1; x++)
        //        {
        //            if (y % squareHeight == 0 && x != 0 && x != (4 * squareWidth))
        //            {
        //                Console.SetCursorPosition(x + startingPoint.X, y + startingPoint.Y);
        //                Console.Write('_');//\u035F');
        //            }
        //            if (x % squareWidth == 0 && y != 0)
        //            {
        //                Console.SetCursorPosition(x + startingPoint.X, y + startingPoint.Y);
        //                Console.Write('|');
        //            }
        //        }
        //    }
        //}

        //void ClearPromotion(Point startingPoint)
        //{
        //    for (int y = 0; y < (4 * squareHeight) + 1; y++)
        //    {
        //        for (int x = 0; x < (1 * squareWidth) + 1; x++)
        //        {
        //            Console.SetCursorPosition(x + startingPoint.X, y + startingPoint.Y);
        //            Console.Write(" ");
        //        }
        //    }
        //}
        //#endregion

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

        public void Update(KeyPressed keyPressed)//need to make an input manager that works for both console and mono
        {
            if (InCheck())
            {
                ;
            }
            if (InCheckMate())
            {
                ;
            }
            if (ShowMoves && !IsPromoting)
            {
                var moves = GridSquares[CurrentPosition.Y, CurrentPosition.X].AllowedMoves();
                //if(this[CurrentPosition] is King)
                //{
                //    moves = ((King)this[CurrentPosition]).AllowedMoves();
                //}
                //for (int i = 0; i < moves.Count; i++)
                //{
                //    var temp = moves[i];
                //    MovePiece(CurrentPosition, temp);
                //    if (WhiteCheckCheck())
                //    {
                //        moves.RemoveAt(i);
                //        i--;
                //    }
                //    MovePiece(temp, CurrentPosition);
                //}
                //DrawingPoints = moves;
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
                    ShowMoves = false;
                    CurrentMoveIndex = 0;
                }
                if (keyPressed == KeyPressed.Enter)
                {

                    ShowMoves = false;
                    if (moves.Count != 0)
                    {
                        Point originalPosition = CurrentPosition;

                        Point prevPawnMove = new Point(-1, -1);
                        if (this[CurrentPosition] is Pawn)
                        {
                            prevPawnMove = CurrentPosition;
                        }


                        Point currentMove = moves[CurrentMoveIndex];

                        GridSquares[currentMove.Y, currentMove.X] = GridSquares[CurrentPosition.Y, CurrentPosition.X];
                        GridSquares[CurrentPosition.Y, CurrentPosition.X] = new EmptyPiece(CurrentPosition, this);
                        GridSquares[currentMove.Y, currentMove.X].CurrentPosition = currentMove;
                        CurrentMoveIndex = 0;
                        CurrentPosition = currentMove;

                        if (InCheck())
                        {
                            ;
                        }
                        if (InCheckMate())
                        {
                            ;
                        }

                        if (currentMove == EnPassantTargetSquare)
                        {
                            if (this[currentMove].IsWhite())
                            {
                                GridSquares[currentMove.Y - 1, currentMove.X] = fenToPiece['.']?.Invoke(PieceColors.None, new Point(currentMove.Y - 1, currentMove.X));
                            }
                            else
                            {
                                GridSquares[currentMove.Y + 1, currentMove.X] = fenToPiece['.']?.Invoke(PieceColors.None, new Point(currentMove.Y + 1, currentMove.X));
                            }
                        }

                        if (prevPawnMove.IsInside() && CurrentPosition.Y - 2 == prevPawnMove.Y)
                        {
                            EnPassantTargetSquare = prevPawnMove;
                            EnPassantTargetSquare.Y++;
                        }
                        else if (prevPawnMove.IsInside() && CurrentPosition.Y + 2 == prevPawnMove.Y)
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
                                IsPromoting = true;
                            }
                        }

                        if (this[currentMove] is King)
                        {
                            if (currentMove.X == 6)
                            {
                                if (this[currentMove].IsWhite() && KCastle)
                                {
                                    MovePiece(new Point(7, 0), new Point(5, 0));
                                }
                                else if (kCastle)
                                {
                                    MovePiece(new Point(7, 7), new Point(5, 7));
                                }
                            }
                            else if (currentMove.X == 2)
                            {
                                if (this[currentMove].IsWhite() && QCastle)
                                {
                                    MovePiece(new Point(0, 0), new Point(3, 0));
                                }
                                else if (qCastle)
                                {
                                    MovePiece(new Point(0, 7), new Point(3, 7));
                                }
                            }
                        }

                        isCurrentMoveWhite = !isCurrentMoveWhite;
                        justMoved = true;
                    }
                }
            }
            else if (!IsPromoting)
            {
                Point tempPoint;
                if (keyPressed == KeyPressed.Left)
                {
                    ShowMoves = false;
                    tempPoint = currentPositionMoveHelper(new Point(-1, 0), isCurrentMoveWhite);
                    if (this[tempPoint].IsWhite() == isCurrentMoveWhite)
                    {
                        CurrentPosition = tempPoint;
                    }
                    justMoved = false;
                    //currentPosition = tempPoint;
                }
                else if (keyPressed == KeyPressed.Right)
                {
                    ShowMoves = false;
                    tempPoint = currentPositionMoveHelper(new Point(1, 0), isCurrentMoveWhite);
                    if (this[tempPoint].IsWhite() == isCurrentMoveWhite)
                    {
                        CurrentPosition = tempPoint;
                    }
                    justMoved = false;
                    //currentPosition = tempPoint;
                }
                else if (keyPressed == KeyPressed.Up)
                {
                    ShowMoves = false;
                    tempPoint = currentPositionMoveHelper(new Point(0, 1), isCurrentMoveWhite);//currentPositionMoveHelper(new Point(0, -1));
                    if (this[tempPoint].IsWhite() == isCurrentMoveWhite)
                    {
                        CurrentPosition = tempPoint;
                    }
                    justMoved = false;
                    //currentPosition = tempPoint;
                }
                else if (keyPressed == KeyPressed.Down)
                {
                    ShowMoves = false;
                    tempPoint = currentPositionMoveHelper(new Point(0, -1), isCurrentMoveWhite);//currentPositionMoveHelper(new Point(0, 1));
                    if (this[tempPoint].IsWhite() == isCurrentMoveWhite)
                    {
                        CurrentPosition = tempPoint;
                    }
                    justMoved = false;
                    //currentPosition = tempPoint;
                }
                else if (keyPressed == KeyPressed.Enter && !justMoved)
                {
                    ShowMoves = true;

                }
            }




            if (IsPromoting)
            {
                if (keyPressed == KeyPressed.Up && PromotionIndex > 0)
                {
                    PromotionIndex--;
                }
                if (keyPressed == KeyPressed.Down && PromotionIndex < 4)
                {
                    PromotionIndex++;
                }
                if (keyPressed == KeyPressed.Enter && Once)
                {
                    GridSquares[CurrentPosition.Y, CurrentPosition.X] = fenToPiece[PromotionPieces[PromotionIndex]](this[CurrentPosition].PieceColor, CurrentPosition);
                    PromotionIndex = 0;
                    IsPromoting = false;
                    Once = false;
                    HasPromoted = true;
                }
                Once = true;
            }

            if (!(GridSquares[0, 0] is Rook))
            {
                QCastle = false;
            }
            if (!(GridSquares[0, 7] is Rook))
            {
                KCastle = false;
            }
            if (!(GridSquares[7, 0] is Rook))
            {
                qCastle = false;
            }
            if (!(GridSquares[7, 7] is Rook))
            {
                kCastle = false;
            }
            if (!(GridSquares[0, 4] is King))
            {
                KCastle = false;
                QCastle = false;
            }
            if (!(GridSquares[7, 4] is King))
            {
                kCastle = false;
                qCastle = false;
            }
            if (InCheck())
            {
                ;
            }
            if (InCheckMate())
            {
                throw new Exception((isCurrentMoveWhite ? "white" : "black") + " is bad at chess");
            }
            if (inStaleMate())
            {
                throw new Exception("stalemate");
            }
            if (draw())
            {
                throw new Exception("draw by insufficient material");
            }
            Console.SetCursorPosition(10, 26);
            Console.Write(CurrentPosition.X);
            Console.SetCursorPosition(12, 26);
            Console.WriteLine(CurrentPosition.Y);
        }


        #region MoveHelpers

        public Point yChangeCurrentPositionMoveHelper(Point direction, bool isWhiteTurn)
        {
            Point leftPoint = CurrentPosition;
            Point rightPoint = CurrentPosition;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    leftPoint += direction;
                    rightPoint += direction;
                    if (leftPoint.IsInside() && !(this[leftPoint] is EmptyPiece) && this[leftPoint].IsWhite() == isWhiteTurn)
                    {
                        return leftPoint;
                    }
                    if (rightPoint.IsInside() && !(this[rightPoint] is EmptyPiece) && this[rightPoint].IsWhite() == isWhiteTurn)
                    {
                        return rightPoint;
                    }
                }
                leftPoint.Y = CurrentPosition.Y;
                leftPoint.X -= 1;
                rightPoint.Y = CurrentPosition.Y;
                rightPoint.X += 1;
            }
            return CurrentPosition;
        }

        public Point xChangeCurrentPositionMoveHelper(Point direction, bool isWhiteTurn)
        {
            Point topPoint = CurrentPosition;
            Point bottomPoint = CurrentPosition;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    topPoint += direction;
                    bottomPoint += direction;
                    if (topPoint.IsInside() && !(this[topPoint] is EmptyPiece) && this[topPoint].IsWhite() == isWhiteTurn)
                    {
                        return topPoint;
                    }
                    if (bottomPoint.IsInside() && !(this[bottomPoint] is EmptyPiece) && this[bottomPoint].IsWhite() == isWhiteTurn)
                    {
                        return bottomPoint;
                    }
                }
                topPoint.X = CurrentPosition.X;
                topPoint.Y -= 1;
                bottomPoint.X = CurrentPosition.X;
                bottomPoint.Y += 1;
            }
            return CurrentPosition;
        }

        public Point currentPositionMoveHelper(Point direction, bool isWhiteTurn)
        {
            if (direction.X != 0)
            {
                return xChangeCurrentPositionMoveHelper(direction, isWhiteTurn);
            }
            else
            {
                return yChangeCurrentPositionMoveHelper(direction, isWhiteTurn);
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
            if (CurrentMoveIndex + direction.X > size - 1)
            {
                CurrentMoveIndex = 0;
            }
            else if (CurrentMoveIndex + direction.X < 0)
            {
                CurrentMoveIndex = size - 1;
            }
            else
            {
                CurrentMoveIndex += direction.X;
            }
            return CurrentMoveIndex;
        }

        #endregion

        void DebugOut(List<string> suffixes, string fen)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(0, 26);
            Console.Write("                 ");
            Console.SetCursorPosition(0, 26);
            Console.Write(suffixes[0]);
            Console.SetCursorPosition(0, 27);
            Console.Write("                 ");
            Console.SetCursorPosition(0, 27);
            Console.Write(suffixes[1]);
            Console.SetCursorPosition(0, 28);
            Console.Write("                 ");
            Console.SetCursorPosition(0, 28);
            Console.WriteLine(suffixes[2]);
            Console.SetCursorPosition(0, 29);
            Console.Write("                                                                   ");
            Console.SetCursorPosition(0, 29);
            Console.WriteLine(fen);
        }

        public void MovePiece(Point from, Point to)
        {
            Point p = this[from].CurrentPosition;
            this[from].CurrentPosition = this[to].CurrentPosition;
            this[to].CurrentPosition = p;
            Piece temp = this[from];
            GridSquares[from.Y, from.X] = this[to];
            GridSquares[to.Y, to.X] = temp;
            counter++;
        }

        bool draw()
        {
            var blackPieces = GetPieces(false);
            var whitePieces = GetPieces(true);

            if (blackPieces.Count > 2 || blackPieces.Count > 2) return false;

            if (blackPieces.Count == 1 && whitePieces.Count == 1) return true;

            Piece blackBishop = null;
            Piece whiteBishop = null;
            Piece blackKnight = null;
            Piece whiteKnight = null;
            foreach (Piece piece in blackPieces)
            {
                if (piece is Bishop)
                {
                    blackBishop = piece;
                }
                if (piece is Knight)
                {
                    blackKnight = piece;
                }
            }
            foreach (Piece piece in whitePieces)
            {
                if (piece is Bishop)
                {
                    whiteBishop = piece;
                }
                if (piece is Knight)
                {
                    whiteKnight = piece;
                }
            }

            if (whitePieces.Count == 1 && (blackKnight != null || blackBishop != null)) return true;
            if (blackPieces.Count == 1 && (whiteKnight != null || whiteBishop != null)) return true;

            if (blackBishop != null && whiteBishop != null)
            {
                if (blackBishop.IsOnWhite() == whiteBishop.IsOnWhite()) return true;
            }
            if (whiteKnight != null ^ blackKnight != null) return true;

            return false;
        }

        bool inStaleMate()
        {
            if (InCheck()) return false;
            var pieces = GetCurrentPieces();

            foreach (var piece in pieces)
            {
                if (piece.AllowedMoves().Count != 0)
                {
                    return false;
                }
            }
            return true;
        }

        List<Piece> GetCurrentPieces()
        {
            List<Piece> pieces = new List<Piece>();
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    Piece current = GridSquares[y, x];
                    if (!(current is EmptyPiece) && current.IsWhite() == isCurrentMoveWhite)
                    {
                        pieces.Add(current);
                    }
                }
            }
            return pieces;
        }

        List<Piece> GetPieces(bool isWhite)
        {
            List<Piece> pieces = new List<Piece>();
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    Piece current = GridSquares[y, x];
                    if (!(current is EmptyPiece) && current.IsWhite() == isWhite)
                    {
                        pieces.Add(current);
                    }
                }
            }
            return pieces;
        }

        public bool InCheck(Piece piece)
        {
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {

                    Piece current = GridSquares[y, x];
                    //if (current is King && x == 3 && y == 6)
                    //{
                    //   // Visualizer.DrawBoard(this);
                    //    ;
                    //}
                    if (current == piece /*|| current.IsWhite() == isCurrentMoveWhite*/) continue;

                    var moves = current.PossibleMoves();
                    for (int i = 0; i < moves.Count; i++)
                    {
                        var currentMove = moves[i];
                        if (GridSquares[currentMove.Y, currentMove.X] is King)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }


        public bool InCheck()
        {
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    Piece current = GridSquares[y, x];
                    //if (current.IsWhite() == isCurrentMoveWhite) continue;

                    var moves = current.PossibleMoves();
                    for (int i = 0; i < moves.Count; i++)
                    {
                        var currentMove = moves[i];
                        if (GridSquares[currentMove.Y, currentMove.X] is King)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }



        public bool InCheckMate()
        {

            if (InCheck())
            {
                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        Piece current = GridSquares[y, x];
                        if (!(current is EmptyPiece) && current.IsWhite() == isCurrentMoveWhite)//!(current is King))
                        {
                            if (current.CurrentPosition.X == 4 && current.CurrentPosition.Y == 0)
                            {
                                ;
                            }
                            if (current.AllowedMoves().Count > 0)
                            {
                                return false;
                            }
                        }

                    }
                }
                return true;
            }
            return false;
        }

        //public bool CheckCheck(bool isWhite)
        //{
        //    int k = 0; 
        //    for (int y = 0; y < 8; y++)
        //    {
        //        for (int x = 0; x < 8; x++)
        //        {
        //            if(k == 9)
        //            {
        //                ;
        //            }
        //            var moves = GridSquares[y, x].PossibleMoves();
        //            Debug.WriteLine(moves.Count);
        //            for (int i = 0; i < moves.Count; i++)
        //            {
        //                if (this[moves[i]] is King && this[moves[i]].IsWhite() != isWhite)
        //                {
        //                    return true;// throw new Exception("White Check");
        //                }
        //            }
        //            k++;
        //        }
        //    }
        //    return false;
        //}

        //public bool CheckPieceCheck(Piece piece)
        //{
        //    for (int y = 0; y < 8; y++)
        //    {
        //        for (int x = 0; x < 8; x++)
        //        {
        //            if (GridSquares[y, x] == piece || GridSquares[y, x].IsWhite() == isCurrentMoveWhite)
        //            {
        //                ;
        //                continue;
        //            }
        //            var moves = GridSquares[y, x].PossibleMoves();
        //            for (int i = 0; i < moves.Count; i++)
        //            {
        //                if (this[moves[i]] is King && this[moves[i]].IsWhite() == isCurrentMoveWhite)
        //                {
        //                    return true;// throw new Exception("White Check");
        //                }
        //            }
        //        }
        //    }
        //    return false;
        //}
    }
}
