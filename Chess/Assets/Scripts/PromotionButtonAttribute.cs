using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PromotionButtonAttribute : MonoBehaviour
{
    private PromotionPanel.ChessPieceButtons pieceButtons;
    private Button button;
    private BoardManager boardManager;
    private PromotionPanel promotionPanel;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    public void SetAttribute(PromotionPanel.ChessPieceButtons attribute, BoardManager boardManager, PromotionPanel promPanel)
    {
        pieceButtons = attribute;
        this.boardManager = boardManager;
        promotionPanel = promPanel;
    }

    public void OnButtonClick()
    {
        var type = pieceButtons.type;
        boardManager.SetPromotionType(type);
        promotionPanel.ClosePromotionPanel();
    }

}
