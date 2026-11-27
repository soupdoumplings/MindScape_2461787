using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

/// <summary>Creates the small scene-navigation menu used by the prototype.</summary>
public sealed class GameMenu : MonoBehaviour
{
    private const string GameplayScene = "SampleGameplay";
    private const string PathfindingScene = "SamplePathfinding";
    private Text settingsText;
    private Text volumeValue;
    private Text sensitivityValue;
    private Text frameRateValue;
    private GameObject settingsPanel;

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
        panelRect.sizeDelta = new Vector2(340f, 290f);

        CreateLabel("Mindscape", panel.transform, new Vector2(24f, -24f), 30);
        CreateButton("Play Gameplay", panel.transform, new Vector2(24f, -82f), () => LoadScene(GameplayScene));
        CreateButton("Pathfinding Test", panel.transform, new Vector2(24f, -148f), () => LoadScene(PathfindingScene));
        CreateButton("Settings", panel.transform, new Vector2(24f, -214f), ToggleSettings);
        CreateSettingsPanel(canvasObject.transform);
    }

    private void CreateSettingsPanel(Transform parent)
    {
        GameObject panel = CreateElement("Settings", parent);
        settingsPanel = panel;
        Image image = panel.AddComponent<Image>();
        image.color = new Color(0.04f, 0.07f, 0.12f, 0.94f);
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(390f, -32f);
        rect.sizeDelta = new Vector2(400f, 310f);
        settingsText = CreateLabel("Settings", panel.transform, new Vector2(22f, -20f), 26);
        volumeValue = CreateSlider("Volume", panel.transform, new Vector2(22f, -78f), 0f, 1f, GameSettings.Instance.Volume, value => GameSettings.Instance.SetVolume(value));
        sensitivityValue = CreateSlider("Look sensitivity", panel.transform, new Vector2(22f, -148f), 0.5f, 2f, GameSettings.Instance.Sensitivity, value => GameSettings.Instance.SetSensitivity(value));
        frameRateValue = CreateSlider("FPS cap", panel.transform, new Vector2(22f, -218f), 0f, 2f, GameSettings.Instance.FrameRate == 30 ? 0f : GameSettings.Instance.FrameRate == 60 ? 1f : 2f, value => GameSettings.Instance.SetFrameRate(value < 0.5f ? 30 : value < 1.5f ? 60 : 120));
        RefreshSettings();
        panel.SetActive(false);
    }

    private void ToggleSettings()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
        RefreshSettings();
    }

    private void RefreshSettings()
    {
        if (settingsText == null || GameSettings.Instance == null) return;
        settingsText.text = "Settings";
        if (volumeValue != null)
            volumeValue.text = $"{GameSettings.Instance.Volume:0%}";
        if (sensitivityValue != null)
            sensitivityValue.text = $"{GameSettings.Instance.Sensitivity:0.00}x";
        if (frameRateValue != null)
            frameRateValue.text = $"{GameSettings.Instance.FrameRate} FPS";
    }

    private Text CreateSlider(string label, Transform parent, Vector2 position, float min, float max, float value, UnityEngine.Events.UnityAction<float> onChanged)
    {
        CreateLabel(label, parent, position, 18);
        Text valueLabel = CreateLabel(string.Empty, parent, position + new Vector2(210f, 0f), 18);
        valueLabel.alignment = TextAnchor.MiddleRight;
        valueLabel.rectTransform.sizeDelta = new Vector2(134f, 42f);
        GameObject sliderObject = CreateElement(label + " Slider", parent);
        Slider slider = sliderObject.AddComponent<Slider>();
        RectTransform rect = slider.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = position + new Vector2(0f, -36f);
        rect.sizeDelta = new Vector2(344f, 20f);
        Image background = sliderObject.AddComponent<Image>();
        background.color = new Color(0.15f, 0.2f, 0.27f, 1f);
        GameObject fill = CreateElement("Fill", sliderObject.transform);
        Image fillImage = fill.AddComponent<Image>(); fillImage.color = new Color(0.12f, 0.55f, 0.95f, 1f);
        RectTransform fillRect = fill.GetComponent<RectTransform>(); fillRect.anchorMin = Vector2.zero; fillRect.anchorMax = new Vector2(1f, 1f); fillRect.offsetMin = new Vector2(8f, 4f); fillRect.offsetMax = new Vector2(-8f, -4f);
        GameObject handle = CreateElement("Handle", sliderObject.transform);
        Image handleImage = handle.AddComponent<Image>(); handleImage.color = Color.white;
        RectTransform handleRect = handle.GetComponent<RectTransform>(); handleRect.sizeDelta = new Vector2(18f, 28f);
        slider.fillRect = fillRect; slider.handleRect = handleRect; slider.targetGraphic = handleImage;
        slider.minValue = min;
        slider.maxValue = max;
        slider.value = min;
        slider.SetValueWithoutNotify(value);
        slider.onValueChanged.AddListener(changedValue =>
        {
            onChanged(changedValue);
            RefreshSettings();
        });
        return valueLabel;
    }

    private static GameObject CreateElement(string name, Transform parent)
    {
        GameObject element = new GameObject(name, typeof(RectTransform));
        element.transform.SetParent(parent, false);
        return element;
    }

    private static Text CreateLabel(string text, Transform parent, Vector2 position, int fontSize)
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
        return label;
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
