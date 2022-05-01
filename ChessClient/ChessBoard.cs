using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;
using Pieces;
using PointLibrary;

namespace ChessClient
{
    public class ChessBoard : IChessBoard<Piece>
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

        public Point EnPassantTargetSquare { get; private set; }

        public bool KCastle { get; private set; }
        public bool kCastle { get; private set; }
        public bool QCastle { get; private set; }
        public bool qCastle { get; private set; }


        public bool ShowMoves { get; private set; }

        public Dictionary<char, string> FENToScreenOutput { get; private set; }
        private Dictionary<char, Func<PieceColors, Point, Piece>> fenToPiece { get; set; }

        public bool IsPromoting { get; private set; }
        public int PromotionIndex { get; private set; }
        public char[] PromotionPieces { get; private set; }
        public bool Once;
        public bool HasPromoted { get; set; }

        public bool IsCurrentMoveWhite { get; private set; }

        public ChessBoard()
        {
            GridSquares = new Piece[8, 8];

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
            if (IsCurrentMoveWhite)
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

            IsCurrentMoveWhite = false;
            if (suffixes[0] == "w")
            {
                IsCurrentMoveWhite = true;
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
            //DebugOut(suffixes, fen);
        }

        #endregion

        public bool InCheck(Piece piece)
        {
            throw new NotImplementedException();
        }
    }
}
