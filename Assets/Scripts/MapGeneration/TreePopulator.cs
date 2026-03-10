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

    [Header("Spawn Settings")]
    public float spawnRadius = 3.5f; 
    public int treesPerTile = 8; 
    [Range(0, 1)] public float spawnChance = 0.7f;

    void Start()
    {
        GenerateForests();
    }

    public void GenerateForests()
    {
        BoundsInt bounds = groundTilemap.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = groundTilemap.GetTile(pos);

            if (tile == targetTile)
            {
                Vector3 worldPos = groundTilemap.GetCellCenterWorld(pos);
                //Debug.Log(worldPos);
                GrowForestAtPoint(worldPos);
            }
        }
    }

    private void GrowForestAtPoint(Vector3 centerPos)
{
    for (int i = 0; i < treesPerTile; i++)
    {
        if (Random.value > spawnChance) continue;

        float angle = Random.value * Mathf.PI * 2;
        float radius = Mathf.Sqrt(Random.value) * spawnRadius; 
        
        Vector3 scatter = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
        Vector3 spawnPos = centerPos + scatter;

        // 2. Validation: Only spawn if there's a tile (uncomment if needed)
        /*
        if (groundTilemap.HasTile(groundTilemap.WorldToCell(spawnPos)))
        {
            PlaceTree(spawnPos);
        }
        */
        
        PlaceTree(spawnPos);
    }
}

private void PlaceTree(Vector3 pos)
{
    if (treePrefabs.Count == 0) return;

    GameObject prefab = treePrefabs[Random.Range(0, treePrefabs.Count)];
    
    GameObject tree = Instantiate(prefab, pos, Quaternion.identity, treeContainer);

    float randomScale = Random.Range(8*0.75f, 8*1.25f);
    tree.transform.localScale = new Vector3(randomScale, randomScale, 1f);
}
}