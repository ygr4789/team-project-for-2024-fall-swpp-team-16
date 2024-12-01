using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSelectionManager : MonoBehaviour
{
    public List<LevelSelectionNoteController> levelNotes; // Assign all notes (UI Images) in the Inspector
    public TMP_Text stageNumberText;

    private int currentSelectedIndex = 0;

    // Sliding animation properties
    public float slideDuration = 0.9f;
    private RectTransform rectTransform;  // To move the UI panel
    private Vector2 offScreenPosition;
    private Vector2 onScreenPosition;

    void Awake()
    {
        // Get RectTransform and define positions
        rectTransform = GetComponent<RectTransform>();
        onScreenPosition = rectTransform.anchoredPosition; // Current position
        offScreenPosition = new Vector2(onScreenPosition.x, -Screen.height); // Off-screen position

        // Start off-screen
        rectTransform.anchoredPosition = offScreenPosition;
    }

    void Start()
    {
        // Ensure the first unlocked stage is selected at the start
        currentSelectedIndex = FindLastUnlockedIndex();
        StartCoroutine(SlideIn()); // Slide in the level selection screen
        UpdateSelection();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveSelection(1);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveSelection(-1);
        }

        if (Input.GetKeyDown(KeyCode.Return)) // Confirm selection
        {
            ConfirmSelection();
        }
    }

    void MoveSelection(int direction)
    {
        // Deselect the current note
        levelNotes[currentSelectedIndex].isSelected = false;

        // Move to the next unlocked stage
        do
        {
            currentSelectedIndex += direction;

            // Wrap around the list if needed
            if (currentSelectedIndex < 0)
            {
                currentSelectedIndex = levelNotes.Count - 1;
            }
            else if (currentSelectedIndex >= levelNotes.Count)
            {
                currentSelectedIndex = 0;
            }
        } while (levelNotes[currentSelectedIndex].isLocked); // Skip locked stages

        // Update the selection
        UpdateSelection();
    }

    void UpdateSelection()
    {
        for (int i = 0; i < levelNotes.Count; i++)
        {
            levelNotes[i].isSelected = (i == currentSelectedIndex);
            levelNotes[i].UpdateNoteAppearance();
        }
        stageNumberText.text = $"- {levelNotes[currentSelectedIndex].stageNumber} -";
    }

    void ConfirmSelection()
{
    LevelSelectionNoteController selectedNote = levelNotes[currentSelectedIndex];

    if (selectedNote.isLocked)
    {
        Debug.LogWarning($"Stage {selectedNote.stageNumber} is locked and cannot be selected.");
        return; // Prevent loading locked stages
    }

    Debug.Log($"Loading Stage {selectedNote.stageNumber}...");

    // Load the scene corresponding to the selected stage
    string sceneName = $"Test Stage{selectedNote.stageNumber} JS"; // Example: Stage1, Stage2, etc.

    if (Application.CanStreamedLevelBeLoaded(sceneName))
    {
        SceneManager.LoadScene(sceneName);
    }
    else
    {
        Debug.LogError($"Scene '{sceneName}' not found. Please ensure it is added to the Build Settings.");
    }
}


    int FindLastUnlockedIndex()
    {
        for (int i = 0; i < levelNotes.Count; i++)
        {
            if (levelNotes[i].isLocked)
            {
                if (i == 0) return 0;
                return i - 1;
            }
        }
        return 0; // Default to the first stage if no unlocked stages are found
    }

    IEnumerator SlideIn()
    {
        float elapsedTime = 0f;

        while (elapsedTime < slideDuration)
        {
            elapsedTime += Time.deltaTime;
            rectTransform.anchoredPosition = Vector2.Lerp(offScreenPosition, onScreenPosition, elapsedTime / slideDuration);
            yield return null;
        }

        // Snap to the final position to ensure precision
        rectTransform.anchoredPosition = onScreenPosition;
    }
}
