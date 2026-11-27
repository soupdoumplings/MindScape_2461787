using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Starts the application from the Core Bootstrap scene and keeps the bootstrap
/// object alive while gameplay scenes are loaded.
/// </summary>
public sealed class GameBootstrap : MonoBehaviour
{
    [SerializeField] private string startupScene = "SampleGameplay";

    private static GameBootstrap instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        if (SceneManager.GetActiveScene().name != startupScene)
            SceneManager.LoadSceneAsync(startupScene, LoadSceneMode.Single);
    }
}
