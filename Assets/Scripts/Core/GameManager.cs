using UnityEngine;

/// <summary>
/// Persistent application-level game service.
/// Place exactly one instance in the Bootstrap scene.
/// </summary>
public sealed class GameManager : MonoBehaviour
{
    /// <summary>The active persistent game manager, if the application has started.</summary>
    public static GameManager Instance { get; private set; }

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

}
