using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    //promotion requirements
    private Vector3 whiteDeadLastPos = Vector3.zero;
    private Vector3 blackDeadLastPos = Vector3.zero;
    private List<GameObject> whiteGraveyard = new List<GameObject>();
    private List<GameObject> blackGraveyard = new List<GameObject>();
    private List<GameObject> whitePromotion = new List<GameObject>();
    private List<GameObject> blackPromotion = new List<GameObject>();
    [SerializeField] private PromotionPanel promotionPanel;
    private GameObject promotionPawn;
    private int promotionTeam = -1;

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
                                    currentPiece.transform.position = new Vector3(currentPiece.transform.position.x, currentPiece.transform.position.y + 0.5f, currentPiece.transform.position.z);

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

                                        //Add the piece to the graveyard
                                        AddPieceToGraveyard(eatenPiece);

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

    private void AddPieceToGraveyard(GameObject eatenPiece)
    {
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

            whiteGraveyard.Add(eatenPiece);
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

            blackGraveyard.Add(eatenPiece);
        }

        pieceMap[eatenPiece.transform.GetComponent<ChessPiece>().currentX, eatenPiece.transform.GetComponent<ChessPiece>().currentY] = null;
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
        currentPiece.transform.position = new Vector3(currentPiece.transform.position.x, currentPiece.transform.position.y - 0.5f, currentPiece.transform.position.z);

        ResetTileAfterHighlight();
        initialMaterial = tileMap[currentPiece.GetComponent<ChessPiece>().currentX, currentPiece.GetComponent<ChessPiece>().currentY].GetComponent<MeshRenderer>().material;
        gameManager.SwitchTurn();

        moveList.Add(new Vector2Int[] { prevPosition, newPosition });
        ProcessSpecialMove();

        currentPiece = null;
    }

    private void ResetPiecePosition()
    {
        currentPiece.transform.position = new Vector3(currentPiece.transform.position.x, currentPiece.transform.position.y - 0.5f, currentPiece.transform.position.z);
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
            var newMove = moveList[moveList.Count - 1]; //To check if we actually do the en passant
            ChessPiece myPawn = pieceMap[newMove[1].x, newMove[1].y];

            var targetPawnPosition = moveList[moveList.Count - 2];
            ChessPiece enemyPawn = pieceMap[targetPawnPosition[1].x, targetPawnPosition[1].y];

            //the pawns need to be in the same x position since we jump over it
            if(myPawn.currentX == enemyPawn.currentX)
            {
                //check if my pawn is diagonal to enemy pawn
                if(myPawn.currentY == enemyPawn.currentY -1 || myPawn.currentY == enemyPawn.currentY + 1)
                {
                    //the enemy piece is valid and add it to the graveyard
                    AddPieceToGraveyard(enemyPawn.transform.gameObject);
                }
            }
        }

        if(specialMove == SpecialMove.Promotion)
        {
            var lastMove = moveList[moveList.Count - 1];
            var targetPawn = pieceMap[lastMove[1].x, lastMove[1].y];

            if(targetPawn.type == ChessPieceType.Pawn)
            {
                //if the target pawn is white pawn and lastmove is on the other side
                if(targetPawn.team == 0 && lastMove[1].y == 7)
                {
                    GetPromotionChessPieces(0);
                    promotionPawn = targetPawn.transform.gameObject;
                    promotionTeam = 0;
                    promotionPanel.SpawnChessPiecePromotionButtons(whitePromotion, 0);


                }
                //if the target pawn is black pawn and lastmove is on the other side
                else if (targetPawn.team == 1 && lastMove[1].y == 0)
                {
                    GetPromotionChessPieces(1);
                    promotionPawn = targetPawn.transform.gameObject;
                    promotionTeam = 1;
                    promotionPanel.SpawnChessPiecePromotionButtons(blackPromotion,1);

                }


            }
        }

        if(specialMove == SpecialMove.Castling)
        {
            var lastMove = moveList[moveList.Count - 1];
            //Left rook
            if(lastMove[1].x == 2)
            {
                //if we are on the white side
                if (lastMove[1].y == 0)
                {
                    var rook = pieceMap[0, 0];
                    pieceMap[3, 0] = rook;
                    pieceMap[0, 0] = null;
                    if (currentPiece.transform.GetComponent<King>() != null)
                    {
                        currentPiece.transform.GetComponent<King>().SetRookPosAfterCastling(tileMap[3, 0].transform, rook.transform);
                    }
                }
                //if we are on the black side
                else if (lastMove[1].y == 7)
                {
                    var rook = pieceMap[0, 7];
                    pieceMap[3, 7] = rook;
                    pieceMap[0, 7] = null;
                    if (currentPiece.transform.GetComponent<King>() != null)
                    {
                        currentPiece.transform.GetComponent<King>().SetRookPosAfterCastling(tileMap[3, 7].transform, rook.transform);
                    }
                }
            }
            //Right rook
            else if (lastMove[1].x == 6)
            {
                //if we are on the white side
                if (lastMove[1].y == 0)
                {
                    var rook = pieceMap[7, 0];
                    pieceMap[5, 0] = rook;
                    pieceMap[7, 0] = null;
                    if (currentPiece.transform.GetComponent<King>() != null)
                    {
                        currentPiece.transform.GetComponent<King>().SetRookPosAfterCastling(tileMap[5, 0].transform, rook.transform);
                    }
                }
                //if we are on the black side
                else if (lastMove[1].y == 7)
                {
                    var rook = pieceMap[7, 7];
                    pieceMap[5, 7] = rook;
                    pieceMap[7, 7] = null;
                    if (currentPiece.transform.GetComponent<King>() != null)
                    {
                        currentPiece.transform.GetComponent<King>().SetRookPosAfterCastling(tileMap[5, 7].transform, rook.transform);
                    }
                }
            }
        }
    }

    private void GetPromotionChessPieces(int team)
    {
        if(team == 0)
        {
            foreach(GameObject obj in whiteGraveyard)
            {
                if(obj.GetComponent<ChessPiece>().type != ChessPieceType.Pawn)
                {
                    whitePromotion.Add(obj);
                }
            }
        }
        else
        {
            foreach (GameObject obj in blackGraveyard)
            {
                if (obj.GetComponent<ChessPiece>().type != ChessPieceType.Pawn)
                {
                    blackPromotion.Add(obj);
                }
            }
        }
    }

    public void SetPromotionType(ChessPieceType type)
    {
        SwapPiecePromotion(type, promotionPawn, promotionTeam);
    }

    public void SwapPiecePromotion(ChessPieceType type, GameObject pawn, int team)
    {
        GameObject swapGameObject = null;
        if(team == 0)
        {
            var promotion = (from obj in whitePromotion
                             where obj.GetComponent<ChessPiece>().type == type
                             select obj).First();
            swapGameObject = promotion;
        }
        else
        {
            var promotion = (from obj in blackPromotion
                             where obj.GetComponent<ChessPiece>().type == type
                             select obj).First();
            swapGameObject = promotion;
        }

        if (swapGameObject != null)
        {
            var lastMove = moveList[moveList.Count - 1];

            pieceMap[lastMove[1].x, lastMove[1].y] = swapGameObject.GetComponent<ChessPiece>();
            var targetTile = tileMap[lastMove[1].x, lastMove[1].y];

            pawn.transform.SetParent(null, false);
            Vector3 tmp = swapGameObject.transform.position;
            pawn.transform.position = tmp;
            pawn.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

            swapGameObject.transform.SetParent(targetTile.transform);
            swapGameObject.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            swapGameObject.transform.localPosition= new Vector3(0, 0.5f, 0);


            swapGameObject.transform.GetComponent<ChessPiece>().currentX = (int)swapGameObject.transform.position.x;
            swapGameObject.transform.GetComponent<ChessPiece>().currentY = (int)swapGameObject.transform.position.z;

            promotionPawn = null;
            promotionTeam = -1;
        }

    }

   

}
