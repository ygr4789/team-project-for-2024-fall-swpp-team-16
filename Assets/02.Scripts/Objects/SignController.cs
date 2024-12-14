using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignController : MonoBehaviour
{
    [SerializeField] private GameObject tutorialSignUI; // UI GameObject
    [SerializeField] private string textContent;
    [SerializeField] private float imageWidth = 1500;
    [SerializeField] private float imageHeight = 800;
    
    private RectTransform imageRect; // Image의 RectTransform
    private TextMeshProUGUI textComponent; // Text Component
    private bool isActive = false;
    private float padding = 0.2f; // 10% padding

    private void Awake()
    {
        textContent = textContent.Replace("\\n", "\n");
        // 자식 객체에서 RectTransform과 Text 자동으로 찾기
        if (tutorialSignUI != null)
        {
            imageRect = tutorialSignUI.GetComponentInChildren<Image>().rectTransform;
            textComponent = tutorialSignUI.GetComponentInChildren<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogError("TutorialSignUI is not assigned!");
        }
    }

    private void Start()
    {
        tutorialSignUI.SetActive(false);
    }

    public void Inspect(GameObject floatingText)
    {
        if (isActive)
        {
            tutorialSignUI.SetActive(false);
            GameManager.sm.PlaySound("sign");
            isActive = false;
        }
        else
        {
            if (imageRect == null || textComponent == null)
            {
                Debug.LogError("Image or Text component is not found!");
                return;
            }

            // Set the text content
            textComponent.text = textContent;

            // Update the size of the image
            imageRect.sizeDelta = new Vector2(imageWidth, imageHeight);

            // Calculate and update the size of the text with 10% padding
            float textWidth = imageWidth * (1 - 2 * padding);
            float textHeight = imageHeight * (1 - 2 * padding);
            textComponent.rectTransform.sizeDelta = new Vector2(textWidth, textHeight);

            tutorialSignUI.SetActive(true);
            GameManager.sm.PlaySound("sign");
            isActive = true;
        }
    }
}