using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarGenerator : MonoBehaviour
{
    const int TILE_X_COUNT = 8;
    const int TILE_Y_COUNT = 8;

    public GameObject whiteTile;
    public GameObject brownTile;
    private GameObject startTile;
    private GameObject currentTile;
    public GameObject[,] board = new GameObject[TILE_X_COUNT, TILE_Y_COUNT];

    private void Awake()
    {
        GenerateAllTiles();
    }

    private void GenerateAllTiles()
    {
        for(int x = 0; x< TILE_X_COUNT; x++)
        {
            if(startTile == null)
            {
                startTile = brownTile;
            }
            else if (startTile == brownTile)
            {
                startTile = whiteTile;
            }
            else if(startTile = whiteTile)
            {
                startTile = brownTile;
            }

            for(int y=0; y < TILE_Y_COUNT; y++)
            {
                if (y == 0)
                {
                    currentTile = startTile;
                }
                else if(currentTile == brownTile)
                {
                    currentTile = whiteTile;
                }
                else if (currentTile == whiteTile)
                {
                    currentTile = brownTile;
                }

                Vector3 spawnPos = new Vector3(x, 0, y);
                GameObject tile = Instantiate(currentTile, spawnPos, Quaternion.identity);
                tile.transform.SetParent(transform, false);
                tile.name = string.Format("X:{0},Y:{1}", x, y);
            }
        }

    }
}
