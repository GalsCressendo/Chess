using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
    const int TILE_X_COUNT = 8;
    const int TILE_Y_COUNT = 8;
    const string TILE_TAG = "Tile";

    [Header("Board Components")]
    public GameObject whiteTile;
    public GameObject brownTile;
    private GameObject startTile;
    private GameObject currentTile;
    public GameObject[,] board = new GameObject[TILE_X_COUNT, TILE_Y_COUNT];

    [Header("Pieces Components")]
    [SerializeField] private GameObject[] chessPiecePrefabs;
    [SerializeField] private Material[] colorMaterial;
    public ChessPiece[,] chessPieces;

    private void Awake()
    {
        SpawnAllChessPieces();
        GenerateAllTiles();
    }

    private void GenerateAllTiles()
    {
        for(int x = 0; x< TILE_X_COUNT; x++)
        {
            if(startTile == null)
            {
                startTile = brownTile;
            }
            else if (startTile == brownTile)
            {
                startTile = whiteTile;
            }
            else if(startTile = whiteTile)
            {
                startTile = brownTile;
            }

            for(int y=0; y < TILE_Y_COUNT; y++)
            {
                if (y == 0)
                {
                    currentTile = startTile;
                }
                else if(currentTile == brownTile)
                {
                    currentTile = whiteTile;
                }
                else if (currentTile == whiteTile)
                {
                    currentTile = brownTile;
                }

                Vector3 spawnPos = new Vector3(x, 0, y);
                GameObject tile = Instantiate(currentTile, spawnPos, Quaternion.identity);
                tile.transform.SetParent(transform, false);
                tile.name = string.Format("X:{0},Y:{1}", x, y);
                tile.tag = TILE_TAG;
                board[x, y] = tile;

                RepositionChessPiece(x, y, tile);
            }
        }

    }

    private void SpawnAllChessPieces()
    {
        chessPieces = new ChessPiece[TILE_X_COUNT, TILE_Y_COUNT];

        int whiteTeam = 0, blackTeam = 1; 

        //White Team
        chessPieces[0, 0] = SpawnSinglePiece(ChessPieceType.Rook, whiteTeam);
        chessPieces[1, 0] = SpawnSinglePiece(ChessPieceType.Knight, whiteTeam);
        chessPieces[2, 0] = SpawnSinglePiece(ChessPieceType.Bishop, whiteTeam);
        chessPieces[3, 0] = SpawnSinglePiece(ChessPieceType.Queen, whiteTeam);
        chessPieces[4, 0] = SpawnSinglePiece(ChessPieceType.King, whiteTeam);
        chessPieces[5, 0] = SpawnSinglePiece(ChessPieceType.Bishop, whiteTeam);
        chessPieces[6, 0] = SpawnSinglePiece(ChessPieceType.Knight, whiteTeam);
        chessPieces[7, 0] = SpawnSinglePiece(ChessPieceType.Rook, whiteTeam);
        for (int i = 0; i < TILE_X_COUNT; i++)
        {
            chessPieces[i, 1] = SpawnSinglePiece(ChessPieceType.Pawn, whiteTeam);
        }

        //Black Team
        chessPieces[0, 7] = SpawnSinglePiece(ChessPieceType.Rook, blackTeam);
        chessPieces[1, 7] = SpawnSinglePiece(ChessPieceType.Knight,blackTeam);
        chessPieces[2, 7] = SpawnSinglePiece(ChessPieceType.Bishop, blackTeam);
        chessPieces[3, 7] = SpawnSinglePiece(ChessPieceType.Queen, blackTeam);
        chessPieces[4, 7] = SpawnSinglePiece(ChessPieceType.King, blackTeam);
        chessPieces[5, 7] = SpawnSinglePiece(ChessPieceType.Bishop, blackTeam);
        chessPieces[6, 7] = SpawnSinglePiece(ChessPieceType.Knight, blackTeam);
        chessPieces[7, 7] = SpawnSinglePiece(ChessPieceType.Rook, blackTeam);
        for (int i = 0; i < TILE_X_COUNT; i++)
        {
            chessPieces[i, 6] = SpawnSinglePiece(ChessPieceType.Pawn, blackTeam);
        }

    }

    public ChessPiece SpawnSinglePiece(ChessPieceType type, int team)
    {
        ChessPiece cp = Instantiate(chessPiecePrefabs[(int)type - 1], transform).GetComponent<ChessPiece>();
        cp.type = type;
        cp.team = team;
        cp.GetComponent<MeshRenderer>().material = colorMaterial[team];
        if (team == 1)
        {
            cp.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }

        return cp;
    }

    private void RepositionChessPiece(int x, int y, GameObject parent)
    {
        if (chessPieces[x, y] != null)
        {
            chessPieces[x, y].transform.SetParent(parent.transform, false);
            chessPieces[x, y].currentX = x;
            chessPieces[x, y].currentY = y;
        }

    }
}
