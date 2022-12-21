using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool gameIsActive;

    public enum TurnState
    {
        WhiteTurn = 0,
        BlackTurn = 1
    }

    public TurnState turnState = TurnState.WhiteTurn;
    [SerializeField]private WinnerPanel winnerPanel;

    public void SwitchTurn()
    {
        if(turnState == TurnState.WhiteTurn)
        {
            turnState = TurnState.BlackTurn;
        }
        else
        {
            turnState = TurnState.WhiteTurn;
        }
    }

    public void CheckWinConditions(ChessPieceType type, int team)
    {
        if(type == ChessPieceType.King)
        {
            CheckMate(team);
        }
    }

    private void CheckMate(int team)
    {
        gameIsActive = false;
        winnerPanel.transform.gameObject.SetActive(true);
        winnerPanel.SetWinnerText(team);
    }
}
