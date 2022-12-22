using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        int direction = (team == 0) ? 1 : -1; //up if white, down if black

        //One in front
        if(board[currentX,currentY + direction] == null)
        {
            r.Add(new Vector2Int(currentX, currentY + direction));
        }

        //Two in front (in initial position)
        if(board[currentX,currentY + direction] == null)
        {
            //Team white direction
            if(team == 0 && currentY ==1 && board[currentX, currentY + (direction *2)] == null)
            {
                r.Add(new Vector2Int(currentX, currentY + (direction * 2)));
            }

            //Team black direction
            if (team == 1 && currentY == 6 && board[currentX, currentY + (direction * 2)] == null)
            {
                r.Add(new Vector2Int(currentX, currentY + (direction * 2)));
            }
        }

        //Kill Move
        if(currentX != tileCountX - 1) //if not on the most right side of the board
        {
            if(board[currentX + 1, currentY+direction]!=null && board[currentX + 1, currentY + direction].team != team)
            {
                r.Add(new Vector2Int(currentX + 1, currentY + direction));
            }
        }
        if (currentX != 0) //if not on the most left side of the board
        {
            if (board[currentX - 1, currentY + direction] != null && board[currentX - 1, currentY + direction].team != team)
            {
                r.Add(new Vector2Int(currentX - 1, currentY + direction));
            }
        }

        return r;
    }

    public override BoardManager.SpecialMove GetSpecialMoves(ref ChessPiece[,] board, ref List<Vector2Int> availableMoves, ref List<Vector2Int[]> moveList)
    {
        int direction = (team == 0) ? 1 : -1;

        //Promotion
        //check pawn team and the position of the pawn
        if((team == 0 && currentY ==6) || (team == 1 && currentY == 1))
        {
            return BoardManager.SpecialMove.Promotion;
        }

        //En passant
        //if there is already a move on the game
        if (moveList.Count > 0)
        {
            var lastMove = moveList[moveList.Count - 1];
            //if lastMove done by a pawn
            if(board[lastMove[1].x, lastMove[1].y].type == ChessPieceType.Pawn)
            {
                if(Mathf.Abs(lastMove[0].y - lastMove[1].y) == 2) //if the moved pawn at least move 2 units
                {
                    if(board[lastMove[1].x, lastMove[1].y].team != team) //if the move was from the other team
                    {
                        if(lastMove[1].y == currentY) //if the opponet team pawn lands exactly to my right or my left
                        {
                            //left
                            if(lastMove[1].x == currentX - 1)
                            {
                                availableMoves.Add(new Vector2Int(currentX - 1, currentY + direction));
                                return BoardManager.SpecialMove.EnPassant;
                            }

                            //right
                            if(lastMove[1].x == currentX + 1)
                            {
                                availableMoves.Add(new Vector2Int(currentX + 1, currentY + direction));
                                return BoardManager.SpecialMove.EnPassant;
                            }
                        }
                    }
                }
            }
        }

        return BoardManager.SpecialMove.None;
    }
}
