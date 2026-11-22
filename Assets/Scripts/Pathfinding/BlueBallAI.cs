using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BlueBallAI : MonoBehaviour
{
    [Header("Hide And Seek")]
    [SerializeField] private float hidingTime = 10f;
    [SerializeField] private float seekSpeed = 3.5f;
    [SerializeField] private float minimumDistance = 3.2f;
    [SerializeField] private float maximumDistance = 4f;
    [SerializeField] private float replanInterval = 0.35f;

    private Rigidbody body;
    private PlayerMotionReader player;
    private Grid3D grid;
    private readonly List<Vector3> path = new List<Vector3>();
    private int pathIndex;
    private float nextReplanTime;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.freezeRotation = true;
    }

    private void Start()
    {
        player = FindFirstObjectByType<PlayerMotionReader>();
        grid = FindFirstObjectByType<Grid3D>();
    }

    private void FixedUpdate()
    {
        if (player == null || grid == null)
            return;

        // This gives the player ten seconds to hide behind the obstacle maze.
        if (Time.timeSinceLevelLoad < hidingTime)
        {
            Stop();
            return;
        }

        Vector3 toPlayer = player.transform.position - transform.position;
        toPlayer.y = 0f;
        float distance = toPlayer.magnitude;

        // Once caught up, the seeker holds a narrow distance band instead of colliding with the player.
        if (distance >= minimumDistance && distance <= maximumDistance)
        {
            Stop();
            return;
        }

        if (Time.time >= nextReplanTime)
        {
            Vector3 desiredTarget = player.transform.position;
            if (distance < minimumDistance && distance > 0.01f)
                desiredTarget = transform.position - toPlayer.normalized * maximumDistance;

            BuildPath(grid.WorldToCell(transform.position), grid.WorldToCell(desiredTarget));
            nextReplanTime = Time.time + replanInterval;
        }

        FollowPath();
    }

    private void BuildPath(Vector3Int startCell, Vector3Int targetCell)
    {
        path.Clear();
        pathIndex = 0;

        GridNode start = grid.GetNode(startCell);
        GridNode target = grid.GetNode(targetCell);
        if (start == null || target == null || !start.IsWalkable || !target.IsWalkable)
            return;

        List<GridNode> open = new List<GridNode> { start };
        HashSet<GridNode> closed = new HashSet<GridNode>();
        Dictionary<GridNode, GridNode> parents = new Dictionary<GridNode, GridNode>();
        Dictionary<GridNode, int> costs = new Dictionary<GridNode, int> { [start] = 0 };

        while (open.Count > 0)
        {
            GridNode current = GetBestNode(open, costs, target);
            if (current == target)
            {
                for (GridNode node = target; node != start; node = parents[node])
                    path.Add(node.WorldPosition);
                path.Add(start.WorldPosition);
                path.Reverse();
                return;
            }

            open.Remove(current);
            closed.Add(current);
            foreach (GridNode neighbour in GetNeighbours(current))
            {
                if (!neighbour.IsWalkable || closed.Contains(neighbour))
                    continue;

                int nextCost = costs[current] + 10;
                if (!costs.TryGetValue(neighbour, out int knownCost) || nextCost < knownCost)
                {
                    costs[neighbour] = nextCost;
                    parents[neighbour] = current;
                    if (!open.Contains(neighbour))
                        open.Add(neighbour);
                }
            }
        }
    }

    private IEnumerable<GridNode> GetNeighbours(GridNode node)
    {
        Vector3Int[] directions = { Vector3Int.left, Vector3Int.right, new Vector3Int(0, 0, 1), new Vector3Int(0, 0, -1) };
        foreach (Vector3Int direction in directions)
        {
            GridNode neighbour = grid.GetNode(node.GridPosition + direction);
            if (neighbour != null)
                yield return neighbour;
        }
    }

    private static GridNode GetBestNode(List<GridNode> nodes, Dictionary<GridNode, int> costs, GridNode target)
    {
        GridNode best = nodes[0];
        int bestScore = GetScore(best, costs, target);
        for (int i = 1; i < nodes.Count; i++)
        {
            int score = GetScore(nodes[i], costs, target);
            if (score < bestScore)
            {
                best = nodes[i];
                bestScore = score;
            }
        }
        return best;
    }

    private static int GetScore(GridNode node, Dictionary<GridNode, int> costs, GridNode target)
    {
        int heuristic = Mathf.Abs(node.GridPosition.x - target.GridPosition.x) + Mathf.Abs(node.GridPosition.z - target.GridPosition.z);
        return costs[node] + heuristic * 10;
    }

    private void FollowPath()
    {
        if (path.Count == 0)
        {
            Stop();
            return;
        }

        while (pathIndex < path.Count - 1 && FlatDistance(transform.position, path[pathIndex]) < 0.18f)
            pathIndex++;

        Vector3 direction = path[pathIndex] - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.001f)
        {
            Stop();
            return;
        }

        body.linearVelocity = new Vector3(direction.normalized.x * seekSpeed, body.linearVelocity.y, direction.normalized.z * seekSpeed);
    }

    private void Stop()
    {
        body.linearVelocity = new Vector3(0f, body.linearVelocity.y, 0f);
    }

    private static float FlatDistance(Vector3 a, Vector3 b)
    {
        a.y = 0f;
        b.y = 0f;
        return Vector3.Distance(a, b);
    }
}
