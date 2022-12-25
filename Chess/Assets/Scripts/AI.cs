using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    private const int PAWN_VALUE = 100;
    private const int KNIGHT_VALUE = 320;
    private const int BISHOP_VALUE = 330;
    private const int ROOK_VALUE = 500;
    private const int QUEEN_VALUE = 900;
    private const int KING_VALUE = 20000;

    private int depth = 1;
    List<ChessPiece> blackPieces = new List<ChessPiece>();
    List<ChessPiece> whitePieces = new List<ChessPiece>();

    public class Move
    {
        public ChessPiece piece;
        public Vector2Int tile;

        public Move(ChessPiece piece, Vector2Int tile)
        {
            this.piece = piece;
            this.tile = tile;
        }
    }

    public void SetAvailableChessPieces(ChessPiece[,] pieceMap)
    {
        for (int x = 0; x < BoardManager.TILE_X_COUNT; x++)
        {
            for (int y = 0; y < BoardManager.TILE_Y_COUNT; y++)
            {
                if (pieceMap[x, y] != null)
                {
                    if(pieceMap[x,y].team == 0)
                    {
                        whitePieces.Add(pieceMap[x, y]);
                    }
                    else
                    {
                        blackPieces.Add(pieceMap[x, y]);
                    }
                }
            }
        }
    }

    public int GetChessPieceValue(ChessPiece p)
    {
        switch (p.type)
        {
            case ChessPieceType.Pawn:
                return PAWN_VALUE;
            case ChessPieceType.Rook:
                return ROOK_VALUE;
            case ChessPieceType.Knight:
                return KNIGHT_VALUE;
            case ChessPieceType.Bishop:
                return BISHOP_VALUE;
            case ChessPieceType.Queen:
                return QUEEN_VALUE;
            case ChessPieceType.King:
                return KING_VALUE;
        }

        return 0;
    }

    public Move Evaluate(ChessPiece[,] pieceMap, GameObject[,] tileMap)
    {
        ChessPiece[,] simMap = pieceMap;
        Move bestMove = null;
        for(int i =0; i< depth; i++)
        {
            if(i%2 == 0) //Max
            {
                int maxValue = int.MinValue;
                foreach(ChessPiece p in blackPieces)
                {
                    if(p.GetAvailableMoves(ref simMap, BoardManager.TILE_X_COUNT, BoardManager.TILE_Y_COUNT).Count > 0)
                    {
                        List<Vector2Int> moves = p.GetAvailableMoves(ref simMap, BoardManager.TILE_X_COUNT, BoardManager.TILE_Y_COUNT);
                        foreach(Vector2Int m in moves)
                        {
                            if(tileMap[m.x, m.y].transform.childCount > 0)
                            {
                                int score = GetChessPieceValue(p) - GetChessPieceValue(simMap[m.x, m.y]);
                                if(score > maxValue)
                                {
                                    maxValue = score;
                                    bestMove = new Move(p, m);
                                }
                            }
                            else
                            {
                                int score = GetChessPieceValue(p);
                                if (score > maxValue)
                                {
                                    maxValue = score;
                                    bestMove = new Move(p, m);
                                }
                            }
                        }
                    }
                }

                simMap[bestMove.piece.currentX, bestMove.piece.currentY] = null;
                simMap[bestMove.tile.x, bestMove.tile.y] = bestMove.piece;
                
            }
            else //Min
            {

            }
        }

        return bestMove;
    }

    public Move GetAIMove (ChessPiece[,] pieceMap, GameObject[,] tileMap)
    {
        SetAvailableChessPieces(pieceMap);
        var bestMove = Evaluate(pieceMap, tileMap);
        return bestMove;
    }
}
