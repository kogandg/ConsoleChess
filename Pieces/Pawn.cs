using System;
using System.Collections.Generic;
using System.Text;
using PointLibrary;
using Interfaces;

namespace Pieces
{
    public class Pawn : Piece
    {
        public Pawn(PieceColors color, Point currentPosition, IChessBoard<Piece> board) : base(color, currentPosition, board)
        {
            if (IsWhite())
            {
                FENNotation = 'P';
            }
            else
            {
                FENNotation = 'p';
            }
        }
        public override List<Point> PossibleMoves()
        {
            List<Point> moves = new List<Point>();
            if (IsWhite())
            {
                //promotion thing
                if (CurrentPosition.Y == 7)
                {
                    //owningBoard[CurrentPosition] = new Queen(IsWhite, CurrentPosition);
                }

                else
                {
                    Piece rightTarget = (Piece)owningBoard[CurrentPosition.Y + 1, CurrentPosition.X + 1];
                    Piece leftTarget = (Piece)owningBoard[CurrentPosition.Y + 1, CurrentPosition.X - 1];

                    if (owningBoard[CurrentPosition.Y + 1, CurrentPosition.X] is EmptyPiece)
                    {
                        moves.Add(new Point(CurrentPosition.X, CurrentPosition.Y + 1));
                        if (CurrentPosition.Y == 1 && owningBoard[CurrentPosition.Y + 2, CurrentPosition.X] is EmptyPiece)
                        {
                            moves.Add(new Point(CurrentPosition.X, CurrentPosition.Y + 2));
                        }
                    }

                    if (rightTarget != null && ((!(rightTarget is EmptyPiece) && !rightTarget.IsWhite()) || (rightTarget.CurrentPosition == owningBoard.EnPassantTargetSquare)))
                    {
                        moves.Add(rightTarget.CurrentPosition);
                    }
                    if (leftTarget != null && ((!(leftTarget is EmptyPiece) && !leftTarget.IsWhite()) || (leftTarget.CurrentPosition == owningBoard.EnPassantTargetSquare)))
                    {
                        moves.Add(leftTarget.CurrentPosition);
                    }
                }
            }
            else
            {
                Piece rightTarget = (Piece)owningBoard[CurrentPosition.Y - 1, CurrentPosition.X + 1];
                Piece leftTarget = (Piece)owningBoard[CurrentPosition.Y - 1, CurrentPosition.X - 1];

                if (owningBoard[CurrentPosition.Y - 1, CurrentPosition.X] is EmptyPiece)
                {
                    moves.Add(new Point(CurrentPosition.X, CurrentPosition.Y - 1));
                    if (CurrentPosition.Y == 6 && owningBoard[CurrentPosition.Y - 2, CurrentPosition.X] is EmptyPiece)
                    {
                        moves.Add(new Point(CurrentPosition.X, CurrentPosition.Y - 2));
                    }
                }


                if (rightTarget != null && ((!(rightTarget is EmptyPiece) && rightTarget.IsWhite()) || (rightTarget.CurrentPosition == owningBoard.EnPassantTargetSquare)))
                {
                    moves.Add(rightTarget.CurrentPosition);
                }
                if (leftTarget != null && ((!(leftTarget is EmptyPiece) && leftTarget.IsWhite()) || (leftTarget.CurrentPosition == owningBoard.EnPassantTargetSquare)))
                {
                    moves.Add(leftTarget.CurrentPosition);
                }
            }
            for (int i = 0; i < moves.Count; i++)
            {
                Point move = moves[i];
                if (move.X < 0 || move.X > 7 || move.Y < 0 || move.Y > 7)
                {
                    moves.Remove(move);
                    i--;
                }
            }

            //for(int i = 0; i < moves.Count; i++)
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
    }
}
