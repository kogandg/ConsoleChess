using ConsoleChess.Pieces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleChess
{
    public class ConsoleChessInputManager : InputManager<Point>
    {
        public List<Point> Moves { get; set; }
        int movesIndex;

        public ConsoleChessInputManager(Func<(Point, bool), bool> validate) : base(validate)
        {
            Moves = new List<Point>();
        }

        public override (Point square, bool isSelected) GetInput(Point current)
        {
            var keyPressed = Console.ReadKey(true).Key;
            bool isSelected = keyPressed == ConsoleKey.Enter;

            if (Moves.Count == 0)
            {
                if (isSelected)
                {
                    movesIndex = 0;
                    return (current, true);
                }

                switch (keyPressed)
                {
                    case ConsoleKey.LeftArrow:
                        return (currentPositionMoveHelper(current, new Point(-1, 0)), false);
                        break;

                    case ConsoleKey.RightArrow:
                        return (currentPositionMoveHelper(current, new Point(1, 0)), false);
                        break;

                    case ConsoleKey.UpArrow:
                        return (currentPositionMoveHelper(current, new Point(0, 1)), false);
                        break;

                    case ConsoleKey.DownArrow:
                        return (currentPositionMoveHelper(current, new Point(0, -1)), false);
                        break;

                    default:
                        return (current, false);
                }
            }
            else
            {
                if(isSelected)
                {
                    var move = Moves[movesIndex];
                    Moves.Clear();
                    movesIndex = 0;
                    return (move, true);
                }

                switch(keyPressed)
                {
                    case ConsoleKey.RightArrow:
                        moveMoveHelper(new Point(1, 0), Moves.Count);
                        return (Moves[movesIndex], false);
                        break;

                    case ConsoleKey.LeftArrow:
                        moveMoveHelper(new Point(-1, 0), Moves.Count);
                        return (Moves[movesIndex], false);
                        break;

                    case ConsoleKey.Escape:
                        Moves.Clear();
                        return (null, true);
                        break;

                    default:
                        return (Moves[movesIndex], false);
                }
            }

        }

        #region MoveHelpers

        public Point yChangeCurrentPositionMoveHelper(Point current, Point direction)
        {
            Point leftPoint = current;
            Point rightPoint = current;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    leftPoint += direction;
                    rightPoint += direction;
                    if (Validate((leftPoint, false)))//leftPoint.IsInside() && !(this[leftPoint] is EmptyPiece) && this[leftPoint].IsWhite() == isWhiteTurn)
                    {
                        return leftPoint;
                    }
                    if (Validate((rightPoint, false)))//rightPoint.IsInside() && !(this[rightPoint] is EmptyPiece) && this[rightPoint].IsWhite() == isWhiteTurn)
                    {
                        return rightPoint;
                    }
                }
                leftPoint.Y = current.Y;
                leftPoint.X -= 1;
                rightPoint.Y = current.Y;
                rightPoint.X += 1;
            }
            return current;
        }

        Point xChangeCurrentPositionMoveHelper(Point current, Point direction)
        {
            Point topPoint = current;
            Point bottomPoint = current;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    topPoint += direction;
                    bottomPoint += direction;
                    if (Validate((topPoint, false)))//topPoint.IsInside() && !(this[topPoint] is EmptyPiece) && this[topPoint].IsWhite() == isWhiteTurn)
                    {
                        return topPoint;
                    }
                    if (Validate((bottomPoint, false)))//bottomPoint.IsInside() && !(this[bottomPoint] is EmptyPiece) && this[bottomPoint].IsWhite() == isWhiteTurn)
                    {
                        return bottomPoint;
                    }
                }
                topPoint.X = current.X;
                topPoint.Y -= 1;
                bottomPoint.X = current.X;
                bottomPoint.Y += 1;
            }
            return current;
        }

        Point currentPositionMoveHelper(Point current, Point direction)
        {
            if (direction.X != 0)
            {
                return xChangeCurrentPositionMoveHelper(current, direction);
            }
            else
            {
                return yChangeCurrentPositionMoveHelper(current, direction);
            }
        }


        int moveMoveHelper(Point direction, int size)
        {
            if (movesIndex + direction.X > size - 1)
            {
                movesIndex = 0;
            }
            else if (movesIndex + direction.X < 0)
            {
                movesIndex = size - 1;
            }
            else
            {
                movesIndex += direction.X;
            }
            return movesIndex;
        }
        #endregion

    }
}
