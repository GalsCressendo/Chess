using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class PromotionPanel : MonoBehaviour
{
    [Serializable]
    public class ChessPieceButtons
    {
        public ChessPieceType type;
        public int team;
        public GameObject buttonPrefab;
    }

    public List<ChessPieceButtons> chessPieceButtons;
    List<ChessPieceType> spawnType = new List<ChessPieceType>();
    List<GameObject> spawnedButtons = new List<GameObject>();

    [SerializeField] GameManager gameManager;
    public BoardManager boardManager;

    public void SpawnChessPiecePromotionButtons(List<GameObject> promotionPieces, int team)
    {
        foreach(GameObject piece in promotionPieces)
        {
            if (!spawnType.Contains(piece.GetComponent<ChessPiece>().type))
            {
                var button = (from b in chessPieceButtons
                              where (b.type == piece.GetComponent<ChessPiece>().type && b.team == team)
                              select b).First();

                GameObject spawn = Instantiate(button.buttonPrefab, transform);
                spawn.GetComponent<PromotionButtonAttribute>().SetAttribute(button, boardManager, this);
                spawnedButtons.Add(spawn);
                
                spawnType.Add(piece.GetComponent<ChessPiece>().type);
            }
        }

        transform.parent.parent.gameObject.SetActive(true);

    }

    public void ClosePromotionPanel()
    {
        foreach(GameObject obj in spawnedButtons)
        {
            Destroy(obj);
        }

        spawnedButtons = new List<GameObject>();
        gameManager.gameIsActive = true;
        transform.parent.parent.gameObject.SetActive(false);
    }
}
