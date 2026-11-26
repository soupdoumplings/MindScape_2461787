using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Persistent application-level game service.
/// Place exactly one instance in the Bootstrap scene.
/// </summary>
public sealed class GameManager : MonoBehaviour
{
    [SerializeField] private string startupScene = "SampleGameplay";

    /// <summary>The active persistent game manager, if the application has started.</summary>
    public static GameManager Instance { get; private set; }

    /// <summary>True while an accepted scene transition is still loading.</summary>
    public bool IsTransitioning { get; private set; }

    public string StartupScene => startupScene;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticState()
    {
        Instance = null;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("A duplicate GameManager was destroyed.", this);
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (!string.IsNullOrWhiteSpace(startupScene) &&
            SceneManager.GetActiveScene().name != startupScene)
        {
            LoadScene(startupScene);
        }
    }

    /// <summary>Begins a single-mode asynchronous scene transition.</summary>
    public bool LoadScene(string sceneName)
    {
        if (IsTransitioning || string.IsNullOrWhiteSpace(sceneName) ||
            SceneManager.GetActiveScene().name == sceneName)
        {
            return false;
        }

        StartCoroutine(LoadSceneRoutine(sceneName));
        return true;
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        IsTransitioning = true;
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        if (operation != null)
        {
            yield return operation;
        }

        IsTransitioning = false;
    }
}
