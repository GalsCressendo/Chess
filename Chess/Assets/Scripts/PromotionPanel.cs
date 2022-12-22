using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class PromotionPanel : MonoBehaviour
{
    public List<GameObject> buttons;
    public GameManager gameManager;
    public List<GameObject> spawnedButtons;

    public void SpawnPiecesButtons(int team)
    {
        gameManager.gameIsActive = false;
        transform.parent.parent.gameObject.SetActive(true);

        foreach(GameObject obj in buttons)
        {
            var b = obj.GetComponent<PromotionButtonAttribute>();
            if(b.team == team)
            {
                spawnedButtons.Add(Instantiate(obj.transform.gameObject, transform));
            }
        }
    }

    private void OnDisable()
    {
        foreach(GameObject obj in spawnedButtons)
        {
            Destroy(obj);
        }

        spawnedButtons = new List<GameObject>();
    }
}
