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

        Move bestMove = CheckForCheckMate(simMap);

        if(bestMove != null)
        {
            return bestMove;
        }

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

    private Move CheckForCheckMate(ChessPiece[,] pieceMap)
    {
        var simMap = pieceMap;
        ChessPiece targetKing = null;
        List<Move> defendMove = new List<Move>();

        //Get the black team king
        for (int x = 0; x < BoardManager.TILE_X_COUNT; x++)
        {
            for (int y = 0; y < BoardManager.TILE_Y_COUNT; y++)
            {
                if(pieceMap[x,y]!= null)
                {
                    if (pieceMap[x, y].type == ChessPieceType.King && pieceMap[x, y].team == 1)
                    {
                        targetKing = pieceMap[x, y];
                    }
                }
                
            }
        }

        List<Vector2Int> regularMoves = new List<Vector2Int>();
        BoardManager.SpecialMove sm = BoardManager.SpecialMove.None;

        //Check for available movement in white pieces
        foreach (ChessPiece piece in whitePieces)
        {
            if(piece.GetAvailableMoves(ref simMap, BoardManager.TILE_X_COUNT, BoardManager.TILE_Y_COUNT).Count > 0)
            {
                var moves = piece.GetAvailableMoves(ref simMap, BoardManager.TILE_X_COUNT, BoardManager.TILE_Y_COUNT);
                foreach(Vector2Int m in moves)
                {
                    if(simMap[m.x,m.y]!= null && simMap[m.x, m.y].type == ChessPieceType.King && simMap[m.x,m.y].team == 1)
                    {
                        //BLACK KING IS BEING ATTACKED

                        //Can he move?
                        regularMoves = targetKing.GetAvailableMoves(ref simMap, BoardManager.TILE_X_COUNT, BoardManager.TILE_Y_COUNT);
                        sm = targetKing.GetSpecialMoves(ref simMap, ref regularMoves, ref FindObjectOfType<BoardManager>().moveList);

                        Vector2Int currentBestMove = Vector2Int.down;
                        if(regularMoves.Count > 0)
                        {
                            int maxValue = int.MinValue;
                            foreach(Vector2Int rm in regularMoves)
                            {
                                int score = GetChessPieceValue(targetKing) + positionPoints.GetPositionValue(targetKing.type, rm.x, rm.y);

                                if(score > maxValue)
                                {
                                    maxValue = score;
                                    currentBestMove = rm;
                                }
                            }

                            FindObjectOfType<BoardManager>().ProcessSpecialMove(sm);
                            return (new Move(targetKing, currentBestMove));
                        }

                        //Can we defend him?
                        for (int i = 0; i < blackPieces.Count; i++)
                        {
                            List<Vector2Int> defendingMoves = blackPieces[i].GetAvailableMoves(ref simMap, BoardManager.TILE_X_COUNT, BoardManager.TILE_Y_COUNT);
                            FindObjectOfType<BoardManager>().SimulateMoveForSinglePiece(blackPieces[i], ref defendingMoves, targetKing);

                            //if defending moves is empty, then we are in checkmate
                            if (defendingMoves.Count != 0)
                            {
                                foreach(Vector2Int dm in defendingMoves)
                                {
                                    defendMove.Add(new Move(blackPieces[i], dm));
                                }
                            }
                        }

                        return defendMove[Random.Range(0, defendMove.Count)];
                    }
                }
            }
        }

        return null;
    }
}


