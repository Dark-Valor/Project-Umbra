using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent (typeof(Tilemap))]
public class ChangeTileColor : MonoBehaviour
{
    [SerializeField]
    Tilemap tilemap; // Assign your Tilemap in the Inspector

    void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPosition = tilemap.WorldToCell(worldPoint);


            tilemap.SetTileFlags(gridPosition,TileFlags.None);
            int result = GetComponent<GridInformation>().GetPositionProperty(gridPosition,"Umbral State",0);
            tilemap.SetColor(gridPosition, Color.white);
            Debug.Log("Grid Pos: " + gridPosition);
            Debug.Log("Umbral State: " + result);
            GetComponent<GridInformation>().SetPositionProperty(gridPosition,"Umbral State",0);
        }
    }
}
