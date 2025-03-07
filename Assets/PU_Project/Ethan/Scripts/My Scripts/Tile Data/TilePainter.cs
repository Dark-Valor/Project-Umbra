using UnityEngine;
using UnityEngine.Tilemaps;

public class TilePainter : MonoBehaviour
{
    public Tilemap tilemap;
    public TileManager tileManager;
    public TileData groundTileData;
    public TileData umbralTileData;

    void Start()
    {
        PaintTiles();
    }

    void PaintTiles()
    {
        BoundsInt bounds = tilemap.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);

            if (tile != null)
            {
                TileData baseTileData = null;

                //// Identify whether tile belongs to Ground or Umbral category
                //if (tile.name.Contains("Ground"))
                //{
                //    baseTileData = groundTileData;
                //}
                //else if (tile.name.Contains("Umbral"))
                //{
                //    baseTileData = umbralTileData;
                //}

                baseTileData = umbralTileData;

                if (baseTileData != null)
                {
                    tileManager.AssignTileMetadata(pos, baseTileData); // Store only tile type
                    Debug.Log($"Assigned TileData: {baseTileData.name} at {pos}");
                }
                else
                {
                    Debug.LogWarning($"No TileData assigned at {pos} for tile: {tile.name}");
                }
            }
        }
    }
}
