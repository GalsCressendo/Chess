using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WinnerPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI winnerText;

    public void SetWinnerText(int team)
    {
        if(team == 0)
        {
            winnerText.text = ("BLACK TEAM WIN !");
            winnerText.color = Color.black;
        }
        else
        {
            winnerText.text = ("WHITE TEAM WIN !");
            winnerText.color = Color.white;
        }
    }
}
