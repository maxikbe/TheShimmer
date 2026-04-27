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
    public int tilesPerFrame = 10;

    private class Cell
    {
        public Vector3Int position;
        public int x, y;
        public bool isCollapsed = false;
        public TileData chosenTile = null;
        public List<TileData> possibilities;

        public Cell(int x, int y, List<TileData> options)
        {
            this.x = x;
            this.y = y;
            position = new Vector3Int(x, y, 0);
            possibilities = new List<TileData>(options);
        }

        public Cell Clone()
        {
            return new Cell(x, y, new List<TileData>(possibilities))
            {
                isCollapsed = this.isCollapsed,
                chosenTile = this.chosenTile
            };
        }
    }

    private class WFCState
    {
        public Cell[,] gridSnapshot;
        public Vector3Int collapsedPos;
        public TileData attemptedTile;

        public WFCState(Cell[,] currentGrid, Vector3Int pos, TileData tile)
        {
            int w = currentGrid.GetLength(0);
            int h = currentGrid.GetLength(1);
            gridSnapshot = new Cell[w, h];
            for (int i = 0; i < w; i++)
                for (int j = 0; j < h; j++)
                    gridSnapshot[i, j] = currentGrid[i, j].Clone();

            collapsedPos = pos;
            attemptedTile = tile;
        }
    }

    private Cell[,] grid;
    private Stack<WFCState> history = new Stack<WFCState>();
    private readonly Vector3Int[] directions = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

    void Start() => GenerateMap();

    public void GenerateMap()
    {
        StopAllCoroutines();
        targetTilemap.ClearAllTiles();
        history.Clear();
        InitializeGrid();
        StartCoroutine(WFCAlgorithm());
    }

    void InitializeGrid()
    {
        grid = new Cell[width, height];
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                grid[x, y] = new Cell(x, y, tileOptions);
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
                if (history.Count == 0)
                {
                    Debug.LogError("No valid configuration possible.");
                    yield break;
                }
                Backtrack();
                continue;
            }

            TileData choice = PickWeightedTile(nextCell);
            history.Push(new WFCState(grid, nextCell.position, choice));

            ApplyCollapse(nextCell, choice);

            if (!Propagate(nextCell))
            {
                Backtrack();
                continue;
            }

            processedThisFrame++;
            if (processedThisFrame >= tilesPerFrame)
            {
                processedThisFrame = 0;
                yield return null;
            }
        }
    }

    void Backtrack()
    {
        WFCState lastState = history.Pop();
        grid = lastState.gridSnapshot;
        
        Cell cellInGrid = grid[lastState.collapsedPos.x, lastState.collapsedPos.y];
        cellInGrid.possibilities.RemoveAll(t => t.tileID == lastState.attemptedTile.tileID);
        
        UpdateTilemapVisuals();
    }

    void UpdateTilemapVisuals()
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                targetTilemap.SetTile(grid[x, y].position, grid[x, y].isCollapsed ? grid[x, y].chosenTile.tile : null);
    }

    TileData PickWeightedTile(Cell cell)
    {
        float totalWeight = cell.possibilities.Sum(t => t.weight);
        float r = Random.Range(0, totalWeight);
        float currentWeight = 0;

        foreach (var tile in cell.possibilities)
        {
            currentWeight += tile.weight;
            if (r <= currentWeight) return tile;
        }
        return cell.possibilities[0];
    }

    void ApplyCollapse(Cell cell, TileData tile)
    {
        cell.chosenTile = tile;
        cell.possibilities = new List<TileData> { tile };
        cell.isCollapsed = true;
        targetTilemap.SetTile(cell.position, tile.tile);
    }

    bool Propagate(Cell collapsedCell)
    {
        Queue<Cell> queue = new Queue<Cell>();
        queue.Enqueue(collapsedCell);

        while (queue.Count > 0)
        {
            Cell current = queue.Dequeue();

            foreach (Vector3Int dir in directions)
            {
                int nx = current.x + dir.x;
                int ny = current.y + dir.y;

                if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                {
                    Cell neighbor = grid[nx, ny];
                    if (neighbor.isCollapsed) continue;

                    int startCount = neighbor.possibilities.Count;
                    neighbor.possibilities.RemoveAll(nTile => !IsStillValid(nTile, current.possibilities, dir));

                    if (neighbor.possibilities.Count == 0) return false;

                    if (neighbor.possibilities.Count < startCount)
                    {
                        if (!queue.Contains(neighbor)) queue.Enqueue(neighbor);
                    }
                }
            }
        }
        return true;
    }

    bool IsStillValid(TileData neighborTile, List<TileData> currentPossibilities, Vector3Int dir)
    {
        return currentPossibilities.Any(curr => IsCompatible(curr, neighborTile, dir));
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
                if (count > 0 && count < minEntropy)
                {
                    minEntropy = count;
                    bestCell = cell;
                }
            }
        }
        return bestCell;
    }
}