using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tile Data")]
public class TileData : ScriptableObject
{
    [Header("Tiles that use this metadata")]
    public TileBase[] tiles;

    public enum Type
    {
        umbral,
        ground
    };

    [Header("Tile Properties")]
    public Type type;

    [Header("Check if surface tile, avoid processing irrelevant tiles")]
    public bool isGroundTile = false;
}
