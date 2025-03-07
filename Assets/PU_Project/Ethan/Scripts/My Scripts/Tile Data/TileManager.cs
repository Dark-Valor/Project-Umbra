using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance;
    public Tilemap tilemap;

    // Stores tile metadata (ONLY Type, NOT Orientation)
    public Dictionary<Vector3Int, TileData> tileMetadata;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        tileMetadata = new Dictionary<Vector3Int, TileData>();
    }

    // Get TileData at a position (no longer includes orientation)
    public TileData GetTileData(Vector3Int position)
    {
        if (tileMetadata.ContainsKey(position))
        {
            return tileMetadata[position];
        }
        return null;
    }

    // Assign tile metadata (Only storing type)
    public void AssignTileMetadata(Vector3Int position, TileData baseTileData)
    {
        tileMetadata[position] = baseTileData;
    }

    // Debugging functions
    public int GetMetadataCount()
    {
        return tileMetadata.Count;
    }

    public void PrintAllTileMetadata()
    {
        Debug.Log("===== TILE METADATA STORED =====");
        foreach (var entry in tileMetadata)
        {
            Debug.Log($"Tile at {entry.Key}: {entry.Value.type}");
        }
    }
}
