using System;
using System.Collections.Generic;
using System.Text;
using Models;

namespace PointLibrary
{
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
        public Point(Point p)
        {
            X = p.X;
            Y = p.Y;
        }
        public static Point operator +(Point pt, Point p)
        {
            return new Point(pt.X + p.X, pt.Y + p.Y);
        }
        public static bool operator ==(Point left, Point right)
        {
            if(left is null)
            {
                return right is null;
            }
            if(right is null)
            {
                return false;
            }
            return (left.X == right.X && left.Y == right.Y);
        }
        public static bool operator !=(Point left, Point right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Point)) return false;
            Point comp = (Point)obj;
            return comp.X == this.X && comp.Y == this.Y;
        }
        public override int GetHashCode()
        {
            return unchecked(X ^ Y);
        }

        public bool IsInside()
        { 
            return X >= 0 && X <= 7 && Y >= 0 && Y <= 7;
        }

        public bool IsAtEnd()
        {
            return Y == 0 || Y == 7;
        }

        public static implicit operator Point(ChessSquareModel square)
        {
            return new Point(square.Col, square.Row);
        }
        public static implicit operator ChessSquareModel(Point point)
        {
            return new ChessSquareModel { Row = point.Y, Col = point.X };
        }
    }
}
