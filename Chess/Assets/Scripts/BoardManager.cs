using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

public class BoardManager : MonoBehaviour
{
    public const int TILE_X_COUNT = 8;
    public const int TILE_Y_COUNT = 8;
    const string TILE_TAG = "Tile";

    public GameObject[,] tileMap;
    GameManager gameManager;

    //hovering
    public Material hoverMaterial;
    GameObject currentTile;
    Material initialMaterial;

    //piece movement
    public ChessPiece[,] pieceMap;
    private GameObject currentPiece;
    private List<Vector2Int> availableMoves;
    public List<Vector2Int[]> moveList = new List<Vector2Int[]>(); //To keep track of move record
    private Vector3 whiteDeadLastPos = Vector3.zero;
    private Vector3 blackDeadLastPos = Vector3.zero;

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
    public Material killerMoveMaterial;

    //promotion requirements
    public PromotionPanel promotionPanel;

    //AI
    private GameObject AIChosenTile;

    private void Start()
    {
        pieceMap = FindObjectOfType<BoardGenerator>().chessPieces;
        FindObjectOfType<AI>().GenerateTree(pieceMap);
        tileMap = FindObjectOfType<BoardGenerator>().board;
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if (gameManager.gameIsActive)
        {
            if(gameManager.turnState == GameManager.TurnState.WhiteTurn || (gameManager.turnState == GameManager.TurnState.BlackTurn && !gameManager.isVsAI))
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
                                        PreventCheck();
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
                                            if (CheckEatenPieceIsKing(eatenPiece))
                                            {
                                                gameManager.CheckMate(currentPiece.GetComponent<ChessPiece>().team);
                                                return;
                                            }

                                            AddPieceToGraveyard(eatenPiece);
                                            //Set the new position of the current piece
                                            SetPiecePosition(currentTile.transform);

                                           

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
            //if it is AI black turn
            //else if(gameManager.turnState == GameManager.TurnState.BlackTurn && gameManager.isVsAI)
            //{
            //    if (currentPiece == null && AIChosenTile == null)
            //    {
            //        var aiMove = FindObjectOfType<AI>().GetAIMove(pieceMap);
            //        currentPiece = aiMove.piece.gameObject;
            //        AIChosenTile = tileMap[aiMove.tile.x, aiMove.tile.y];

            //        currentPiece.transform.position = new Vector3(currentPiece.transform.position.x, currentPiece.transform.position.y + 0.5f, currentPiece.transform.position.z);
            //    }
            //    else
            //    {
            //        if(AIChosenTile.transform.childCount == 1)
            //        {
            //            GameObject eatenPiece = AIChosenTile.transform.GetChild(0).gameObject;
            //            StartCoroutine(SetAIPiecePosition(AIChosenTile.transform, eatenPiece));
            //        }
            //        else
            //        {
            //            StartCoroutine(SetAIPiecePosition(AIChosenTile.transform, null));
            //        }
            //    }
            //}
            
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
                if (whiteDeadLastPos.x <= -3)
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
                if (blackDeadLastPos.x >= 9)
                {
                    blackDeadLastPos.x = 8;
                    blackDeadLastPos.z += 0.5f;
                }
                Vector3 position = new Vector3(blackDeadLastPos.x + 0.5f, 0, blackDeadLastPos.z);
                eatenPiece.transform.position = position;
                blackDeadLastPos = position;
            }

        }

