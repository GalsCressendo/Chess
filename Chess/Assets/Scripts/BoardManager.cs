using UnityEngine;

public class BoardManager : MonoBehaviour
{
    const string TILE_TAG = "Tile";

    public Material hoverMaterial;
    GameObject currentTile;
    Material initialMaterial;
    ChessPiece[,] pieceMap;

    //piece movement
    private GameObject currentPiece;
    private Vector3 whiteDeadLastPos = Vector3.zero;
    private Vector3 blackDeadLastPos = Vector3.zero;

    private void Start()
    {
        pieceMap = FindObjectOfType<BoardGenerator>().chessPieces;
    }

    private void Update()
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

                    //if not holding a piece
                    if (currentPiece == null)
                    {
                        if (currentTile.transform.childCount == 1)
                        {
                            currentPiece = currentTile.transform.GetChild(0).gameObject;
                            currentPiece.transform.position = new Vector3(currentPiece.transform.position.x, currentPiece.transform.position.y + 1, currentPiece.transform.position.z);
                        }
                    }
                    //if holding a piece
                    else if (currentPiece != null)
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

                            }
                            else if(currentPiece.GetComponent<ChessPiece>().team == currentTile.transform.GetChild(0).GetComponent<ChessPiece>().team)
                            {
                                currentPiece.transform.position = new Vector3(currentPiece.transform.position.x, currentPiece.transform.position.y - 1, currentPiece.transform.position.z);
                                currentPiece = null;
                            }

                        }
                        else
                        {

                            //Set the new position of the current piece
                            SetPiecePosition(currentTile.transform);

                            currentPiece = null;
                        }

                    }

                }
            }
        }
    }

    private void SetPiecePosition(Transform tile)
    {
        currentPiece.transform.SetParent(tile, false);
        pieceMap[currentPiece.GetComponent<ChessPiece>().currentX, currentPiece.GetComponent<ChessPiece>().currentY] = null;

        currentPiece.GetComponent<ChessPiece>().currentX = (int)tile.transform.position.x;
        currentPiece.GetComponent<ChessPiece>().currentY = (int)tile.transform.position.z;

        pieceMap[currentPiece.GetComponent<ChessPiece>().currentX, currentPiece.GetComponent<ChessPiece>().currentY] = currentPiece.GetComponent<ChessPiece>();
        currentPiece.transform.position = new Vector3(currentPiece.transform.position.x, currentPiece.transform.position.y - 1, currentPiece.transform.position.z);


        currentPiece = null;
    }


}
