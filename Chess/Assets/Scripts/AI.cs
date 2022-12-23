using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AI : BoardManager
{
    protected override ChessPiece GetAIPiece()
    {
       List<ChessPiece> blackPieces = new List<ChessPiece>();

       for(int x=0; x< TILE_X_COUNT; x++)
       {
            for(int y=0; y<TILE_Y_COUNT; y++)
            {
                if(pieceMap[x,y]!= null && pieceMap[x,y].team == 1)
                {
                    //Check if the piece is able to move
                    if(pieceMap[x,y].GetAvailableMoves(ref pieceMap,TILE_X_COUNT, TILE_Y_COUNT).Count > 0)
                    {
                        blackPieces.Add(pieceMap[x, y]);
                    }

                }
            }
       }

        int random = Random.Range(0, blackPieces.Count);

        return blackPieces[random];

    }
}