        pieceMap[eatenPiece.transform.GetComponent<ChessPiece>().currentX, eatenPiece.transform.GetComponent<ChessPiece>().currentY] = null;
    }

    private bool CheckEatenPieceIsKing(GameObject eatenPiece)
    {
        if (eatenPiece.GetComponent<ChessPiece>().type == ChessPieceType.King)
        {
            return true;
        }

        return false;
    }

    private void SetPiecePosition(Transform tile)
    {
        FindObjectOfType<AudioManager>().GetMovePieceAudio();
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

        moveList.Add(new Vector2Int[] { prevPosition, newPosition });
        ProcessSpecialMove(specialMove);

        if (CheckForCheckmate())
        {
            gameManager.CheckMate(currentPiece.GetComponent<ChessPiece>().team);
        }

        gameManager.SwitchTurn();
        currentPiece = null;
    }

    private IEnumerator SetAIPiecePosition(Transform tile, GameObject eatenPiece)
    {
        yield return new WaitForSeconds(0.5f);
        if (currentPiece != null)
        {
            FindObjectOfType<AudioManager>().GetMovePieceAudio();
            currentPiece.transform.SetParent(tile, false);

            if (eatenPiece != null)
            {
                if (CheckEatenPieceIsKing(eatenPiece))
                {
                    gameManager.CheckMate(currentPiece.GetComponent<ChessPiece>().team);
                    yield return null;
                }

                AddPieceToGraveyard(eatenPiece);
            }

            Vector2Int prevPosition = new Vector2Int(currentPiece.GetComponent<ChessPiece>().currentX, currentPiece.GetComponent<ChessPiece>().currentY);
            pieceMap[currentPiece.GetComponent<ChessPiece>().currentX, currentPiece.GetComponent<ChessPiece>().currentY] = null;

            currentPiece.GetComponent<ChessPiece>().currentX = (int)tile.transform.position.x;
            currentPiece.GetComponent<ChessPiece>().currentY = (int)tile.transform.position.z;
            Vector2Int newPosition = new Vector2Int(currentPiece.GetComponent<ChessPiece>().currentX, currentPiece.GetComponent<ChessPiece>().currentY);

            pieceMap[currentPiece.GetComponent<ChessPiece>().currentX, currentPiece.GetComponent<ChessPiece>().currentY] = currentPiece.GetComponent<ChessPiece>();
            currentPiece.transform.position = new Vector3(currentPiece.transform.position.x, currentPiece.transform.position.y - 0.5f, currentPiece.transform.position.z);
            moveList.Add(new Vector2Int[] { prevPosition, newPosition });
            ProcessSpecialMove(specialMove);
            if (CheckForCheckmate())
            {
                gameManager.CheckMate(currentPiece.GetComponent<ChessPiece>().team);
            }

            gameManager.SwitchTurn();

            AIChosenTile = null;
            currentPiece = null;
        }
        
    }

    private void ResetPiecePosition()
    {
        FindObjectOfType<AudioManager>().GetMovePieceAudio();
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
            if(highlight_initialMaterial.Length > 0)
            {
                tileMap[pos.x, pos.y].GetComponent<MeshRenderer>().material = highlight_initialMaterial[pos.x, pos.y];
            }

        }
    }

    public void ProcessSpecialMove(BoardManager.SpecialMove specialMove)
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
            if (gameManager.gameIsActive)
            {
                var lastMove = moveList[moveList.Count - 1];
                var targetPawn = pieceMap[lastMove[1].x, lastMove[1].y];

                if (targetPawn.type == ChessPieceType.Pawn)
                {
                    //if the target pawn is white pawn and lastmove is on the other side
                    if (targetPawn.team == 0 && lastMove[1].y == 7)
                    {
                        promotionPanel.SpawnPiecesButtons(0);
                    }
                    //if the target pawn is black pawn and lastmove is on the other side
                    else if (targetPawn.team == 1 && lastMove[1].y == 0)
                    {
                        if (!gameManager.isVsAI)
                        {
                            promotionPanel.SpawnPiecesButtons(1);
                        }
                        else
                        {
                            //If it is AI that get a promotion, pick random piece
                            var pieces = (from piece in GetComponent<BoardGenerator>().chessPiecePrefabs
                                          where piece.GetComponent<ChessPiece>().type != ChessPieceType.Pawn
                                          select piece.GetComponent<ChessPiece>().type).ToList();

                            int random = Random.Range(0, pieces.Count());
                            GetPromotionPiece(pieces[random], 1);
                        }

                    }
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
                        rook.currentX = 3;
                        rook.currentY = 0;
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
                        rook.currentX = 3;
                        rook.currentY = 7;
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
                        rook.currentX = 5;
                        rook.currentY = 0;
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
                        rook.currentX = 5;
                        rook.currentY = 7;
                    }
                }
            }
        }
    }

    public void GetPromotionPiece(ChessPieceType type, int team)
    {
        var lastMove = moveList[moveList.Count - 1];

        //Destroy the pawn
        var pawn = pieceMap[lastMove[1].x, lastMove[1].y];
        Destroy(pawn.transform.gameObject);

        //Spawn the new piece
        pieceMap[lastMove[1].x, lastMove[1].y] = transform.GetComponent<BoardGenerator>().SpawnSinglePiece(type, team);
        //Set the piece to the last position
        GameObject piece = pieceMap[lastMove[1].x, lastMove[1].y].transform.gameObject;
        piece.transform.SetParent(tileMap[lastMove[1].x, lastMove[1].y].transform, false);
        piece.GetComponent<ChessPiece>().currentX = lastMove[1].x;
        piece.GetComponent<ChessPiece>().currentY = lastMove[1].y;

        gameManager.gameIsActive = true;
        promotionPanel.transform.parent.parent.gameObject.SetActive(false);
    }

    //Prevent Check and Checkmate
    public void PreventCheck()
    {
        ChessPiece targetKing = null;

        for (int x = 0; x < TILE_X_COUNT; x++)
        {
            for(int y =0; y< TILE_Y_COUNT; y++)
            {
                if (pieceMap[x, y] != null)
                {
                    if (pieceMap[x, y].type == ChessPieceType.King)
                    {
                        if (pieceMap[x, y].team == currentPiece.GetComponent<ChessPiece>().team)
                        {
                            targetKing = pieceMap[x, y];
                        }
                    }
                }
               
            }
        }

        //Deleting moves that will put us in check
        SimulateMoveForSinglePiece(currentPiece.GetComponent<ChessPiece>(), ref availableMoves, targetKing);
    }

    public void SimulateMoveForSinglePiece(ChessPiece cp, ref List<Vector2Int> moves, ChessPiece targetKing)
    {
        //Save the current value, reset after the function called
        int actualX = cp.currentX;
        int actualY = cp.currentY;
        List<Vector2Int> movesToRemove = new List<Vector2Int>();

        //Going through all the moves, simulate them and check if we are in check
        for (int i = 0; i < moves.Count; i++)
        {
            int simX = moves[i].x;
            int simY = moves[i].y;

            Vector2Int kingPosSim = new Vector2Int(targetKing.currentX, targetKing.currentY);
            //Did we simulate the king move
            if(cp.type == ChessPieceType.King)
            {
                kingPosSim = new Vector2Int(simX, simY);
            }

            ChessPiece[,] simulation = new ChessPiece[TILE_X_COUNT, TILE_Y_COUNT];
            List<ChessPiece> simAttackingPieces = new List<ChessPiece>();
            for (int x = 0; x < TILE_X_COUNT; x++)
            {
                for (int y = 0; y < TILE_Y_COUNT; y++)
                {
                    if (pieceMap[x, y] != null)
                    {
                        simulation[x, y] = pieceMap[x, y];
                        if(simulation[x,y].team != cp.team)
                        {
                            //Get all the other team chess position values
                            simAttackingPieces.Add(simulation[x, y]);
                        }
                    }
                }
            }

            //Simulate move
            simulation[actualX, actualY] = null;
            cp.currentX = simX;
            cp.currentY = simY;
            simulation[simX, simY] = cp;

            //Did one of the piece got taken down during the simulation
            var deadPiece = simAttackingPieces.Find(c => c.currentX == simX && c.currentY == simY);
            if (deadPiece != null)
            {
                simAttackingPieces.Remove(deadPiece);
            }

            //Get all the simulated attacking piece move
            List<Vector2Int> simMoves = new List<Vector2Int>();
            for (int j = 0; j < simAttackingPieces.Count; j++)
            {
                var pieceMove = simAttackingPieces[j].GetAvailableMoves(ref simulation, TILE_X_COUNT, TILE_Y_COUNT);
                for (int k = 0; k < pieceMove.Count; k++)
                {
                    simMoves.Add(pieceMove[k]);
                }
            }

            //If the king is in trouble? remove from the availabe move
            if(ContainsValidMove(ref simMoves, kingPosSim))
            {
                movesToRemove.Add(moves[i]);
            }

            //Restore the actual cp Data
            cp.currentX = actualX;
            cp.currentY = actualY;
        }

        //Remove from the available current move list
        for (int i = 0; i < movesToRemove.Count; i++)
        {
            moves.Remove(movesToRemove[i]);
        }
    }

    public bool ContainsValidMove(ref List<Vector2Int> moves, Vector2Int piecePos)
    {
        for (int i = 0; i < moves.Count; i++)
        {
            if(moves[i].x == piecePos.x  && moves[i].y == piecePos.y)
            {
                return true;
            }
        }

        return false;
    }

    private bool CheckForCheckmate()
    {
        var lastMove = moveList[moveList.Count - 1];
        int targetTeam = (pieceMap[lastMove[1].x, lastMove[1].y].team == 0 ? 1 : 0);

        List<ChessPiece> attackingPieces = new List<ChessPiece>();
        List<ChessPiece> defendingPieces = new List<ChessPiece>();

        ChessPiece targetKing = null;
        for (int x = 0; x < TILE_X_COUNT; x++)
        {
            for (int y = 0; y < TILE_Y_COUNT; y++)
            {
                if (pieceMap[x, y] != null)
                {
                    if(pieceMap[x,y].team == targetTeam)
                    {
                        defendingPieces.Add(pieceMap[x, y]);
                        if(pieceMap[x,y].type == ChessPieceType.King)
                        {
                            targetKing = pieceMap[x, y];
                        }
                    }
                    else
                    {
                        attackingPieces.Add(pieceMap[x, y]);
                    }
                }
            }
        }

        //If the king attacked right now
        List<Vector2Int> currentAvailableMoves = new List<Vector2Int>();
        for (int i = 0; i < attackingPieces.Count; i++)
        {
            var pieceMoves = attackingPieces[i].GetAvailableMoves(ref pieceMap, TILE_X_COUNT, TILE_Y_COUNT);
            for (int j = 0; j < pieceMoves.Count; j++)
            {
                currentAvailableMoves.Add(pieceMoves[j]);
            }
        }

        //If the attacking move contains a king (are we in check right now)
        if(ContainsValidMove(ref currentAvailableMoves, new Vector2Int(targetKing.currentX, targetKing.currentY)))
        {
            //King is under attack, can we help him?
            for (int i = 0; i < defendingPieces.Count; i++)
            {
                List<Vector2Int> defendingMoves = defendingPieces[i].GetAvailableMoves(ref pieceMap, TILE_X_COUNT, TILE_Y_COUNT);
                SimulateMoveForSinglePiece(defendingPieces[i], ref defendingMoves, targetKing);

                //if defending moves is empty, then we are in checkmate
                if(defendingMoves.Count != 0)
                {
                    return false;
                }
            }

            return true; //Check mate exist
        }

        return false;

    }

}
