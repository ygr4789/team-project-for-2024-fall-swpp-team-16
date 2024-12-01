using UnityEngine;
using UnityEngine.UI;

public class SettingsModalController : MonoBehaviour
{
	public Sprite closeButtonSprite = null;
	public Sprite quitButtonSprite = null;
	public Sprite settingsButtonSprite = null;

    private GameObject canvas;
    private GameObject settingsModal;
    private Slider soundSlider;
    private Slider bgmSlider;

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
            canvas = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas.tag = "MainCanvas";
        }
        Canvas canvasComponent = canvas.GetComponent<Canvas>();
        canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // Create Settings Modal
        settingsModal = new GameObject("SettingsModal", typeof(Image));
        settingsModal.transform.SetParent(canvas.transform, false);
        RectTransform modalRect = settingsModal.GetComponent<RectTransform>();
        modalRect.sizeDelta = new Vector2(400, 300);
        settingsModal.GetComponent<Image>().color = new Color(0, 0, 0, 0.8f);
        settingsModal.SetActive(false);

        // Close Button
        GameObject closeButton = CreateButton("CloseButton", "X", new Vector2(180, 130), settingsModal.transform, null);
        closeButton.GetComponent<Button>().onClick.AddListener(CloseModal);

        // Quit Button
        GameObject quitButton = CreateButton("QuitButton", "Quit Game", new Vector2(0, -100), settingsModal.transform, null);
        quitButton.GetComponent<Button>().onClick.AddListener(QuitGame);

        // Master Volume Slider
        CreateSlider("MasterVolumeSlider", "Master Volume", new Vector2(0, 50), settingsModal.transform, AdjustSoundLevel, AudioListener.volume);
		
        // BGM Volume Slider
        CreateSlider("BGMVolumeSlider", "BGM Volume", new Vector2(0, -50), settingsModal.transform, AdjustBgmVolume, GameManager.sm.masterVolumeBgm);

        // Settings Button
        GameObject settingsButton = CreateButton("SettingsButton", "Settings", new Vector2(800, 400), canvas.transform, null);
        settingsButton.GetComponent<Button>().onClick.AddListener(ToggleSettings);
    }

    private GameObject CreateButton(string name, string buttonText, Vector2 position, Transform parent, Sprite buttonSprite)
    {
        GameObject button = new GameObject(name, typeof(Button), typeof(Image));
        button.transform.SetParent(parent, false);

        RectTransform rectTransform = button.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200, 50);
        rectTransform.anchoredPosition = position;

        Button btnComponent = button.GetComponent<Button>();
        Image btnImage = button.GetComponent<Image>();
        btnImage.color = Color.white;

        if (buttonSprite != null)
        {
            btnImage.sprite = buttonSprite;
        }

        GameObject textObject = CreateText(name + "Text", buttonText, Vector2.zero, button.transform);
        Text textComponent = textObject.GetComponent<Text>();
        textComponent.alignment = TextAnchor.MiddleCenter;

        return button;
    }

    private void CreateSlider(string name, string label, Vector2 position, Transform parent, UnityEngine.Events.UnityAction<float> onChange, float initialValue)
    {
        GameObject labelObject = CreateText(name + "Label", label, position + new Vector2(0, 30), parent);

        GameObject sliderObject = new GameObject(name, typeof(Slider));
        sliderObject.transform.SetParent(parent, false);
        RectTransform sliderRect = sliderObject.GetComponent<RectTransform>();
        sliderRect.sizeDelta = new Vector2(300, 20);
        sliderRect.anchoredPosition = position;

        Slider slider = sliderObject.GetComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = initialValue;
        slider.onValueChanged.AddListener(onChange);

		// Disable Navigation
    	var navigation = slider.navigation;
    	navigation.mode = Navigation.Mode.None; // Disable arrow key and gamepad navigation
    	slider.navigation = navigation;

        Image sliderBackground = sliderObject.AddComponent<Image>();
        sliderBackground.color = Color.gray;

        GameObject handleObject = new GameObject("Handle", typeof(Image));
        handleObject.transform.SetParent(sliderObject.transform);
        RectTransform handleRect = handleObject.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(20, 40);
        handleRect.anchoredPosition = Vector2.zero;

        Image handleImage = handleObject.GetComponent<Image>();
        handleImage.color = Color.white;

        slider.targetGraphic = handleImage;
        slider.handleRect = handleRect;
    }

    private GameObject CreateText(string name, string text, Vector2 position, Transform parent)
    {
        GameObject textObject = new GameObject(name, typeof(Text));
        textObject.transform.SetParent(parent, false);

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200, 50);
        rectTransform.anchoredPosition = position;

        Text textComponent = textObject.GetComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        textComponent.fontSize = 24;
        textComponent.color = Color.white;

        return textObject;
    }

    public void ToggleSettings()
    {
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

    public void AdjustBgmVolume(float value)
    {
        GameManager.sm.SetVolumeBGM(value);
    }
}