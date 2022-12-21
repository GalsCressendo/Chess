using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    const int TILE_X_COUNT = 8;
    const int TILE_Y_COUNT = 8;
    const string TILE_TAG = "Tile";

    public GameObject[,] tileMap;
    GameManager gameManager;

    //hovering
    public Material hoverMaterial;
    GameObject currentTile;
    Material initialMaterial;

    //piece movement
    ChessPiece[,] pieceMap;
    private GameObject currentPiece;
    private Vector3 whiteDeadLastPos = Vector3.zero;
    private Vector3 blackDeadLastPos = Vector3.zero;
    private List<Vector2Int> availableMoves;
    private List<Vector2Int[]> moveList = new List<Vector2Int[]>(); //To keep track of move record

    //special move
    public enum SpecialMove
    {
        None = 0,
        EnPassant = 1,
        Castling = 2,
        Promotion = 3
    }
    private SpecialMove specialMove;

    //highlighting
    public Material highlightMaterial;
    Material[,] highlight_initialMaterial;

    private void Start()
    {
        pieceMap = FindObjectOfType<BoardGenerator>().chessPieces;
        tileMap = FindObjectOfType<BoardGenerator>().board;
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if (gameManager.gameIsActive)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == TILE_TAG)
                {

                    if (currentTile == null)
                    {
                        currentTile = hit.transform.gameObject;
                        initialMaterial = hit.transform.GetComponent<MeshRenderer>().material;
                        currentTile.GetComponent<MeshRenderer>().material = hoverMaterial;
                    }
                    else if (currentTile != hit.transform.gameObject)
                    {
                        currentTile.GetComponent<MeshRenderer>().material = initialMaterial;
                        initialMaterial = hit.transform.GetComponent<MeshRenderer>().material;
                        currentTile = hit.transform.gameObject;
                        currentTile.GetComponent<MeshRenderer>().material = hoverMaterial;
                    }

                    if (Input.GetMouseButtonDown(0))
                    {

                        currentTile.GetComponent<MeshRenderer>().material = initialMaterial;

                        //if not holding a piece
                        if (currentPiece == null)
                        {
                            if (currentTile.transform.childCount == 1)
                            {
                                if ((gameManager.turnState == GameManager.TurnState.WhiteTurn && currentTile.transform.GetChild(0).GetComponent<ChessPiece>().team == 0) ||
                                    (gameManager.turnState == GameManager.TurnState.BlackTurn && currentTile.transform.GetChild(0).GetComponent<ChessPiece>().team == 1))
                                {
                                    currentPiece = currentTile.transform.GetChild(0).gameObject;
                                    currentPiece.transform.position = new Vector3(currentPiece.transform.position.x, currentPiece.transform.position.y + 1, currentPiece.transform.position.z);

                                    highlight_initialMaterial = new Material[TILE_X_COUNT, TILE_Y_COUNT];
                                    availableMoves = currentPiece.GetComponent<ChessPiece>().GetAvailableMoves(ref pieceMap, TILE_X_COUNT, TILE_Y_COUNT);
                                    specialMove = currentPiece.GetComponent<ChessPiece>().GetSpecialMoves(ref pieceMap, ref availableMoves, ref moveList);
                                    highlight_initialMaterial = GetHighlightInitialMaterial(availableMoves);
                                    HighlightMoveTile(availableMoves);
                                }

                            }
                        }
                        //if holding a piece
                        else if (currentPiece != null)
                        {
                            //if this tile is valid
                            if (availableMoves.Contains(new Vector2Int((int)currentTile.transform.position.x, (int)currentTile.transform.position.z)))
                            {
                                //if there is another piece in the tile
                                if (currentTile.transform.childCount == 1)
                                {
                                    if (currentPiece.GetComponent<ChessPiece>().team != currentTile.transform.GetChild(0).GetComponent<ChessPiece>().team)
                                    {
                                        GameObject eatenPiece = currentTile.transform.GetChild(0).gameObject;
                                        eatenPiece.transform.SetParent(null, false);
                                        eatenPiece.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

                                        //if the eaten piece is white piece
                                        if (eatenPiece.GetComponent<ChessPiece>().team == 0)
                                        {

                                            if (whiteDeadLastPos == Vector3.zero)
                                            {
                                                Vector3 position = new Vector3(-1, 0, 7);
                                                eatenPiece.transform.position = position;
                                                whiteDeadLastPos = position;
                                            }
                                            else
                                            {
                                                if (whiteDeadLastPos.x <= -4)
                                                {
                                                    whiteDeadLastPos.x = -1;
                                                    whiteDeadLastPos.z -= 0.5f;
                                                }
                                                Vector3 position = new Vector3(whiteDeadLastPos.x - 0.5f, 0, whiteDeadLastPos.z);
                                                eatenPiece.transform.position = position;
                                                whiteDeadLastPos = position;
                                            }
                                        }
                                        //if the eaten piece is black piece
                                        else
                                        {

                                            if (blackDeadLastPos == Vector3.zero)
                                            {
                                                Vector3 position = new Vector3(8, 0, 0);
                                                eatenPiece.transform.position = position;
                                                blackDeadLastPos = position;
                                            }
                                            else
                                            {
                                                if (blackDeadLastPos.x >= 10)
                                                {
                                                    blackDeadLastPos.x = 8;
                                                    blackDeadLastPos.z += 0.5f;
                                                }
                                                Vector3 position = new Vector3(blackDeadLastPos.x + 0.5f, 0, blackDeadLastPos.z);
                                                eatenPiece.transform.position = position;
                                                blackDeadLastPos = position;
                                            }
                                        }

                                        //Set the new position of the current piece
                                        SetPiecePosition(currentTile.transform);
                                        gameManager.CheckWinConditions(eatenPiece.GetComponent<ChessPiece>().type, eatenPiece.GetComponent<ChessPiece>().team);

                                    }
                                    else if (currentPiece.GetComponent<ChessPiece>().team == currentTile.transform.GetChild(0).GetComponent<ChessPiece>().team)
                                    {
                                        //Put back the piece;
                                        ResetPiecePosition();
                                    }

                                }
                                else
                                {

                                    //Set the new position of the current piece
                                    SetPiecePosition(currentTile.transform);

                                }
                            }
                            else //if clicked tile is not valid
                            {
                                //Put back the piece
                                ResetPiecePosition();
                            }
                        }

                    }
                }
            }
        }

    }

    private void SetPiecePosition(Transform tile)
    {
        currentPiece.transform.SetParent(tile, false);
        Vector2Int prevPosition = new Vector2Int(currentPiece.GetComponent<ChessPiece>().currentX, currentPiece.GetComponent<ChessPiece>().currentY);
        pieceMap[currentPiece.GetComponent<ChessPiece>().currentX, currentPiece.GetComponent<ChessPiece>().currentY] = null;

        currentPiece.GetComponent<ChessPiece>().currentX = (int)tile.transform.position.x;
        currentPiece.GetComponent<ChessPiece>().currentY = (int)tile.transform.position.z;
        Vector2Int newPosition = new Vector2Int(currentPiece.GetComponent<ChessPiece>().currentX, currentPiece.GetComponent<ChessPiece>().currentY);

        pieceMap[currentPiece.GetComponent<ChessPiece>().currentX, currentPiece.GetComponent<ChessPiece>().currentY] = currentPiece.GetComponent<ChessPiece>();
        currentPiece.transform.position = new Vector3(currentPiece.transform.position.x, currentPiece.transform.position.y - 1, currentPiece.transform.position.z);

        ResetTileAfterHighlight();
        initialMaterial = tileMap[currentPiece.GetComponent<ChessPiece>().currentX, currentPiece.GetComponent<ChessPiece>().currentY].GetComponent<MeshRenderer>().material;
        gameManager.SwitchTurn();

        moveList.Add(new Vector2Int[] { prevPosition, newPosition });
        ProcessSpecialMove();

        currentPiece = null;
    }

    private void ResetPiecePosition()
    {
        currentPiece.transform.position = new Vector3(currentPiece.transform.position.x, currentPiece.transform.position.y - 1, currentPiece.transform.position.z);
        ResetTileAfterHighlight();
        currentPiece = null;
    }

    private Material[,] GetHighlightInitialMaterial(List<Vector2Int> tilesPosition)
    {
        Material[,] initials = new Material[TILE_X_COUNT, TILE_Y_COUNT];
        foreach (Vector2Int pos in tilesPosition)
        {
            initials[pos.x, pos.y] = tileMap[pos.x, pos.y].GetComponent<MeshRenderer>().material;
        }

        return initials;
    }

    private void HighlightMoveTile(List<Vector2Int> tilesPosition)
    {
        foreach (Vector2Int pos in tilesPosition)
        {
            tileMap[pos.x, pos.y].GetComponent<MeshRenderer>().material = highlightMaterial;
        }
    }

    private void ResetTileAfterHighlight()
    {
        foreach (Vector2Int pos in availableMoves)
        {
            tileMap[pos.x, pos.y].GetComponent<MeshRenderer>().material = highlight_initialMaterial[pos.x, pos.y];
        }
    }

    private void ProcessSpecialMove()
    {
        if(specialMove == SpecialMove.EnPassant)
        {

        }
    }

}
