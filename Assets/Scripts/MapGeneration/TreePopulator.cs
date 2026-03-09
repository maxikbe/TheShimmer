using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TreePopulator : MonoBehaviour
{
    [Header("References")]
    public Tilemap groundTilemap;
    public List<GameObject> treePrefabs;
    public Transform treeContainer;

    [Header("Settings")]
    public int targetTileID = 1; 
    public int spawnRadius = 3;
    [Range(0, 1)] public float density = 0.4f;

    public void GrowForestOnSpecialTiles(int[,] gridData)
    {
        for (int x = 0; x < gridData.GetLength(0); x++)
        {
            for (int y = 0; y < gridData.GetLength(1); y++)
            {
                if (gridData[x, y] == targetTileID)
                {
                    Vector3 worldPos = groundTilemap.CellToWorld(new Vector3Int(x, y, 0));
                    GrowForestAtPoint(worldPos);
                }
            }
        }
    }

    public void GrowForestAtPoint(Vector3 worldPosition)
    {
        Vector3Int centerCell = groundTilemap.WorldToCell(worldPosition);

        for (int x = -spawnRadius; x <= spawnRadius; x++)
        {
            for (int y = -spawnRadius; y <= spawnRadius; y++)
            {
                if (x * x + y * y <= spawnRadius * spawnRadius)
                {
                    Vector3Int currentPos = new Vector3Int(centerCell.x + x, centerCell.y + y, 0);
                    TryPlaceTree(currentPos);
                }
            }
        }
    }

    private void TryPlaceTree(Vector3Int pos)
    {
        TileBase groundTile = groundTilemap.GetTile(pos);
        if (groundTile == null) return;

        if (Random.value > density) return;

        Vector3 spawnPos = groundTilemap.GetCellCenterWorld(pos);

        GameObject randomTree = treePrefabs[Random.Range(0, treePrefabs.Count)];
        Instantiate(randomTree, spawnPos, Quaternion.identity, treeContainer);
    }
}