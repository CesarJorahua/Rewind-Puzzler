using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

/// <summary>
/// Deterministic maze generator. Seed format: "SIZE-CODE" e.g. "21-4F7A2B".
/// The SAME seed string always produces the SAME maze on any machine,
/// because we use an instanced System.Random + a stable FNV-1a string hash.
/// </summary>
public class SeededMazeGenerator : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_InputField inputField;

    [Header("Rendering (Tilemap)")]
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private TileBase wallTile;
    [SerializeField] private TileBase floorTile;

    private string seedInput = "";
    // true = wall, false = floor
    private bool[,] grid;
    private int size = 19;

    /// <summary>Query helper for gameplay (pathfinding, spawn checks, etc.).</summary>
    public bool IsWall(int x, int y) => grid[x, y];
    public int Size => size;

    public Vector2Int StartPosition => new(1, 0);
    public Vector2Int EndPosition => new(size - 2, size - 1);

    public Vector3 StartWorldPosition => GetWorldPosition(StartPosition);
    public Vector3 EndWorldPosition => GetWorldPosition(EndPosition);

    private void Start()
    {
        CreateRandomSeed();
        Generate();
        inputField.navigation = new Navigation
        {
            mode = Navigation.Mode.None
        };

        EventSystem.current.SetSelectedGameObject(null);
    }

    /// <summary>Public entry point — hook this to a UI InputField + Button.</summary>
    private void Generate(string seed)
    {
        seedInput = seed;

        inputField.DeactivateInputField();
        EventSystem.current.SetSelectedGameObject(null);

        if (!TryParseSeed(seed, out int rngSeed))
        {
            Debug.LogError($"Invalid seed '{seed}'. Expected format: CODE (e.g. 4F7A2B)");
            return;
        }

        var rng = new System.Random(rngSeed);
        grid = CarveMaze(size, rng);
        Render();
    }

    public void Generate()
    {
        seedInput = inputField.text;
        Generate(seedInput);
    }

    /// <summary>Creates a valid random seed string.</summary>
    public void CreateRandomSeed()
    {
        // Unity's Random is fine HERE because this only picks a new seed,
        // it doesn't participate in maze generation itself.
        String seed = UnityEngine.Random.Range(0, int.MaxValue).ToString("X"); // hex looks seed-y
        inputField.text = seed;
    }

    // ---------------- Worl position ----------------

    public Vector3 GetWorldPosition(Vector2Int mazePosition)
    {
        int offset = size / 2;

        Vector3Int tilePosition = new(
            mazePosition.x - offset,
            mazePosition.y - offset,
            0
        );

        return wallTilemap.GetCellCenterWorld(tilePosition);
    }

    // ---------------- Seed parsing ----------------

    private bool TryParseSeed(string seed, out int rngSeed)
    {
        rngSeed = 0;
        if (string.IsNullOrWhiteSpace(seed)) return false;
        rngSeed = Fnv1aHash(seed);
        return true;
    }

    /// <summary>
    /// FNV-1a: deterministic across platforms/runtimes, unlike string.GetHashCode().
    /// </summary>
    private static int Fnv1aHash(string text)
    {
        unchecked
        {
            const uint fnvPrime = 16777619;
            uint hash = 2166136261;
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            for (int i = 0; i < bytes.Length; i++)
            {
                hash ^= bytes[i];
                hash *= fnvPrime;
            }
            return (int)hash;
        }
    }

    // ---------------- Maze generation (iterative recursive backtracker) ----------------

    private static bool[,] CarveMaze(int size, System.Random rng)
    {
        bool[,] maze = new bool[size, size];

        // Start fully walled
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
                maze[x, y] = true;

        // Cells live on odd coordinates; walls between them on even ones.
        var stack = new Stack<Vector2Int>();
        var start = new Vector2Int(1, 1);
        maze[start.x, start.y] = false;
        stack.Push(start);

        // N/S/E/W jumps of 2 (skipping over the wall cell)
        Vector2Int[] directions =
        {
            new Vector2Int(0, 2), new Vector2Int(0, -2),
            new Vector2Int(2, 0), new Vector2Int(-2, 0)
        };

        var neighbors = new List<Vector2Int>(4);

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Peek();

            neighbors.Clear();
            foreach (var dir in directions)
            {
                Vector2Int next = current + dir;
                bool inBounds = next.x > 0 && next.x < size - 1 && next.y > 0 && next.y < size - 1;
                if (inBounds && maze[next.x, next.y]) // still a wall = unvisited
                    neighbors.Add(next);
            }

            if (neighbors.Count == 0)
            {
                stack.Pop(); // dead end -> backtrack
                continue;
            }

            // rng.Next is the ONLY randomness source -> full determinism
            Vector2Int chosen = neighbors[rng.Next(neighbors.Count)];
            Vector2Int wallBetween = current + (chosen - current) / 2;

            maze[wallBetween.x, wallBetween.y] = false;
            maze[chosen.x, chosen.y] = false;
            stack.Push(chosen);
        }

        // Entrance (bottom-left) and exit (top-right)
        maze[1, 0] = false;
        maze[size - 2, size - 1] = false;

        return maze;
    }

    // ---------------- Rendering ----------------

    private void Render()
    {
        if (wallTilemap == null)
        {
            Debug.LogWarning("No Tilemap assigned; maze generated in memory only.");
            return;
        }

        wallTilemap.ClearAllTiles();
        int offset = size / 2;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector3Int pos = new(
                    x - offset,
                    y - offset,
                    0
                );
                if (grid[x, y])
                {
                    wallTilemap.SetTile(pos, wallTile);
                }
                else
                {
                    floorTilemap.SetTile(pos, floorTile);
                }
            }
        }
    }
}