using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ColapseGeneration : MonoBehaviour
{
    public Tilemap targetTilemap;
    public int width = 10;
    public int height = 10;
    public List<TileData> tileOptions; 

    private class Cell {
        public Vector3Int position;
        public bool isCollapsed = false;
        public TileData chosenTile = null;
        public List<TileData> possibilities;

        public Cell(Vector3Int pos, List<TileData> options) {
            position = pos;
            possibilities = new List<TileData>(options);
        }
    }

    private Cell[,] grid;
    private Coroutine activeRoutine;

    void Start() => generateMap();

    public void generateMap() {
        if (activeRoutine != null) StopCoroutine(activeRoutine);
        targetTilemap.ClearAllTiles();
        InitializeGrid();
        activeRoutine = StartCoroutine(CollapseRoutine());
    }

    void InitializeGrid() {
        grid = new Cell[width, height];
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                grid[x, y] = new Cell(new Vector3Int(x, y, 0), tileOptions);
            }
        }
    }

    IEnumerator CollapseRoutine() {
        while (!IsFullyCollapsed()) {
            Cell nextCell = GetLowestEntropyCell();
            if (nextCell == null) break;

            if (nextCell.possibilities.Count == 0) {
                nextCell.chosenTile = tileOptions[0]; 
            } else {
                nextCell.chosenTile = PickTileWithWeight(nextCell.possibilities);
            }

            nextCell.isCollapsed = true;
            nextCell.possibilities = new List<TileData> { nextCell.chosenTile };
            targetTilemap.SetTile(nextCell.position, nextCell.chosenTile.tile);

            Propagate(nextCell);
            yield return null; 
        }
    }

    void Propagate(Cell collapsedCell) {
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        stack.Push(new Vector2Int(collapsedCell.position.x, collapsedCell.position.y));

        while (stack.Count > 0) {
            Vector2Int currentPos = stack.Pop();
            Cell currentCell = grid[currentPos.x, currentPos.y];
            Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

            foreach (Vector2Int dir in directions) {
                Vector2Int neighborPos = currentPos + dir;
                if (neighborPos.x >= 0 && neighborPos.x < width && neighborPos.y >= 0 && neighborPos.y < height) {
                    Cell neighbor = grid[neighborPos.x, neighborPos.y];
                    if (neighbor.isCollapsed) continue;

                    int optionsBefore = neighbor.possibilities.Count;
                    neighbor.possibilities = neighbor.possibilities.Where(nTile => {
                        return currentCell.possibilities.Any(cTile => CheckSockets(cTile, nTile, dir));
                    }).ToList();

                    if (neighbor.possibilities.Count != optionsBefore) {
                        stack.Push(neighborPos);
                    }
                }
            }
        }
    }

    bool CheckSockets(TileData current, TileData neighbor, Vector2Int direction) {
        if (direction == Vector2Int.up) return current.up == neighbor.down;
        if (direction == Vector2Int.down) return current.down == neighbor.up;
        if (direction == Vector2Int.left) return current.left == neighbor.right;
        if (direction == Vector2Int.right) return current.right == neighbor.left;
        return false;
    }

    Cell GetLowestEntropyCell() {
        List<Cell> candidates = new List<Cell>();
        int minOptions = int.MaxValue;
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (!grid[x, y].isCollapsed) {
                    int count = grid[x, y].possibilities.Count;
                    if (count < minOptions) {
                        minOptions = count;
                        candidates.Clear();
                        candidates.Add(grid[x, y]);
                    } else if (count == minOptions) {
                        candidates.Add(grid[x, y]);
                    }
                }
            }
        }
        return candidates.Count > 0 ? candidates[Random.Range(0, candidates.Count)] : null;
    }

    TileData PickTileWithWeight(List<TileData> options) {
        float totalWeight = options.Sum(t => (float)t.weight);
        float rand = Random.Range(0, totalWeight);
        float currentSum = 0;
        foreach (var tile in options) {
            currentSum += tile.weight;
            if (rand <= currentSum) return tile;
        }
        return options[0];
    }

    bool IsFullyCollapsed() {
        foreach (var cell in grid) if (!cell.isCollapsed) return false;
        return true;
    }
}