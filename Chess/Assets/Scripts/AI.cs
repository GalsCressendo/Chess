using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    //AI Section
    public GameObject GetAIRandomBlackPiece(ChessPiece[,] pieceMap, int tileCountX, int tileCountY)
    {
        List<ChessPiece> blackPieces = new List<ChessPiece>();

        for (int x = 0; x < tileCountX; x++)
        {
            for (int y = 0; y < tileCountY; y++)
            {
                if (pieceMap[x, y] != null && pieceMap[x, y].team == 1)
                {
                    if (pieceMap[x, y].GetAvailableMoves(ref pieceMap, tileCountX, tileCountY).Count > 0)
                    {
                        blackPieces.Add(pieceMap[x, y]);
                    }
                }

            }
        }

        int r = Random.Range(0, blackPieces.Count);
        return blackPieces[r].gameObject;
    }

    public GameObject GetAIChosenTile(List<Vector2Int> availableMoves, GameObject[,] board)
    {
        Vector2Int tile = Vector2Int.down;
        if (availableMoves.Count > 0)
        {
            int r = Random.Range(0, availableMoves.Count);
            tile = availableMoves[r];
        }

        if (tile != Vector2Int.down)
        {
            return board[tile.x, tile.y];
        }

        return null;
    }
}
