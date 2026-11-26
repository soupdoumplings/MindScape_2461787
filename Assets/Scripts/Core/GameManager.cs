using System;
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

    public event Action<string> SceneLoadStarted;
    public event Action<string> SceneLoadCompleted;
    public event Action<string, string> SceneLoadFailed;

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

    /// <summary>
    /// Begins a single-mode transition to a scene included in Build Settings.
    /// Returns false and logs a reason when the request cannot be accepted.
    /// </summary>
    public bool LoadScene(string sceneName)
    {
        if (IsTransitioning)
        {
            ReportFailure(sceneName, "A scene transition is already in progress.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(sceneName))
        {
            ReportFailure(sceneName, "The requested scene name is empty.");
            return false;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            ReportFailure(sceneName, "The scene is not included in Build Settings or cannot be loaded.");
            return false;
        }

        if (SceneManager.GetActiveScene().name == sceneName)
        {
            return true;
        }

        StartCoroutine(LoadSceneRoutine(sceneName));
        return true;
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        IsTransitioning = true;
        SceneLoadStarted?.Invoke(sceneName);

        AsyncOperation operation;
        try
        {
            operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        }
        catch (Exception exception)
        {
            ReportFailure(sceneName, exception.Message);
            IsTransitioning = false;
            yield break;
        }

        if (operation == null)
        {
            ReportFailure(sceneName, "Unity did not create a load operation.");
            IsTransitioning = false;
            yield break;
        }

        yield return operation;
        IsTransitioning = false;
        SceneLoadCompleted?.Invoke(sceneName);
    }

    private void ReportFailure(string sceneName, string reason)
    {
        Debug.LogError($"GameManager could not load scene '{sceneName}': {reason}", this);
        SceneLoadFailed?.Invoke(sceneName, reason);
    }
}
