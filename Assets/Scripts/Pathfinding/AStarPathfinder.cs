using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Grid3D))]
public class AStarPathfinder : MonoBehaviour
{
    [SerializeField] private Vector3Int startCell = new Vector3Int(0, 0, 0);
    [SerializeField] private Vector3Int targetCell = new Vector3Int(7, 0, 7);
    [SerializeField] private bool allowDiagonalMovement;

    private Grid3D grid;
    private readonly List<GridNode> path = new List<GridNode>();

    private void Start()
    {
        grid = GetComponent<Grid3D>();
        FindPath();
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            FindPath();

        // Placeholder for the matrix-puzzle event: mutate a node, then replan immediately.
        if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
        {
            Vector3Int puzzleControlledCell = new Vector3Int(2, 0, 3);
            GridNode node = grid.GetNode(puzzleControlledCell);
            grid.SetWalkable(puzzleControlledCell, !node.IsWalkable);
            FindPath();
        }
    }

    public void FindPath()
    {
        if (grid == null)
            grid = GetComponent<Grid3D>();

        path.Clear();
        GridNode startNode = grid.GetNode(startCell);
        GridNode targetNode = grid.GetNode(targetCell);

        if (startNode == null || targetNode == null || !startNode.IsWalkable || !targetNode.IsWalkable)
        {
            Debug.LogWarning("A* needs walkable start and target cells.");
            return;
        }

        ResetNodes();
        List<GridNode> openSet = new List<GridNode> { startNode };
        HashSet<GridNode> closedSet = new HashSet<GridNode>();
        startNode.GCost = 0;
        startNode.HCost = GetDistance(startNode, targetNode);

        while (openSet.Count > 0)
        {
            GridNode current = GetLowestCostNode(openSet);
            if (current == targetNode)
            {
                BuildPath(startNode, targetNode);
                Debug.Log($"A* path found: {path.Count} nodes. Press P to change the puzzle-controlled node, or R to recalculate.");
                return;
            }

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (GridNode neighbour in GetNeighbours(current))
            {
                if (!neighbour.IsWalkable || closedSet.Contains(neighbour))
                    continue;

                int tentativeCost = current.GCost + GetDistance(current, neighbour);
                if (tentativeCost >= neighbour.GCost)
                    continue;

                neighbour.Parent = current;
                neighbour.GCost = tentativeCost;
                neighbour.HCost = GetDistance(neighbour, targetNode);
                if (!openSet.Contains(neighbour))
                    openSet.Add(neighbour);
            }
        }

        Debug.LogWarning("A* could not find a route. Keep at least one gap between obstacles.");
    }

    private void ResetNodes()
    {
        Vector3Int size = grid.GridSize;
        for (int x = 0; x < size.x; x++)
            for (int z = 0; z < size.z; z++)
                grid.GetNode(new Vector3Int(x, 0, z))?.ResetCosts();
    }

    private IEnumerable<GridNode> GetNeighbours(GridNode node)
    {
        int[,] directions = allowDiagonalMovement
            ? new int[,] { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 }, { -1, -1 }, { -1, 1 }, { 1, -1 }, { 1, 1 } }
            : new int[,] { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } };

        for (int i = 0; i < directions.GetLength(0); i++)
        {
            Vector3Int position = node.GridPosition + new Vector3Int(directions[i, 0], 0, directions[i, 1]);
            GridNode neighbour = grid.GetNode(position);
            if (neighbour != null)
                yield return neighbour;
        }
    }

    private static GridNode GetLowestCostNode(List<GridNode> nodes)
    {
        GridNode best = nodes[0];
        for (int i = 1; i < nodes.Count; i++)
            if (nodes[i].FCost < best.FCost || (nodes[i].FCost == best.FCost && nodes[i].HCost < best.HCost))
                best = nodes[i];
        return best;
    }

    private int GetDistance(GridNode a, GridNode b)
    {
        int dx = Mathf.Abs(a.GridPosition.x - b.GridPosition.x);
        int dz = Mathf.Abs(a.GridPosition.z - b.GridPosition.z);
        return allowDiagonalMovement ? 14 * Mathf.Min(dx, dz) + 10 * Mathf.Abs(dx - dz) : 10 * (dx + dz);
    }

    private void BuildPath(GridNode start, GridNode end)
    {
        for (GridNode current = end; current != start; current = current.Parent)
            path.Add(current);
        path.Add(start);
        path.Reverse();
    }

    private void OnDrawGizmos()
    {
        Grid3D currentGrid = grid != null ? grid : GetComponent<Grid3D>();
        if (currentGrid == null)
            return;

        DrawCellMarker(currentGrid, startCell, Color.yellow, "PATH START");
        DrawCellMarker(currentGrid, targetCell, Color.magenta, "TARGET");

        if (path.Count < 2)
            return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < path.Count - 1; i++)
            Gizmos.DrawLine(path[i].WorldPosition + Vector3.up * 0.15f, path[i + 1].WorldPosition + Vector3.up * 0.15f);
    }

    private static void DrawCellMarker(Grid3D currentGrid, Vector3Int cell, Color color, string label)
    {
        GridNode node = currentGrid.GetNode(cell);
        Vector3 worldPosition = node != null
            ? node.WorldPosition
            : currentGrid.transform.position + new Vector3(cell.x + 0.5f, 0.15f, cell.z + 0.5f);
        Gizmos.color = color;
        Gizmos.DrawSphere(worldPosition + Vector3.up * 0.18f, 0.18f);
#if UNITY_EDITOR
        UnityEditor.Handles.Label(worldPosition + Vector3.up * 0.4f, label);
#endif
    }
}
