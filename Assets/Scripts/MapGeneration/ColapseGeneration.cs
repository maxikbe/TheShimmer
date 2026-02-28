using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

        public Cell(Vector3Int pos, List<TileData> options)
        {
            position = pos;
            possibilities = new List<TileData>(options);
        }
    }

    private Cell[,] grid;

    void Start() => GenerateMap();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            GenerateMap();
        }
    }
    public void GenerateMap()
    {
        StopAllCoroutines();
        targetTilemap.ClearAllTiles();
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
                grid[x, y] = new Cell(new Vector3Int(x, y, 0), tileOptions);
            }
        }
    }

    IEnumerator WFCAlgorithm()
    {
        while (!IsFullyCollapsed())
        {
            Cell nextCell = GetLowestEntropyCell();
            if (nextCell == null) break;

            CollapseCell(nextCell);
            Propagate(nextCell);
            yield return null;
        }
    }

    void CollapseCell(Cell cell)
    {
        if (cell.possibilities.Count == 0)
        {
            cell.isCollapsed = true;
            return;
        }

        float totalWeight = cell.possibilities.Sum(t => t.weight);
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

        cell.possibilities = new List<TileData> { cell.chosenTile };
        cell.isCollapsed = true;
        targetTilemap.SetTile(cell.position, cell.chosenTile.tile);
    }

    void Propagate(Cell collapsedCell)
    {
        Queue<Cell> queue = new Queue<Cell>();
        queue.Enqueue(collapsedCell);

        while (queue.Count > 0)
        {
            Cell current = queue.Dequeue();
            Vector3Int[] dirs = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

            foreach (Vector3Int dir in dirs)
            {
                int nx = current.position.x + dir.x;
                int ny = current.position.y + dir.y;

                if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                {
                    Cell neighbor = grid[nx, ny];
                    if (neighbor.isCollapsed) continue;

                    int countBefore = neighbor.possibilities.Count;

                    neighbor.possibilities = neighbor.possibilities.Where(nPossible => 
                        current.possibilities.Any(cPossible => IsCompatible(cPossible, nPossible, dir))
                    ).ToList();

                    if (neighbor.possibilities.Count < countBefore)
                    {
                        if (!queue.Contains(neighbor)) queue.Enqueue(neighbor);
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
        var remaining = grid.Cast<Cell>().Where(c => !c.isCollapsed).ToList();
        if (remaining.Count == 0) return null;

        int minOptions = remaining.Min(c => c.possibilities.Count);
        return remaining.Where(c => c.possibilities.Count == minOptions)
                        .OrderBy(_ => Random.value).FirstOrDefault();
    }

    bool IsFullyCollapsed() => grid.Cast<Cell>().All(c => c.isCollapsed);
}