
using UnityEngine;
using System.Collections.Generic;

public class Grid3D : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int width = 8;
    [SerializeField] private int height = 1;
    [SerializeField] private int depth = 8;
    [SerializeField] private float cellSize = 1f;

    [Header("Blocked Cells")]
    [SerializeField] private Vector3Int[] blockedCells;

    [Header("Visual Settings")]
    [SerializeField] private float cubeHeight = 0.1f;

    [SerializeField] private bool showGridGizmos = true;
    [SerializeField] private bool showCoordinates = true;

    private GridNode[,,] grid;

    private void Awake()
    {
        CreateGrid();
    }

    private void Start()
    {
        CreateGridVisuals();
    }

    public Vector3Int GridSize => new Vector3Int(width, height, depth);
    public float CellSize => cellSize;

    /// <summary>Updates a node at runtime and refreshes its simple demo visual.</summary>
    public void SetWalkable(Vector3Int position, bool walkable)
    {
        GridNode node = GetNode(position);
        if (node == null)
            return;

        node.IsWalkable = walkable;
        List<Vector3Int> cells = blockedCells == null
            ? new List<Vector3Int>()
            : new List<Vector3Int>(blockedCells);

        if (walkable)
            cells.Remove(position);
        else if (!cells.Contains(position))
            cells.Add(position);

        blockedCells = cells.ToArray();
        CreateGridVisuals();
    }

    public GridNode GetNode(Vector3Int position)
    {
        if (grid == null || !IsInsideGrid(position))
            return null;

        return grid[position.x, position.y, position.z];
    }

    public bool IsCellBlocked(Vector3Int position)
    {
        return IsBlocked(position);
    }

    public Vector3 GetCellWorldPosition(Vector3Int position)
    {
        return transform.position + new Vector3(
            (position.x + 0.5f) * cellSize,
            (position.y + 0.5f) * cellSize,
            (position.z + 0.5f) * cellSize);
    }

    public Vector3Int WorldToCell(Vector3 worldPosition)
    {
        Vector3 local = worldPosition - transform.position;
        return new Vector3Int(
            Mathf.Clamp(Mathf.FloorToInt(local.x / cellSize), 0, width - 1),
            Mathf.Clamp(Mathf.FloorToInt(local.y / cellSize), 0, height - 1),
            Mathf.Clamp(Mathf.FloorToInt(local.z / cellSize), 0, depth - 1));
    }

    public bool IsInsideGrid(Vector3Int position)
    {
        return position.x >= 0 && position.x < width &&
               position.y >= 0 && position.y < height &&
               position.z >= 0 && position.z < depth;
    }

    public void CreateGrid()
    {
        grid = new GridNode[width, height, depth];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    Vector3Int pos = new Vector3Int(x, y, z);

                    bool walkable = !IsBlocked(pos);

                    Vector3 worldPos = transform.position +
                        new Vector3(
                            (x + 0.5f) * cellSize,
                            (y + 0.5f) * cellSize,
                            (z + 0.5f) * cellSize
                        );

                    grid[x, y, z] = new GridNode(
                        pos, worldPos, walkable
                    );
                }
            }
        }

        Debug.Log("Grid created successfully!");
    }

    private bool IsBlocked(Vector3Int position)
    {
        if (blockedCells == null)
            return false;

        foreach (Vector3Int blocked in blockedCells)
        {
            if (blocked == position)
                return true;
        }

        return false;
    }

    private void CreateGridVisuals()
    {
        // Remove previous generated visuals
        Transform oldVisuals = transform.Find("GridVisuals");

        if (oldVisuals != null)
            Destroy(oldVisuals.gameObject);

        GameObject parent = new GameObject("GridVisuals");
        parent.transform.SetParent(transform, false);
        parent.transform.localPosition = Vector3.zero;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                Vector3 position = transform.position +
                    new Vector3(
                        (x + 0.5f) * cellSize,
                        cubeHeight * 0.5f,
                        (z + 0.5f) * cellSize
                    );

                GameObject cube = GameObject.CreatePrimitive(
                    PrimitiveType.Cube
                );

                cube.name = $"Cell_{x}_{z}";

                cube.transform.position = position;

                bool isBlocked = IsBlocked(new Vector3Int(x, 0, z));

                cube.transform.localScale = new Vector3(
                    cellSize * 0.95f,
                    isBlocked ? Mathf.Max(cubeHeight, 0.8f) : cubeHeight,
                    cellSize * 0.95f
                );

                cube.transform.SetParent(parent.transform, true);

                Renderer renderer = cube.GetComponent<Renderer>();

                renderer.material.color = isBlocked ? Color.red : Color.green;
            }
        }

        Debug.Log("Grid visuals created!");
    }

    private void OnDrawGizmos()
    {
        if (!showGridGizmos)
            return;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                Vector3Int cell = new Vector3Int(x, 0, z);
                Vector3 center = transform.position + new Vector3(
                    (x + 0.5f) * cellSize,
                    0.05f,
                    (z + 0.5f) * cellSize);

                Gizmos.color = IsBlocked(cell)
                    ? new Color(1f, 0.2f, 0.2f, 0.45f)
                    : new Color(0.2f, 1f, 0.35f, 0.20f);
                Gizmos.DrawCube(center, new Vector3(cellSize * 0.9f, 0.05f, cellSize * 0.9f));

                Gizmos.color = IsBlocked(cell) ? Color.red : Color.green;
                Gizmos.DrawWireCube(center, new Vector3(cellSize, 0.06f, cellSize));

#if UNITY_EDITOR
                if (showCoordinates)
                    UnityEditor.Handles.Label(center + Vector3.up * 0.08f, $"{x}, {z}");
#endif
            }
        }
    }
}
