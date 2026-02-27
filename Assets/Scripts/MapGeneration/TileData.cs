using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "NewTileData", menuName = "WFC/TileData")]
public class TileData : ScriptableObject
{
    public TileBase tile;
    public int up;
    public int down;
    public int left;
    public int right;
    public int weight = 1; 
}