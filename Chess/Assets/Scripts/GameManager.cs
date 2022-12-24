using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool gameIsActive;
    public bool isVsAI;
    [SerializeField] private GameObject pauseButton;

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

    public void CheckMate(int team)
    {
        gameIsActive = false;
        pauseButton.SetActive(false);
        winnerPanel.transform.gameObject.SetActive(true);
        winnerPanel.SetWinnerText(team);
    }
}
