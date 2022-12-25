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

    List<ChessPiece> blackPieces = new List<ChessPiece>();
    List<ChessPiece> whitePieces = new List<ChessPiece>();
    PiecePositionPoint positionPoints = new PiecePositionPoint();

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
                    if (pieceMap[x, y].team == 1)
                    {
                        blackPieces.Add(pieceMap[x, y]);
                    }
                    else
                    {
                        whitePieces.Add(pieceMap[x, y]);
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

    public Move Evaluate(ChessPiece[,] pieceMap)
    {
        ChessPiece[,] simMap = pieceMap;
        List<Move> piecesBestMove = new List<Move>();

        foreach (ChessPiece piece in blackPieces)
        {
            if (piece.GetAvailableMoves(ref simMap, BoardManager.TILE_X_COUNT, BoardManager.TILE_Y_COUNT).Count > 0)
            {
                int maxValue = int.MinValue;
                Vector2Int currentPieceBestMove = Vector2Int.down;
                var moves = piece.GetAvailableMoves(ref simMap, BoardManager.TILE_X_COUNT, BoardManager.TILE_Y_COUNT);
                foreach (Vector2Int m in moves)
                {
                    int score = GetChessPieceValue(piece) + positionPoints.GetPositionValue(piece.type, m.x, m.y);

                    if (simMap[m.x, m.y] != null && simMap[m.x, m.y].team != piece.team)
                    {
                        score += GetChessPieceValue(simMap[m.x, m.y]);
                    }

                    if (score > maxValue)
                    {
                        maxValue = score;
                        currentPieceBestMove = m;
                    }

                }

                piecesBestMove.Add(new Move(piece, currentPieceBestMove));
            }
        }

        return piecesBestMove[Random.Range(0, piecesBestMove.Count)];
    }

    public Move GetAIMove(ChessPiece[,] pieceMap)
    {
        SetAvailableChessPieces(pieceMap);
        var bestMove = Evaluate(pieceMap);
        return bestMove;
    }
}


