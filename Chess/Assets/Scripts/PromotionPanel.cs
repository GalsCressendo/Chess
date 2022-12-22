using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class PromotionPanel : MonoBehaviour
{
    public List<GameObject> buttons;
    public GameManager gameManager;

    public void SpawnPiecesButtons(int team)
    {
        gameManager.gameIsActive = false;
        transform.parent.parent.gameObject.SetActive(true);

        foreach(GameObject obj in buttons)
        {
            var b = obj.GetComponent<PromotionButtonAttribute>();
            if(b.team == team)
            {
                Instantiate(obj.transform.gameObject, transform);
            }
        }
    }
}
