using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    const string TILE_TAG = "Tile";

    public Material hoverMaterial;
    GameObject currentTile;
    Material initialMaterial;

    private void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit))
        {
            if(hit.transform.tag == TILE_TAG)
            {

                if(currentTile == null)
                {
                    currentTile = hit.transform.gameObject;
                    initialMaterial = hit.transform.GetComponent<MeshRenderer>().material;
                    currentTile.GetComponent<MeshRenderer>().material = hoverMaterial;
                }
                else if (currentTile != hit.transform.gameObject)
                {
                    currentTile.GetComponent<MeshRenderer>().material = initialMaterial;
                    initialMaterial = hit.transform.GetComponent<MeshRenderer>().material;
                    currentTile = hit.transform.gameObject;
                    currentTile.GetComponent<MeshRenderer>().material = hoverMaterial;
                }
            }
        }
    }
}
