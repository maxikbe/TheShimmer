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

    private class Cell
    {
        public Vector3Int position;
        public bool isCollapsed = false;
        public TileData chosenTile = null;
        public List<TileData> possibilities;
        public bool inQueue = false;

        public Cell(Vector3Int pos, List<TileData> options)
        {
            position = pos;
            possibilities = new List<TileData>(options);
        }
    }

    private Cell[,] grid;
    private readonly Vector3Int[] directions = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

    void Start() => GenerateMap();

    public void GenerateMap()
    {
        StopAllCoroutines();
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
                Vector3Int pos = new Vector3Int(x, y, 0);
                grid[x, y] = new Cell(pos, tileOptions);

                TileBase existing = targetTilemap.GetTile(pos);
                if (existing != null)
                {
                    TileData match = tileOptions.Find(t => t.tile == existing);
                    if (match != null)
                    {
                        grid[x, y].chosenTile = match;
                        grid[x, y].possibilities = new List<TileData> { match };
                        grid[x, y].isCollapsed = true;
                    }
                }
            }
        }

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (grid[x, y].isCollapsed) Propagate(grid[x, y]);
    }

    public int tilesPerFrame = 10; 

    IEnumerator WFCAlgorithm()
    {
        int tilesCollapsedThisFrame = 0;

        while (true)
        {
            Cell nextCell = GetLowestEntropyCell();
            if (nextCell == null) break;

            CollapseCell(nextCell);
            Propagate(nextCell);

            tilesCollapsedThisFrame++;
            if (tilesCollapsedThisFrame >= tilesPerFrame)
            {
                tilesCollapsedThisFrame = 0;
                yield return null;
            }
        }
        Debug.Log("Generation Complete!");
    }

    void CollapseCell(Cell cell)
    {
        if (cell.possibilities.Count == 0)
        {
            cell.isCollapsed = true;
            return;
        }

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
        cell.isCollapsed = true;
        targetTilemap.SetTile(cell.position, cell.chosenTile.tile);
    }

    void Propagate(Cell collapsedCell)
    {
        Queue<Cell> queue = new Queue<Cell>();
        queue.Enqueue(collapsedCell);
        collapsedCell.inQueue = true;

        while (queue.Count > 0)
        {
            Cell current = queue.Dequeue();
            current.inQueue = false;

            foreach (Vector3Int dir in directions)
            {
                int nx = current.position.x + dir.x;
                int ny = current.position.y + dir.y;

                if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                {
                    Cell neighbor = grid[nx, ny];
                    if (neighbor.isCollapsed) continue;

                    int startCount = neighbor.possibilities.Count;
                    for (int i = neighbor.possibilities.Count - 1; i >= 0; i--)
                    {
                        bool possible = false;
                        TileData nTile = neighbor.possibilities[i];

                        for (int j = 0; j < current.possibilities.Count; j++)
                        {
                            if (IsCompatible(current.possibilities[j], nTile, dir))
                            {
                                possible = true;
                                break;
                            }
                        }

                        if (!possible)
                        {
                            neighbor.possibilities.RemoveAt(i);
                        }
                    }

                    if (neighbor.possibilities.Count < startCount && !neighbor.inQueue)
                    {
                        queue.Enqueue(neighbor);
                        neighbor.inQueue = true;
                    }
                }
            }
        }
    }

    bool IsCompatible(TileData anchor, TileData check, Vector3Int dir)
    {
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
                else if (count == minEntropy && Random.value > 0.5f)
                {
                    bestCell = cell;
                }
            }
        }
        return bestCell;
    }
}