using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "NewTileData", menuName = "WFC/TileData")]
public class TileData : ScriptableObject
{
    public TileBase tile;

    [Header("Valid Neighbor IDs")]
    [Tooltip("List the IDs of tiles that can be placed ABOVE this tile")]
    public List<int> validUp;
    
    [Tooltip("List the IDs of tiles that can be placed BELOW this tile")]
    public List<int> validDown;
    
    [Tooltip("List the IDs of tiles that can be placed to the LEFT of this tile")]
    public List<int> validLeft;
    
    [Tooltip("List the IDs of tiles that can be placed to the RIGHT of this tile")]
    public List<int> validRight;

    [Header("Settings")]
    public int tileID;
    public float weight = 1f;
}