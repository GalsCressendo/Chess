using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum TurnState
    {
        WhiteTurn = 0,
        BlackTurn = 1
    }

    public TurnState turnState = TurnState.WhiteTurn;

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
}
