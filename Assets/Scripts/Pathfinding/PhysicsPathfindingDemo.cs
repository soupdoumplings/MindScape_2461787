using UnityEngine;

[RequireComponent(typeof(Grid3D))]
public class PhysicsPathfindingDemo : MonoBehaviour
{
    [SerializeField] private GameObject firstPersonPlayerPrefab;

    private const string LevelObjectsName = "Level Objects";
    private const string ObstaclesName = "Physical Obstacles";

    private Grid3D grid;

    private void Start()
    {
        grid = GetComponent<Grid3D>();
        grid.WalkabilityChanged += RefreshObstacles;
        BuildLevelObjects();
    }

    private void OnDestroy()
    {
        if (grid != null)
            grid.WalkabilityChanged -= RefreshObstacles;
    }

    [ContextMenu("Rebuild Level Objects")]
    public void RebuildLevelObjects()
    {
        Transform oldRoot = transform.Find(LevelObjectsName);
        if (oldRoot != null)
        {
            if (Application.isPlaying)
                Destroy(oldRoot.gameObject);
            else
                DestroyImmediate(oldRoot.gameObject);
        }

        BuildLevelObjects();
    }

    private void BuildLevelObjects()
    {
        if (transform.Find(LevelObjectsName) != null)
            return;

        if (grid == null || firstPersonPlayerPrefab == null)
            return;

        Transform root = new GameObject(LevelObjectsName).transform;
        root.SetParent(transform);
        root.localPosition = Vector3.zero;
        root.localRotation = Quaternion.identity;

        CreateFloor(grid, root);
        CreateObstacleCubes(grid, root);
        CreatePlayer(grid, root);
        CreateBlueBall(grid, root);

#if UNITY_EDITOR
        if (!Application.isPlaying)
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
#endif
    }

    private void CreateFloor(Grid3D grid, Transform root)
    {
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Physical Floor";
        floor.transform.SetParent(root);
        Vector3Int size = grid.GridSize;
        float cellSize = grid.CellSize;
        floor.transform.position = transform.position + new Vector3(size.x * cellSize * 0.5f, -0.15f, size.z * cellSize * 0.5f);
        floor.transform.localScale = new Vector3(size.x * cellSize, 0.3f, size.z * cellSize);
        SetColor(floor.GetComponent<Renderer>(), new Color(0.16f, 0.18f, 0.22f));
    }

    private void CreateObstacleCubes(Grid3D grid, Transform root)
    {
        Transform parent = new GameObject(ObstaclesName).transform;
        parent.SetParent(root);
        Vector3Int size = grid.GridSize;

        for (int x = 0; x < size.x; x++)
        for (int z = 0; z < size.z; z++)
        {
            Vector3Int cell = new Vector3Int(x, 0, z);
            if (!grid.IsCellBlocked(cell))
                continue;

            GameObject obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obstacle.name = $"Obstacle_{x}_{z}";
            obstacle.transform.SetParent(parent);
            obstacle.transform.position = grid.GetCellWorldPosition(cell) + Vector3.up * 0.15f;
            obstacle.transform.localScale = new Vector3(0.88f, 1.3f, 0.88f);
            SetColor(obstacle.GetComponent<Renderer>(), new Color(0.8f, 0.16f, 0.13f));
        }
    }

    private void RefreshObstacles(Vector3Int changedCell, bool isWalkable)
    {
        Transform levelRoot = transform.Find(LevelObjectsName);
        if (levelRoot == null)
            return;

        Transform oldObstacles = levelRoot.Find(ObstaclesName);
        if (oldObstacles != null)
            Destroy(oldObstacles.gameObject);

        CreateObstacleCubes(grid, levelRoot);
    }

    private void CreatePlayer(Grid3D grid, Transform root)
    {
        if (firstPersonPlayerPrefab == null)
        {
            Debug.LogError("Assign Starter Assets/Runtime/FirstPersonController/Prefabs/PlayerCapsule to PhysicsPathfindingDemo.");
            return;
        }

        GameObject player = Instantiate(firstPersonPlayerPrefab);
        player.name = "First Person Player";
        player.transform.SetParent(root);
        player.transform.position = grid.GetCellWorldPosition(new Vector3Int(0, 0, 0)) + Vector3.down * 0.45f;
        player.AddComponent<PlayerMotionReader>();

        Transform cameraTarget = player.transform.Find("PlayerCameraRoot");
        Camera mainCamera = Camera.main;
        if (cameraTarget != null && mainCamera != null)
        {
            mainCamera.transform.SetParent(cameraTarget, false);
            mainCamera.transform.localPosition = Vector3.zero;
            mainCamera.transform.localRotation = Quaternion.identity;
        }
    }

    private void CreateBlueBall(Grid3D grid, Transform root)
    {
        GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.name = "Blue Ball AI";
        ball.transform.SetParent(root);
        Vector3Int size = grid.GridSize;
        ball.transform.position = grid.GetCellWorldPosition(new Vector3Int(size.x - 1, 0, size.z - 1)) + Vector3.up * 0.45f;
        ball.transform.localScale = Vector3.one * 0.75f;
        Rigidbody rigidbody = ball.AddComponent<Rigidbody>();
        rigidbody.mass = 0.6f;
        ball.AddComponent<BlueBallAI>();
        SetColor(ball.GetComponent<Renderer>(), new Color(0.08f, 0.42f, 1f));
    }

    private static void SetColor(Renderer renderer, Color color)
    {
        MaterialPropertyBlock properties = new MaterialPropertyBlock();
        properties.SetColor("_BaseColor", color);
        properties.SetColor("_Color", color);
        renderer.SetPropertyBlock(properties);
    }
}
