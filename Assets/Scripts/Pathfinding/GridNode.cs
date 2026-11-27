
using UnityEngine;

public class GridNode
{
    public Vector3Int GridPosition { get; private set; }

    public Vector3 WorldPosition { get; private set; }

    public bool IsWalkable { get; set; }

    public int GCost { get; set; }

    public int HCost { get; set; }

    public int FCost => GCost + HCost;

    public GridNode Parent { get; set; }

    public GridNode(
        Vector3Int gridPosition,
        Vector3 worldPosition,
        bool isWalkable)
    {
        GridPosition = gridPosition;
        WorldPosition = worldPosition;
        IsWalkable = isWalkable;

        ResetCosts();
    }

    public void ResetCosts()
    {
        GCost = int.MaxValue;
        HCost = 0;
        Parent = null;
    }
}
