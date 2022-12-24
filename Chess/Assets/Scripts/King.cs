using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        //Right
        if(currentX + 1 < tileCountX)
        {
            //Right
            if(board[currentX + 1, currentY] == null)
            {
                r.Add(new Vector2Int(currentX + 1, currentY));
            }
            else if(board[currentX + 1, currentY].team != team)
            {
                r.Add(new Vector2Int(currentX + 1, currentY));
            }

            //Diagonal top right
            if(currentY + 1 < tileCountY)
            {
                if (board[currentX + 1, currentY + 1] == null)
                {
                    r.Add(new Vector2Int(currentX + 1, currentY + 1));
                }
                else if (board[currentX + 1, currentY + 1].team != team)
                {
                    r.Add(new Vector2Int(currentX + 1, currentY + 1));
                }
            }

            //Diagonal bottom right
            if (currentY - 1 >= 0)
            {
                if (board[currentX + 1, currentY - 1] == null)
                {
                    r.Add(new Vector2Int(currentX + 1, currentY - 1));
                }
                else if (board[currentX + 1, currentY - 1].team != team)
                {
                    r.Add(new Vector2Int(currentX + 1, currentY - 1));
                }
            }
        }

        //Left
        if (currentX - 1 >= 0)
        {
            //Left
            if (board[currentX - 1, currentY] == null)
            {
                r.Add(new Vector2Int(currentX - 1, currentY));
            }
            else if (board[currentX - 1, currentY].team != team)
            {
                r.Add(new Vector2Int(currentX + 1, currentY));
            }

            //Diagonal top left
            if (currentY + 1 < tileCountY)
            {
                if (board[currentX - 1, currentY + 1] == null)
                {
                    r.Add(new Vector2Int(currentX - 1, currentY + 1));
                }
                else if (board[currentX - 1, currentY + 1].team != team)
                {
                    r.Add(new Vector2Int(currentX - 1, currentY + 1));
                }
            }

            //Diagonal bottom left
            if (currentY - 1 >= 0)
            {
                if (board[currentX - 1, currentY - 1] == null)
                {
                    r.Add(new Vector2Int(currentX - 1, currentY - 1));
                }
                else if (board[currentX - 1, currentY - 1].team != team)
                {
                    r.Add(new Vector2Int(currentX - 1, currentY - 1));
                }
            }
        }

        //Up
        if(currentY +1 < tileCountY)
        {
            if(board[currentX, currentY + 1] == null || board[currentX, currentY + 1].team != team)
            {
                r.Add(new Vector2Int(currentX, currentY + 1));
            }
        }

        //Down
        if (currentY - 1 >= 0)
        {
            if (board[currentX, currentY - 1] == null || board[currentX, currentY - 1].team != team)
            {
                r.Add(new Vector2Int(currentX, currentY - 1));
            }
        }

        return r;
    }

    public override BoardManager.SpecialMove GetSpecialMoves(ref ChessPiece[,] board, ref List<Vector2Int> availableMoves, ref List<Vector2Int[]> moveList)
    {
        BoardManager.SpecialMove r = BoardManager.SpecialMove.None;

        //going through all the move on the list, if any of those move start position is at the king (x = 4) either in white team (y=0) or black team (y=7)
        var kingMove = moveList.Find(m => m[0].x == 4 && m[0].y == ((team == 0) ? 0 : 7));

        //get the rook on the left or right side
        var leftRook = moveList.Find(m => m[0].x == 0 && m[0].y == ((team == 0) ? 0 : 7));
        var rightRook = moveList.Find(m => m[0].x == 7 && m[0].y == ((team == 0) ? 0 : 7));


        //check if the king already move. if havent move, then proceed
        if (kingMove == null && currentX == 4)
        {
            //White team
            if(team == 0)
            {
                //Left Rook
                if(leftRook == null) //if left rook haven't move, then proceed
                {
                    if(board[0,0].type == ChessPieceType.Rook)
                    {
                        if(board[0,0].team == 0)
                        {
                            //if the tiles leftside of the king are empty
                            if(board[1,0] == null && board[2,0] == null && board[3,0] == null)
                            {
                                availableMoves.Add(new Vector2Int(2, 0));
                                r = BoardManager.SpecialMove.Castling;
                            }
                        }
                    }
                }

                //Right Rook
                if (rightRook == null) //if left rook haven't move, then proceed
                {
                    if (board[7, 0].type == ChessPieceType.Rook)
                    {
                        if (board[7, 0].team == 0)
                        {
                            //if the tiles leftside of the king are empty
                            if (board[5, 0] == null && board[6, 0] == null)
                            {
                                availableMoves.Add(new Vector2Int(6, 0));
                                r = BoardManager.SpecialMove.Castling;
                            }
                        }
                    }
                }
            }

            //Black Team
            if (team == 1)
            {
                //Left Rook
                if (leftRook == null) //if left rook haven't move, then proceed
                {
                    if (board[0, 7].type == ChessPieceType.Rook)
                    {
                        if (board[0, 7].team == 1)
                        {
                            //if the tiles leftside of the king are empty
                            if (board[1, 7] == null && board[2, 7] == null && board[3, 7] == null)
                            {
                                availableMoves.Add(new Vector2Int(2, 7));
                                r = BoardManager.SpecialMove.Castling;
                            }
                        }
                    }
                }

                //Right Rook
                if (rightRook == null) //if left rook haven't move, then proceed
                {
                    if (board[7, 7].type == ChessPieceType.Rook)
                    {
                        if (board[7, 7].team == 1)
                        {
                            //if the tiles leftside of the king are empty
                            if (board[5, 7] == null && board[6, 7] == null)
                            {
                                availableMoves.Add(new Vector2Int(6, 7));
                                r = BoardManager.SpecialMove.Castling;
                            }
                        }
                    }
                }
            }

        } 

        return r;
    }

    public void SetRookPosAfterCastling(Transform tileTransform, Transform rook)
    {
        rook.transform.SetParent(tileTransform, false);
        
    }
}
