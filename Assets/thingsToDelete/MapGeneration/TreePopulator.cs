using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TreePopulator : MonoBehaviour
{
    [Header("References")]
    public Tilemap groundTilemap;
    public TileBase targetTile;
    public List<GameObject> treePrefabs;
    public Transform treeContainer;

    [Header("Forest Density")]
    [Tooltip("Crank this up for a thick forest!")]
    public int attemptsPerTile = 25; 
    [Range(0, 1)] public float spawnChance = 0.8f;
    
    [Header("Spacing & Shape")]
    public float spawnRadius = 1.5f; 
    public float minDistanceBetweenTrees = 0.25f;
    public float verticalSquish = 0.6f;

    [Header("Visual Variance")]
    public Vector2 scaleRange = new Vector2(0.8f, 1.3f);
    public float baseScale = 1f;

    private Dictionary<Vector2Int, List<Vector3>> spatialGrid = new Dictionary<Vector2Int, List<Vector3>>();

    void Start() => GenerateForests();

    public void GenerateForests()
    {
        foreach (Transform child in treeContainer) { Destroy(child.gameObject); }
        spatialGrid.Clear();

        BoundsInt bounds = groundTilemap.cellBounds;

        foreach (Vector3Int tilePos in bounds.allPositionsWithin)
        {
            if (groundTilemap.GetTile(tilePos) == targetTile)
            {
                Vector3 worldPos = groundTilemap.GetCellCenterWorld(tilePos);
                
                for (int i = 0; i < attemptsPerTile; i++)
                {
                    TrySpawnTree(worldPos, tilePos);
                }
            }
        }
    }

    private void TrySpawnTree(Vector3 centerPos, Vector3Int tilePos)
    {
        if (Random.value > spawnChance) return;

        float angle = Random.value * Mathf.PI * 2;
        float radius = Mathf.Sqrt(Random.value) * spawnRadius; 
        
        Vector3 spawnPos = centerPos + new Vector3(
            Mathf.Cos(angle) * radius, 
            Mathf.Sin(angle) * radius * verticalSquish, 
            0
        );

        if (IsTooClose(spawnPos, tilePos)) return;

        PlaceTree(spawnPos, tilePos);
    }

    private bool IsTooClose(Vector3 pos, Vector3Int centerTile)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector2Int checkTile = new Vector2Int(centerTile.x + x, centerTile.y + y);
                if (spatialGrid.ContainsKey(checkTile))
                {
                    foreach (Vector3 existingPos in spatialGrid[checkTile])
                    {
                        if ((pos - existingPos).sqrMagnitude < minDistanceBetweenTrees * minDistanceBetweenTrees)
                            return true;
                    }
                }
            }
        }
        return false;
    }

    private void PlaceTree(Vector3 pos, Vector3Int tilePos)
    {
        if (treePrefabs.Count == 0) return;

        Vector3 sortedPos = new Vector3(pos.x, pos.y, pos.y);
        GameObject tree = Instantiate(treePrefabs[Random.Range(0, treePrefabs.Count)], sortedPos, Quaternion.identity, treeContainer);

        float randomScale = baseScale * Random.Range(scaleRange.x, scaleRange.y);
        tree.transform.localScale = new Vector3(randomScale, randomScale, 1f);
        if(Random.value > 0.5f) tree.transform.localScale = new Vector3(-tree.transform.localScale.x, tree.transform.localScale.y, 1f);

        Vector2Int gridKey = new Vector2Int(tilePos.x, tilePos.y);
        if (!spatialGrid.ContainsKey(gridKey)) spatialGrid[gridKey] = new List<Vector3>();
        spatialGrid[gridKey].Add(pos);
    }
}