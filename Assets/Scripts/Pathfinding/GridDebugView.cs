using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>Shows the grid cell under the centre of the camera while testing.</summary>
[RequireComponent(typeof(Grid3D))]
public sealed class GridDebugView : MonoBehaviour
{
    [SerializeField] private bool visibleOnStart = true;
    [SerializeField] private Key toggleKey = Key.F3;

    private Grid3D grid;
    private bool isVisible;
    private bool hasSelectedCell;
    private Vector3Int selectedCell;

    private void Awake()
    {
        grid = GetComponent<Grid3D>();
        isVisible = visibleOnStart;
        grid.SetDevelopmentViewEnabled(isVisible);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current[toggleKey].wasPressedThisFrame)
        {
            isVisible = !isVisible;
            grid.SetDevelopmentViewEnabled(isVisible);
        }

        UpdateSelectedCell();
    }

    private void UpdateSelectedCell()
    {
        Camera camera = Camera.main;
        if (camera == null || !Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hit, 100f))
        {
            hasSelectedCell = false;
            return;
        }

        Vector3Int cell = grid.WorldToCell(hit.point);
        hasSelectedCell = grid.IsInsideGrid(cell);
        if (hasSelectedCell)
            selectedCell = cell;
    }

    private void OnGUI()
    {
        if (!isVisible || grid == null)
            return;

        GUI.Box(new Rect(20f, 320f, 310f, 176f), "Grid Debug  [F3 to hide]");
        GUI.Label(new Rect(38f, 354f, 276f, 22f), $"Size: {grid.GridSize}   Cells: {grid.CellCount}");
        GUI.Label(new Rect(38f, 378f, 276f, 22f), $"Blocked: {grid.BlockedCellCount}");

        if (!hasSelectedCell)
        {
            GUI.Label(new Rect(38f, 410f, 276f, 44f), "Aim the centre of the camera at a grid cell.");
            return;
        }

        GridNode node = grid.GetNode(selectedCell);
        string state = node.IsWalkable ? "WALKABLE" : "BLOCKED";
        GUI.Label(new Rect(38f, 410f, 276f, 22f), $"Cell: {selectedCell}  {state}");
        GUI.Label(new Rect(38f, 434f, 276f, 22f), $"World: {node.WorldPosition:F2}");
        GUI.Label(new Rect(38f, 458f, 276f, 22f), $"Face neighbours: {grid.GetNeighbours(selectedCell).Count}");
    }

    private void OnDrawGizmos()
    {
        if (!isVisible || !hasSelectedCell || grid == null)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(grid.GetCellWorldPosition(selectedCell), Vector3.one * grid.CellSize * 0.96f);
    }
}
