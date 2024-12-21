using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // For Image components

public class DoorController : MonoBehaviour
{
    public List<GameObject> scores = new List<GameObject>(); // list of scores to collect
	public GameObject scoreUIPanel; // Reference to the score UI panel
    public int[] answerNotes; // Array to store notes for this door
    public List<int> playedNotes = new List<int>(); // List to store played notes
    public GameObject doorWing; // Reference to the door object
    
    public GameObject rippleEffectPrefab; // Reference to the ripple effect prefab
    public GameObject[] coloredNotes; // Array to store note prefabs

    void Start()
    {
        scoreUIPanel.SetActive(false);

        // Initialize all notes to black
        foreach (var note in coloredNotes)
        {
            Image image = note.GetComponent<Image>();
            if (image != null)
            {
                image.color = Color.black;
            }
            else
            {
                Debug.LogError($"Missing Image component on {note.name}");
            }
        }
    }

    public void Inspect(GameObject floatingText)
    {
        if (GameManager.gm.isUIOpen){
            return;
        }
        
        // if all scores are collected, the user needs to play music
		if (GameManager.im.HasAllScores())
        {
            // Activate the Score UI to display it
            GameManager.gm.isUIOpen = true;
            scoreUIPanel.SetActive(true);
			Debug.Log("Play the music to open the door");
            floatingText.SendMessage("Hide");
            
            // Disable player input
            DisableInput();
        }
        else
        {
            Debug.Log("You need to collect all the scores to open the door");
        }
    }
    
    private void DisableInput()
    {
        PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement != null)
        {
            PlayerInput playerInput = playerMovement.GetPlayerInput();
            if (playerInput != null)
                playerInput.Active = false; // 플레이어 움직임 활성화
        }
    }

    private void EnableInput()
    {
        PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement != null)
        {
            PlayerInput playerInput = playerMovement.GetPlayerInput();
            if (playerInput != null)
                playerInput.Active = false; // 플레이어 움직임 활성화
        }
    }

    

    void Update()
    {
        // Playing notes
        if (scoreUIPanel.activeSelf)
        {
            for (int i = 1; i <= 8; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + (i - 1)))
                {
                    playedNotes.Add(i);

                    if (CheckNotes())
                    {
                        UpdateColoredNote(i);

                        if (playedNotes.Count == answerNotes.Length)
                        {
                            Debug.Log("Correct notes played. Door is opening.");
                            GameManager.stm.CompleteCurrentStage();
                            GetComponent<Collider>().enabled = false;
                            GameManager.gm.isUIOpen = false;
                            StartCoroutine(StageClearDirection());
                            // TODO: change camera view to the back of the player
                        }
                    }
                    else
                    {
                        Debug.Log("Incorrect notes played. Try again.");
                        GameManager.sm.PlaySound("wrong-answer");
                        playedNotes.Clear();
                        ResetColoredNotes();
                    }
                }
            }
        }
    }

    private void UpdateColoredNote(int note)
    {
        if (playedNotes.Count - 1 < coloredNotes.Length)
        {
            GameObject noteObject = coloredNotes[playedNotes.Count - 1];
            Image image = noteObject.GetComponent<Image>();
            if (image != null)
            {
                image.color = GetNoteColor(note);

                // Create ripple effect
                GameObject ripple = Instantiate(rippleEffectPrefab,
                    noteObject.transform.position + new Vector3(0, 0, -1), Quaternion.identity);
                ripple.transform.SetParent(noteObject.transform);
                ripple.transform.localScale = Vector3.one * 10;
                Destroy(ripple, 1.0f);

                // StartCoroutine(ScaleEffect(noteObject.transform)); // IF NOTE SHOULD SCALE WHEN PLAYED UNCOMMENT THIS
            }
        }
    }

    private Color GetNoteColor(int note)
    {
        switch (note)
        {
            case 1: return Color.red;
            case 2: return new Color(1f, 0.5f, 0f); // Orange
            case 3: return Color.yellow;
            case 4: return Color.green;
            case 5: return Color.blue;
            case 6: return new Color(0.5f, 0f, 0.5f); // Purple
            case 7: return new Color(1f, 0.75f, 0.8f); // Pink
            default: return Color.white;
        }
    }

    private IEnumerator ScaleEffect(Transform targetTransform)
    {
        float duration = 0.15f;
        Vector3 initialScale = Vector3.zero;
        Vector3 finalScale = Vector3.one;

        float time = 0;
        while (time < duration)
        {
            targetTransform.localScale = Vector3.Lerp(initialScale, finalScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        targetTransform.localScale = finalScale;
    }

    private void ResetColoredNotes()
    {
        foreach (GameObject note in coloredNotes)
        {
            Image image = note.GetComponent<Image>();
            if (image != null)
            {
                image.color = Color.black;
            }
        }
    }

    private bool CheckNotes()
    {
        for (int i = 0; i < playedNotes.Count; i++)
        {
            if (playedNotes[i] != answerNotes[i])
            {
                return false;
            }
        }
        return true;
    }

    private IEnumerator StageClearDirection()
    {
        // UI off
        scoreUIPanel.SetActive(false);
        playedNotes.Clear();
        
       	// opening door sound & animation
		GameManager.sm.PlaySound("opening-door");
        yield return OpenDoorAnimation(doorWing);
        StartCoroutine(GameManager.gm.controller.GetComponent<PlayerMovement>().WalkToPoint(transform.position, 1.5f));
        
        // camera effect
        GameManager.em.FadeInCircleTransition();
        // Go Back to the stage selection scene
        StartCoroutine(GameManager.stm.WaitAndLoadScene("StageScene"));
    }
    
    private IEnumerator OpenDoorAnimation(GameObject door)
    {
        float duration = 1f; // animation duration
        float startRotation = door.transform.localEulerAngles.y;
        float endRotation = startRotation + 90;
        float time = 0;

        while (time < duration)
        {
            float yRotation = Mathf.Lerp(startRotation, endRotation, time / duration);
            door.transform.localEulerAngles = new Vector3(door.transform.localEulerAngles.x, yRotation, door.transform.localEulerAngles.z);
            time += Time.deltaTime;
            yield return null;
        }

        door.transform.localEulerAngles = new Vector3(door.transform.localEulerAngles.x, endRotation, door.transform.localEulerAngles.z);
    }
}
