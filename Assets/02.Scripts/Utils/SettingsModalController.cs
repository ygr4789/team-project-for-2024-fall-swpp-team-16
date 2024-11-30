using UnityEngine;
using UnityEngine.UI;

public class SettingsModalController : MonoBehaviour
{
    private GameObject canvas;
    private GameObject settingsModal;
    private Slider soundSlider;

    void Start()
    {
        CreateUI();
    }

    private void CreateUI()
    {
        // Create Canvas
        canvas = GameObject.FindWithTag("MainCanvas");
		if (canvas == null)
		{
    		// Create the canvas if it doesn't exist
    		canvas = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
    		canvas.tag = "MainCanvas";
		}
        Canvas canvasComponent = canvas.GetComponent<Canvas>();
        canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;

        // Adjust Canvas Scaler
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // Create Settings Modal
        settingsModal = new GameObject("SettingsModal", typeof(Image));
        settingsModal.transform.SetParent(canvas.transform, false);
        RectTransform modalRect = settingsModal.GetComponent<RectTransform>();
        modalRect.sizeDelta = new Vector2(400, 300);
        settingsModal.GetComponent<Image>().color = new Color(0, 0, 0, 0.8f); // Semi-transparent background
        settingsModal.SetActive(false); // Initially hidden

        // Create Close Button
        GameObject closeButton = CreateButton("CloseButton", "X", new Vector2(180, 130), settingsModal.transform);
        closeButton.GetComponent<Button>().onClick.AddListener(CloseModal);

        // Create Quit Button
        GameObject quitButton = CreateButton("QuitButton", "Quit Game", new Vector2(0, -100), settingsModal.transform);
        quitButton.GetComponent<Button>().onClick.AddListener(QuitGame);

        // Create Sound Slider
        GameObject sliderLabel = CreateText("SoundLabel", "Sound Volume", new Vector2(0, 50), settingsModal.transform);
        GameObject sliderObject = new GameObject("SoundSlider", typeof(Slider));
        sliderObject.transform.SetParent(settingsModal.transform, false);
        RectTransform sliderRect = sliderObject.GetComponent<RectTransform>();
        sliderRect.sizeDelta = new Vector2(300, 20);
        sliderRect.anchoredPosition = new Vector2(0, 0);

        // Configure Slider
        soundSlider = sliderObject.GetComponent<Slider>();
        var navigation = soundSlider.navigation; // Get current navigation settings
        navigation.mode = Navigation.Mode.None; // Disable navigation
        soundSlider.navigation = navigation; // Apply the changes
        soundSlider.minValue = 0f;
        soundSlider.maxValue = 1f;
        soundSlider.value = AudioListener.volume;
        soundSlider.onValueChanged.AddListener(AdjustSoundLevel);
        
        // Add Background Image for Slider
        Image sliderBackground = sliderObject.AddComponent<Image>();
        sliderBackground.color = Color.gray; // Gray background for slider

        // Add Handle Image for Slider
        GameObject handleObject = new GameObject("Handle", typeof(Image));
        handleObject.transform.SetParent(sliderObject.transform);
        RectTransform handleRect = handleObject.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(20, 40); // Adjust handle size
        handleRect.anchorMin = new Vector2(0.5f, 0.5f);
        handleRect.anchorMax = new Vector2(0.5f, 0.5f);
        handleRect.pivot = new Vector2(0.5f, 0.5f);
        handleRect.anchoredPosition = Vector2.zero;

        Image handleImage = handleObject.GetComponent<Image>();
        handleImage.color = Color.white; // White handle for better visibility

        soundSlider.targetGraphic = handleImage;
        soundSlider.fillRect = null; // No fill by default
        soundSlider.handleRect = handleRect;

        // Create Settings Button
        GameObject settingsButton = CreateButton("SettingsButton", "Settings", new Vector2(800, 400), canvas.transform);
        settingsButton.GetComponent<Button>().onClick.AddListener(ToggleSettings);
    }

    private GameObject CreateButton(string name, string buttonText, Vector2 position, Transform parent)
    {
        // Create Button GameObject
        GameObject button = new GameObject(name, typeof(Button), typeof(Image));
        button.transform.SetParent(parent, false);
		button.SetActive(true);

        // Configure RectTransform
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200, 50);
        rectTransform.anchoredPosition = position;

        // Set Button Visuals
        Button btnComponent = button.GetComponent<Button>();
        Image btnImage = button.GetComponent<Image>();
        btnImage.color = Color.white;

        // Add Text to Button
        GameObject textObject = CreateText(name + "Text", buttonText, Vector2.zero, button.transform);
        Text textComponent = textObject.GetComponent<Text>();
        textComponent.alignment = TextAnchor.MiddleCenter;

        return button;
    }

    private GameObject CreateText(string name, string text, Vector2 position, Transform parent)
    {
        // Create Text GameObject
        GameObject textObject = new GameObject(name, typeof(Text));
        textObject.transform.SetParent(parent, false);

        // Configure RectTransform
        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200, 50);
        rectTransform.anchoredPosition = position;

        // Set Text Properties
        Text textComponent = textObject.GetComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        textComponent.fontSize = 24;
        textComponent.color = Color.black;

        return textObject;
    }

    public void ToggleSettings()
    {
        Debug.Log("Settings Button Clicked!");
        settingsModal.SetActive(!settingsModal.activeSelf);
    }

    public void CloseModal()
    {
        settingsModal.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();

        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void AdjustSoundLevel(float value)
    {
        AudioListener.volume = value;
    }
}