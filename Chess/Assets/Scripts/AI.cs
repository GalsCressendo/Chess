using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AI : MonoBehaviour
{
    private const int PAWN_VALUE = 10;
    private const int KNIGHT_VALUE = 30;
    private const int BISHOP_VALUE = 30;
    private const int ROOK_VALUE = 50;
    private const int QUEEN_VALUE = 90;
    private const int KING_VALUE = 9000;

    PiecePositionPoint positionPoints = new PiecePositionPoint();
    [SerializeField] BoardManager boardManager;
    Node root = new Node(null, null);

    int depth = 3;

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

    //Non-Binary Tree
    public class Node
    {
        public Move move;
        public ChessPiece[,] map;
        public int value;
        public List<Node> children;
        public Node parent;

        public Node(Move move, Node parent)
        {
            this.move = move;
            children = new List<Node>();
            this.parent = parent;
        }

        public void AddChild(Node node)
        {
            children.Add(node);
            node.parent = this;
        }

        public void SetValue(int value)
        {
            this.value = value;
        }

        public void SetMap(ChessPiece[,] map)
        {
            this.map = map;
        }

    }

    public int GetChessPieceValue(ChessPiece p)
    {
        if (p.team == 1)
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
        }
        else
        {
            switch (p.type)
            {
                case ChessPieceType.Pawn:
                    return -PAWN_VALUE;
                case ChessPieceType.Rook:
                    return -ROOK_VALUE;
                case ChessPieceType.Knight:
                    return -KNIGHT_VALUE;
                case ChessPieceType.Bishop:
                    return -BISHOP_VALUE;
                case ChessPieceType.Queen:
                    return -QUEEN_VALUE;
                case ChessPieceType.King:
                    return -KING_VALUE;
            }
        }


        return 0;
    }

    private void GenerateNextMovement(ChessPiece[,] simMap, Node target, int currentDepth)
    {
        var map = simMap;
        if (currentDepth >= depth)
        {
            if (target.children.Count > 0)
            {
                foreach (Node child in target.children)
                {
                    var movement = SimulateMovement(child.move, map);
                    child.SetValue(movement.Item2);
                }
            }

            return;
        }

        List<ChessPiece> pieces = new List<ChessPiece>();
        if (currentDepth % 2 == 0) //its white turn
        {
            pieces = GetPieces(0, map);

        }
        else
        {
            pieces = GetPieces(1, map);
        }

        foreach (ChessPiece p in pieces)
        {
            var moves = p.GetAvailableMoves(ref map, BoardManager.TILE_X_COUNT, BoardManager.TILE_Y_COUNT);
            if (moves.Count > 0)
            {
                var children = GenerateChildren(map, p);
                if (children.Count > 0)
                {
                    foreach (Node child in children)
                    {
                        target.AddChild(child);
                    }
                }
            }
        }

        foreach (Node child in target.children)
        {
            var movement = SimulateMovement(child.move, map);
            currentDepth += 1;
            if (currentDepth >= depth)
            {
                child.SetValue(movement.Item2);
                continue;
            }
            GenerateNextMovement(movement.Item1, child, currentDepth);
        }
    }

    private List<Node> GenerateInitialParents(ChessPiece[,] simMap)
    {
        var map = simMap;
        List<ChessPiece> blackPieces = GetPieces(1, map);
        List<Node> parentsNode = new List<Node>();

        foreach (ChessPiece p in blackPieces)
        {
            var moves = p.GetAvailableMoves(ref map, BoardManager.TILE_X_COUNT, BoardManager.TILE_Y_COUNT);
            if (moves.Count > 0)
            {
                foreach (Vector2Int m in moves)
                {
                    Node newNode = new Node(new Move(p, m), null);
                    parentsNode.Add(newNode);
                }

            }
        }

        return parentsNode;
    }

    private List<Node> GenerateChildren(ChessPiece[,] map, ChessPiece piece)
    {
        List<Node> children = new List<Node>();
        var moves = piece.GetAvailableMoves(ref map, BoardManager.TILE_X_COUNT, BoardManager.TILE_Y_COUNT);
        if (moves.Count > 0)
        {
            foreach (Vector2Int m in moves)
            {
                Node newNode = new Node(new Move(piece, m), null);
                children.Add(newNode);
            }

        }

        return children;
    }

    public void GenerateTree(ChessPiece[,] simMap)
    {
        var map = simMap;
        var firstParents = GenerateInitialParents(map);

        foreach (Node node in firstParents)
        {
            root.AddChild(node);
        }

        GenerateNextMovement(map, root, 1);

        GetLastNodes(root);
        //Debug.Log(string.Format("{0},{1}", root.move.piece, root.move.tile));
    }

    private void GetLastNodes(Node target)
    {
        if(target.children.Count > 0)
        {
            foreach(Node child in target.children)
            {
                if(child.children.Count <= 0)
                {
                    Debug.Log(string.Format("{0},{1}", child.move.piece, child.move.tile));
                }
                else
                {
                    GetLastNodes(child);
                }
            }
        }

        

    }

    private Tuple<ChessPiece[,], int> SimulateMovement(Move move, ChessPiece[,] originalMap)
    {
        var map = originalMap;
        int value = GetChessPieceValue(move.piece);
        if (map[move.tile.x, move.tile.y] != null)
        {
            value -= GetChessPieceValue(map[move.tile.x, move.tile.y]);
        }

        map[move.piece.currentX, move.piece.currentY] = null;
        map[move.tile.x, move.tile.y] = move.piece;
        move.piece.currentX = move.tile.x;
        move.piece.currentY = move.tile.y;

        return Tuple.Create(map, value);
    }

    private List<ChessPiece> GetPieces(int team, ChessPiece[,] originalMap)
    {
        List<ChessPiece> pieces = new List<ChessPiece>();
        for (int x = 0; x < BoardManager.TILE_X_COUNT; x++)
        {
            for (int y = 0; y < BoardManager.TILE_Y_COUNT; y++)
            {
                if (originalMap[x, y] != null)
                {
                    if (originalMap[x, y].team == team)
                    {
                        pieces.Add(originalMap[x, y]);
                    }
                }
            }
        }

        return pieces;
    }


}


