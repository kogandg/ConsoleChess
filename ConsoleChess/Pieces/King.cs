using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleChess.Pieces
{
    class King : Piece
    {
        struct CastleMoves
        {
            public static int[] QueenSide = new int[] { 1, 2, 3 };
            public static int[] KingSide = new int[] { 5, 6 };

            public static int QueenSidePlacement = 2;
            public static int KingSidePlacement = 6;

            public static int WhiteRow = 0;
            public static int BlackRow = 7;
        }

        public King(PieceColors color, Point currentPosition, ChessBoard board) : base(color, currentPosition, board)
        {
            if (IsWhite())
            {
                FENNotation = 'K';
            }
            else
            {
                FENNotation = 'k';
            }
        }
        public override List<Point> PossibleMoves()
        {
            List<Point> moves = new List<Point>();
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    Point possiblePoint = CurrentPosition + new Point(x, y);
                    if (possiblePoint.IsInside() && possiblePoint != CurrentPosition && (owningBoard[possiblePoint].IsWhite() != IsWhite() || owningBoard[possiblePoint] is EmptyPiece))
                    {
                        moves.Add(possiblePoint);
                    }
                }
            }

            

            CastleCheck(moves);

            //for (int i = 0; i < moves.Count; i++)
            //{
            //    var tempMove = moves[i];
            //    var originalPosition = CurrentPosition;
            //    owningBoard.MovePiece(CurrentPosition, tempMove);
            //    if (owningBoard.CheckPieceCheck(this))
            //    {
            //        moves.RemoveAt(i);
            //        i--;
            //    }
            //    owningBoard.MovePiece(tempMove, originalPosition);
            //}

            return moves;
        }

        //public List<Point> AllowedMoves()
        //{
        //    var moves = PossibleMoves();
        //    for (int i = 0; i < moves.Count; i++)
        //    {
        //        var tempMove = moves[i];
        //        var originalPosition = CurrentPosition;
        //        owningBoard.MovePiece(CurrentPosition, tempMove);
        //        if(owningBoard.InCheck(this))
        //        {
        //            moves.RemoveAt(i);
        //            i--;
        //        }
        //        owningBoard.MovePiece(tempMove, originalPosition);
        //    }
        //    return moves;
        //}

        private void CastleCheck(List<Point> moves)
        {
            int[] queenSide;
            int[] kingSide;
            int row;

            bool isQueenSideAllowed = true;
            bool isKingSideAllowed = true;

            if (IsWhite())
            {
                isQueenSideAllowed = owningBoard.QCastle;
                isKingSideAllowed = owningBoard.KCastle;
                row = CastleMoves.WhiteRow;
            }
            else
            {
                isQueenSideAllowed = owningBoard.qCastle;
                isKingSideAllowed = owningBoard.kCastle;
                row = CastleMoves.BlackRow;
            }

            queenSide = isQueenSideAllowed ? CastleMoves.QueenSide : Array.Empty<int>();
            kingSide = isKingSideAllowed ? CastleMoves.KingSide : Array.Empty<int>();

            isQueenSideAllowed = CheckSide(owningBoard, row, queenSide);
            isKingSideAllowed = CheckSide(owningBoard, row, kingSide);

            
            if(isQueenSideAllowed)
            {
                moves.Add(new Point(CastleMoves.QueenSidePlacement, row));
            }
            if(isKingSideAllowed)
            {
                moves.Add(new Point(CastleMoves.KingSidePlacement, row));
            }


            static bool CheckSide(ChessBoard board, int row, int[] moves)
            {
                if (moves.Length == 0) return false;

                for (int i = 0; i < moves.Length; i++)
                {
                    if (board[row, moves[i]] is EmptyPiece) continue;

                    return false;
                }

                return true;
            }
        }
    }
}
