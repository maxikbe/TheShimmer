using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class ColapseGeneration : MonoBehaviour
{
    public Tilemap targetTilemap;
    public int width = 20;
    public int height = 20;
    public List<TileData> tileOptions;
    public int tilesPerFrame = 10;

    private class Cell
    {
        public Vector3Int position;
        public int x, y;
        public bool isCollapsed = false;
        public TileData chosenTile = null;
        public List<TileData> possibilities;
        public bool inQueue = false;

        public Cell(int x, int y, List<TileData> options)
        {
            this.x = x;
            this.y = y;
            position = new Vector3Int(x, y, 0);
            possibilities = new List<TileData>(options);
        }
    }

    private Cell[,] grid;
    private readonly Vector3Int[] directions = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };
    private readonly Vector3Int[] directions = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

    void Start() => GenerateMap();

    public void GenerateMap()
    {
        StopAllCoroutines();
        InitializeGrid();
        InitializeGrid();
        StartCoroutine(WFCAlgorithm());
    }

    void InitializeGrid()
    {
        grid = new Cell[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = new Cell(x, y, tileOptions);
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                TileBase existing = targetTilemap.GetTile(grid[x, y].position);
                if (existing != null)
                {
                    TileData match = tileOptions.Find(t => t.tile == existing);
                    if (match != null)
                    {
                        grid[x, y].chosenTile = match;
                        grid[x, y].possibilities.Clear();
                        grid[x, y].possibilities.Add(match);
                        grid[x, y].isCollapsed = true;
                        Propagate(grid[x, y]);
                    }
                }
            }
        }
    }

    IEnumerator WFCAlgorithm()
    {
        int processedThisFrame = 0;

        while (true)
        {
            Cell nextCell = GetLowestEntropyCell();
            if (nextCell == null) break;

            if (nextCell.possibilities.Count == 0)
            {
                Debug.LogError($"Contradiction at {nextCell.position}! Restarting...");
                GenerateMap();
                yield break;
            }

            CollapseCell(nextCell);
            Propagate(nextCell);

            processedThisFrame++;
            if (processedThisFrame >= tilesPerFrame)
            {
                processedThisFrame = 0;
                yield return null; 
            }
        }
        Debug.Log("Generation Complete!");
    }

    void CollapseCell(Cell cell)
    {
        float totalWeight = 0;
        for (int i = 0; i < cell.possibilities.Count; i++)
            totalWeight += cell.possibilities[i].weight;

        float r = Random.Range(0, totalWeight);
        float currentWeight = 0;

        foreach (var tile in cell.possibilities)
        {
            currentWeight += tile.weight;
            if (r <= currentWeight)
            {
                cell.chosenTile = tile;
                break;
            }
        }

        if (cell.chosenTile == null) cell.chosenTile = cell.possibilities[0];

        cell.possibilities.Clear();
        cell.possibilities.Add(cell.chosenTile);
        cell.possibilities.Clear();
        cell.possibilities.Add(cell.chosenTile);
        cell.isCollapsed = true;
        targetTilemap.SetTile(cell.position, cell.chosenTile.tile);
    }

    void Propagate(Cell collapsedCell)
    {
        Queue<Cell> queue = new Queue<Cell>();
        queue.Enqueue(collapsedCell);
        collapsedCell.inQueue = true;
        collapsedCell.inQueue = true;

        while (queue.Count > 0)
        {
            Cell current = queue.Dequeue();
            current.inQueue = false;
            current.inQueue = false;

            foreach (Vector3Int dir in directions)
            foreach (Vector3Int dir in directions)
            {
                int nx = current.x + dir.x;
                int ny = current.y + dir.y;

                if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                {
                    Cell neighbor = grid[nx, ny];
                    if (neighbor.isCollapsed) continue;

                    int startCount = neighbor.possibilities.Count;
                    
                    for (int i = neighbor.possibilities.Count - 1; i >= 0; i--)
                    {
                        TileData nTile = neighbor.possibilities[i];
                        if (!IsStillValid(nTile, current.possibilities, dir))
                        {
                            neighbor.possibilities.RemoveAt(i);
                        }
                    }

                    if (neighbor.possibilities.Count < startCount)
                    {
                        if (neighbor.possibilities.Count == 0) return;
                        if (!neighbor.inQueue)
                        {
                            queue.Enqueue(neighbor);
                            neighbor.inQueue = true;
                        }
                    }
                }
            }
        }
    }

    bool IsStillValid(TileData neighborTile, List<TileData> currentPossibilities, Vector3Int dir)
    {
        for (int i = 0; i < currentPossibilities.Count; i++)
        {
            if (IsCompatible(currentPossibilities[i], neighborTile, dir)) return true;
        }
        return false;
    }

    bool IsCompatible(TileData anchor, TileData check, Vector3Int dir)
    {
        // Direct List.Contains is fine for small lists, but HashSet in TileData would be even faster
        if (dir == Vector3Int.up) return anchor.validUp.Contains(check.tileID);
        if (dir == Vector3Int.down) return anchor.validDown.Contains(check.tileID);
        if (dir == Vector3Int.left) return anchor.validLeft.Contains(check.tileID);
        if (dir == Vector3Int.right) return anchor.validRight.Contains(check.tileID);
        return false;
    }

    Cell GetLowestEntropyCell()
    {
        Cell bestCell = null;
        int minEntropy = int.MaxValue;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = grid[x, y];
                if (cell.isCollapsed) continue;

                int count = cell.possibilities.Count;
                if (count < minEntropy)
                {
                    minEntropy = count;
                    bestCell = cell;
                }
                else if (count == minEntropy && Random.value > 0.8f)
                {
                    bestCell = cell;
                }
            }
        }
        return bestCell;
    }
}