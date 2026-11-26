using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

/// <summary>Creates the small scene-navigation menu used by the prototype.</summary>
public sealed class GameMenu : MonoBehaviour
{
    private const string GameplayScene = "SampleGameplay";
    private const string PathfindingScene = "SamplePathfinding";

    private void Awake()
    {
        CreateEventSystem();
        CreateCanvas();
    }

    private static void CreateEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() != null)
            return;

        GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
        DontDestroyOnLoad(eventSystem);
    }

    private void CreateCanvas()
    {
        GameObject canvasObject = new GameObject("Game Menu", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.transform.SetParent(transform, false);

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        GameObject panel = CreateElement("Panel", canvasObject.transform);
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.04f, 0.07f, 0.12f, 0.9f);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0f, 1f);
        panelRect.anchorMax = new Vector2(0f, 1f);
        panelRect.pivot = new Vector2(0f, 1f);
        panelRect.anchoredPosition = new Vector2(32f, -32f);
        panelRect.sizeDelta = new Vector2(340f, 250f);

        CreateLabel("Mindscape", panel.transform, new Vector2(24f, -24f), 30);
        CreateButton("Play Gameplay", panel.transform, new Vector2(24f, -82f), () => LoadScene(GameplayScene));
        CreateButton("Pathfinding Test", panel.transform, new Vector2(24f, -148f), () => LoadScene(PathfindingScene));
    }

    private static GameObject CreateElement(string name, Transform parent)
    {
        GameObject element = new GameObject(name, typeof(RectTransform));
        element.transform.SetParent(parent, false);
        return element;
    }

    private static void CreateLabel(string text, Transform parent, Vector2 position, int fontSize)
    {
        GameObject labelObject = CreateElement("Label", parent);
        Text label = labelObject.AddComponent<Text>();
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.text = text;
        label.fontSize = fontSize;
        label.color = Color.white;
        label.alignment = TextAnchor.MiddleLeft;
        RectTransform rect = label.rectTransform;
        rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(292f, 42f);
    }

    private static void CreateButton(string text, Transform parent, Vector2 position, UnityEngine.Events.UnityAction action)
    {
        GameObject buttonObject = CreateElement(text, parent);
        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.12f, 0.45f, 0.78f, 1f);
        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);

        RectTransform rect = button.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(292f, 48f);
        CreateLabel(text, button.transform, new Vector2(12f, -3f), 20);
    }

    private static void LoadScene(string sceneName)
    {
        if (GameManager.Instance == null || !GameManager.Instance.LoadScene(sceneName))
            Debug.LogError($"The menu could not open '{sceneName}'.");
    }
}
