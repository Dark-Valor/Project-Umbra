using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AllTilesLocations : MonoBehaviour
{
    public Tilemap tilemap; // Assign your Tilemap in the Inspector

    void Start()
    {
        List<Vector3> worldPositions = GetAllTileWorldPositions();

        foreach (Vector3 position in worldPositions)
        {
            Debug.Log("Tile World Position: " + position);
        }
    }

    List<Vector3> GetAllTileWorldPositions()
    {
        List<Vector3> positions = new List<Vector3>();

        // Iterate through all tiles in the tilemap bounds
        BoundsInt bounds = tilemap.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);

                // Check if a tile is set at this position
                if (tilemap.HasTile(tilePosition))
                {
                    // Convert to world position
                    Vector3 worldPosition = tilemap.CellToWorld(tilePosition);
                    positions.Add(worldPosition);
                }
            }
        }

        return positions;
    }
}
