public class PiecePositionPoint
{
    private readonly int[,] PAWN_POSITION_POINTS =
    {
        { 0,  0,  0,  0, 0,  0,  0,  0},
        { 50, 50, 50, 50, 50, 50, 50, 50},
        {10, 10, 20, 30, 30, 20, 10, 10 },
        {5,  5, 10, 25, 25, 10,  5,  5 },
        {0,  0,  0, 20, 20,  0,  0,  0 },
        {5, -5,-10,  0,  0,-10, -5,  5 },
        { 5, 10, 10,-20,-20, 10, 10,  5},
        { 0,  0,  0,  0, 0,  0,  0,  0}
    };

    private readonly int[,] KNIGHT_POSITION_POINTS =
    {
        { -50,-40,-30,-30,-30,-30,-40,-50 },
        {  -40,-20,  0,  0,  0,  0,-20,-40 },
        { -30,  0, 10, 15, 15, 10,  0,-30 },
        { -30,  5, 15, 20, 20, 15,  5,-30 },
        { -30,  0, 15, 20, 20, 15,  0,-30},
        { -30,  5, 10, 15, 15, 10,  5,-30},
        { -40,-20,  0,  5,  5,  0,-20,-40},
        { 50,-40,-30,-30,-30,-30,-40,-50}
    };

    private readonly int[,] ROOK_POSITION_POINTS =
    {
         { 0,  0,  0,  0,  0,  0,  0,  0 },
         { 5, 10, 10, 10, 10, 10, 10,  5 },
         { -5,  0,  0,  0,  0,  0,  0, -5 },
         { -5,  0,  0,  0,  0,  0,  0, -5 },
         { -5,  0,  0,  0,  0,  0,  0, -5 },
         { -5,  0,  0,  0,  0,  0,  0, -5 },
         { -5,  0,  0,  0,  0,  0,  0, -5 },
         { 0,  0,  0,  5,  5,  0,  0,  0 }
    };

    private readonly int[,] BISHOP_POSITON_POINTS =
    {
            { -20,-10,-10,-10,-10,-10,-10,-20 },
            { -10,  0,  0,  0,  0,  0,  0,-10 },
            {-10,  0,  5, 10, 10,  5,  0,-10},
            { -10,  5,  5, 10, 10,  5,  5,-10},
            { -10,  0, 10, 10, 10, 10,  0,-10},
            { -10, 10, 10, 10, 10, 10, 10,-10},
            { -10,  5,  0,  0,  0,  0,  5,-10},
            { -20,-10,-10,-10,-10,-10,-10,-20}
    };

    private readonly int[,] QUEEN_POSITION_POINTS =
    {
             { -20,-10,-10, -5, -5,-10,-10,-20 },
             { -10,  0,  0,  0,  0,  0,  0,-10 },
             { -10,  0,  5,  5,  5,  5,  0,-10 },
             { -5,  0,  5,  5,  5,  5,  0, -5 },
             { 0,  0,  5,  5,  5,  5,  0, -5 },
             { -10,  5,  5,  5,  5,  5,  0,-10 },
             { -10,  0,  5,  0,  0,  0,  0,-10 },
             { -20,-10,-10, -5, -5,-10,-10,-20 }
    };

    private readonly int[,] KING_POSITION_POINTS =
    {
            { -30, -40, -40, -50, -50, -40, -40, -30 },
            { -30, -40, -40, -50, -50, -40, -40, -30 },
            { -30, -40, -40, -50, -50, -40, -40, -30 },
            { -30, -40, -40, -50, -50, -40, -40, -30 },
            { -20, -30, -30, -40, -40, -30, -30, -20 },
            { -10, -20, -20, -20, -20, -20, -20, -10 },
            { 20, 20, 0, 0, 0, 0, 20, 20 },
            { 20, 30, 10, 0, 0, 10, 30, 20 }
    };

    public int GetPositionValue(ChessPieceType p, int x, int y)
    {
        switch (p)
        {
            case ChessPieceType.Pawn:
                return PAWN_POSITION_POINTS[x, y];
            case ChessPieceType.Rook:
                return ROOK_POSITION_POINTS[x, y];
            case ChessPieceType.Knight:
                return KNIGHT_POSITION_POINTS[x, y];
            case ChessPieceType.Bishop:
                return BISHOP_POSITON_POINTS[x, y];
            case ChessPieceType.Queen:
                return QUEEN_POSITION_POINTS[x, y];
            case ChessPieceType.King:
                return KING_POSITION_POINTS[x, y];
        }

        return 0;
    }

}
