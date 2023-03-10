using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ChessPieceType
{
    None = 0,
    Pawn = 1,
    Rook = 2,
    Knight = 3,
    Bishop = 4,
    Queen = 5,
    King = 6
}

public class ChessPiece : MonoBehaviour
{
    public ChessPieceType type;
    public int team; //0 is white, 1 is black
    public int currentX;
    public int currentY;

    public virtual List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX,int tileCountY)
    {
        return new List<Vector2Int>();
    }

    public virtual BoardManager.SpecialMove GetSpecialMoves(ref ChessPiece[,] board, ref List<Vector2Int> availableMoves, ref List<Vector2Int[]> moveList)
    {
        return BoardManager.SpecialMove.None;
    }

}
