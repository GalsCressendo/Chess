using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PromotionButtonAttribute : MonoBehaviour
{
    public ChessPieceType type;
    public int team;
    Button button;
    BoardManager boardManager;

    private void Awake()
    {
        boardManager = FindObjectOfType<BoardManager>();
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        boardManager.GetPromotionPiece(type, team);
    }
}


