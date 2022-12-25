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
        Move pickedMove = null;


        pickedMove = PreventCheckMate(ref simMap);
        if (pickedMove != null)
        {
            return pickedMove;
        }

        foreach (ChessPiece piece in blackPieces)
        {
            if (piece.GetAvailableMoves(ref simMap, BoardManager.TILE_X_COUNT, BoardManager.TILE_Y_COUNT).Count > 0)
            {
                int maxValue = int.MinValue;
                Vector2Int currentPieceBestMove = Vector2Int.down;
                var moves = piece.GetAvailableMoves(ref simMap, BoardManager.TILE_X_COUNT, BoardManager.TILE_Y_COUNT);

                int score = 0;
                foreach (Vector2Int m in moves)
                {
                    score = GetChessPieceValue(piece) + positionPoints.GetPositionValue(piece.type, m.x, m.y);

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

        pickedMove = piecesBestMove[Random.Range(0, piecesBestMove.Count)];
        return pickedMove;
    }

    public Move GetAIMove(ChessPiece[,] pieceMap)
    {
        SetAvailableChessPieces(pieceMap);
        var bestMove = Evaluate(pieceMap);
        return bestMove;
    }

    public void RemoveCheckmatePosition(ChessPiece piece, ref List<Vector2Int> moves, ref ChessPiece[,] simMap)
    {
        if (piece.type == ChessPieceType.King)
        {
            foreach (ChessPiece whitePiece in whitePieces)
            {
                if (whitePiece.GetAvailableMoves(ref simMap, BoardManager.TILE_X_COUNT, BoardManager.TILE_Y_COUNT).Count > 0)
                {
                    var whiteMove = whitePiece.GetAvailableMoves(ref simMap, BoardManager.TILE_X_COUNT, BoardManager.TILE_Y_COUNT);
                    foreach (Vector2Int wm in whiteMove)
                    {
                        if (simMap[wm.x, wm.y] != null)
                        {
                            if (simMap[wm.x, wm.y].type != ChessPieceType.King && simMap[wm.x, wm.y].team == 1)
                            {
                                moves.Remove(wm);
                            }
                        }
                    }
                }
            }
        }
    }

    public Move PreventCheckMate(ref ChessPiece[,] pieceMap)
    {
        //Check if the king is under attack
        ChessPiece targetKing = null;
        Move bestMove = null;

        for (int x = 0; x < BoardManager.TILE_X_COUNT; x++)
        {
            for (int y = 0; y < BoardManager.TILE_Y_COUNT; y++)
            {
                if (pieceMap[x, y] != null)
                {
                    if (pieceMap[x, y].type == ChessPieceType.King && pieceMap[x,y].team == 1)
                    {
                        targetKing = pieceMap[x, y];
                    }
                }

            }
        }


        foreach (ChessPiece piece in whitePieces)
        {
            if(piece.GetAvailableMoves(ref pieceMap, BoardManager.TILE_X_COUNT, BoardManager.TILE_Y_COUNT).Count > 0)
            {
                List<Vector2Int> moves = piece.GetAvailableMoves(ref pieceMap, BoardManager.TILE_X_COUNT, BoardManager.TILE_Y_COUNT);
                if (FindObjectOfType<BoardManager>().ContainsValidMove(ref moves, new Vector2Int(targetKing.currentX, targetKing.currentY)))
                {
                    //The king is being attacked
                    //Can the king move?
                    List<Vector2Int> kingMoves = new List<Vector2Int>();
                    if (targetKing.GetAvailableMoves(ref pieceMap, BoardManager.TILE_X_COUNT, BoardManager.TILE_Y_COUNT).Count > 0)
                    {
                        kingMoves = targetKing.GetAvailableMoves(ref pieceMap, BoardManager.TILE_X_COUNT, BoardManager.TILE_Y_COUNT);
                        RemoveCheckmatePosition(targetKing, ref kingMoves, ref pieceMap);
                    }

                    if(kingMoves.Count == 0)
                    {
                        bestMove = new Move(targetKing, kingMoves[0]);
                    }
                    else if(kingMoves.Count > 0)
                    {
                        bestMove = new Move(targetKing, kingMoves[Random.Range(0, kingMoves.Count)]);
                    }

                }
            }

        }

        return bestMove;
    }

    private void SimulatePickedMovement(ref ChessPiece[,] simMap, Move move)
    {
        simMap[move.piece.currentX, move.piece.currentY] = null;
        simMap[move.tile.x, move.tile.y] = move.piece;
        move.piece.currentX = move.tile.x;
        move.piece.currentY = move.tile.y;
    }


    private bool NewMoveCausingCheckMate(ChessPiece[,] simMap)
    {
        //Check if the king is under attack
        ChessPiece targetKing = null;

        for (int x = 0; x < BoardManager.TILE_X_COUNT; x++)
        {
            for (int y = 0; y < BoardManager.TILE_Y_COUNT; y++)
            {
                if (simMap[x, y] != null)
                {
                    if (simMap[x, y].type == ChessPieceType.King && simMap[x,y].team == 1)
                    {
                        targetKing = simMap[x, y];
                    }
                }

            }
        }


        foreach (ChessPiece piece in whitePieces)
        {
            if (piece.GetAvailableMoves(ref simMap, BoardManager.TILE_X_COUNT, BoardManager.TILE_Y_COUNT).Count > 0)
            {
                List<Vector2Int> moves = piece.GetAvailableMoves(ref simMap, BoardManager.TILE_X_COUNT, BoardManager.TILE_Y_COUNT);
                if (FindObjectOfType<BoardManager>().ContainsValidMove(ref moves, new Vector2Int(targetKing.currentX, targetKing.currentY)))
                {
                    return true;
                }
            }
        }

        return false;
    }


}


