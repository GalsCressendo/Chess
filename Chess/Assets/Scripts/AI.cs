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

    [SerializeField] BoardManager boardManager;
    public Node root = new Node(null);
    Dictionary<int, List<Node>> Tree = new Dictionary<int, List<Node>>();

    int depth = 2;

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
        public List<Node> children = new List<Node>();
        public Node parent;

        public Node(Move move)
        {
            this.move = move;
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
                foreach (Vector2Int m in moves)
                {
                    Node child = GenerateChild(new Move(p, m));
                    target.AddChild(child);
                }

            }
        }

        foreach (Node node in target.children)
        {
            Tree[currentDepth].Add(node);
        }

        foreach (Node node in Tree[currentDepth])
        {
            SimulateMovement(node, map, currentDepth);
            if (currentDepth < depth)
            {
                GenerateNextMovement(node.map, node, currentDepth+1);
            }
        }



    }

    private Node GenerateChild(Move move)
    {
        Node child = new Node(move);
        return child;
    }

    public void Evaluate(ChessPiece[,] simMap)
    {
        Tree = new Dictionary<int, List<Node>>();
        var map = simMap;
        List<Node> roots = new List<Node>();
        roots.Add(root);

        Tree.Add(0, roots);

        for (int i = 1; i <= depth; i++)
        {
            Tree.Add(i, new List<Node>());
        }

        GenerateNextMovement(map, root, 1);

        MinMaxAlgorithm(depth - 1);

    }

    private void SimulateMovement(Node node, ChessPiece[,] originalMap, int currentDepth)
    {
        var map = originalMap;

        if(currentDepth == depth)
        {
            int value = GetChessPieceValue(node.move.piece);
            if (map[node.move.tile.x, node.move.tile.y] != null)
            {
                value -= GetChessPieceValue(map[node.move.tile.x, node.move.tile.y]);
            }

            node.SetValue(value);
        }


        map[node.move.piece.currentX, node.move.piece.currentY] = null;
        map[node.move.tile.x, node.move.tile.y] = node.move.piece;
        node.move.piece.currentX = node.move.tile.x;
        node.move.piece.currentY = node.move.tile.y;

        node.SetMap(map);
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

    private void MinMaxAlgorithm(int currentDepth)
    {
        List<Node> bestNode = new List<Node>();
        foreach (Node node in Tree[currentDepth])
        {
            if (currentDepth % 2 == 0) //The child is black move (ai move), maximize
            {
                int maxValue = int.MinValue;
                foreach (Node child in node.children)
                {
                    if (child.value > maxValue)
                    {
                        maxValue = child.value;
                    }
                }

                bestNode = (from n in node.children
                            where n.value == maxValue
                            select n).ToList();

            }
            else
            {
                int minValue = int.MaxValue;
                foreach (Node child in node.children)
                {
                    if (child.value < minValue)
                    {
                        minValue = child.value;
                    }
                }

                bestNode = (from n in node.children
                            where n.value == minValue
                            select n).ToList();
            }

            Node chosenNode = null;

            if (bestNode.Count > 1)
            {
                chosenNode = bestNode[UnityEngine.Random.Range(0, bestNode.Count)];
            }
            else if(bestNode.Count == 1)
            {
                chosenNode = bestNode[0];
            }

            if (chosenNode != null)
            {
                node.value = chosenNode.value;
                if (node.parent != null)
                {
                    MinMaxAlgorithm(currentDepth - 1);
                }
                else
                {
                    node.move = chosenNode.move;
                }
            }
            
        }

    }


}


