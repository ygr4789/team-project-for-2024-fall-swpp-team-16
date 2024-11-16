using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
	public GameObject scoreUIPanel; // Reference to the score UI panel
    public int[] answerNotes; // Array to store notes for this door
    private List<int> playedNotes = new List<int>(); // List to store played notes
    public GameObject doorWing; // Reference to the door object

    void Start()
    {
        scoreUIPanel.SetActive(false);
    }

    public void Inspect()
    {
        // if all scores are collected, the user needs to play music
		if (GameManager.im.HasAllScores())
        {
            // Activate the Score UI to display it
            scoreUIPanel.SetActive(true);
			
			Debug.Log("Play the music to open the door");
        }
        else
        {
            Debug.Log("You need to collect all the scores to open the door");
        }
    }

    void Update()
    {
        // if the score UI is active
        if (scoreUIPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) playedNotes.Add(1);
            if (Input.GetKeyDown(KeyCode.Alpha2)) playedNotes.Add(2);
            if (Input.GetKeyDown(KeyCode.Alpha3)) playedNotes.Add(3);
            if (Input.GetKeyDown(KeyCode.Alpha4)) playedNotes.Add(4);
            if (Input.GetKeyDown(KeyCode.Alpha5)) playedNotes.Add(5);
            if (Input.GetKeyDown(KeyCode.Alpha6)) playedNotes.Add(6);
            if (Input.GetKeyDown(KeyCode.Alpha7)) playedNotes.Add(7);
            if (Input.GetKeyDown(KeyCode.Alpha8)) playedNotes.Add(8);
            
            if (CheckNotes())
            {
                if (playedNotes.Count == answerNotes.Length)
                {
                    Debug.Log("Correct notes played. Door is opening.");
                    OpenDoor();
                    playedNotes.Clear();
                    scoreUIPanel.SetActive(false);
                }
            }
            else
            {
                Debug.Log("Incorrect notes played. Try again.");
                GameManager.sm.PlaySound("wrong-answer");
                playedNotes.Clear();
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

	private void OpenDoor()
    {
		GameManager.sm.PlaySound("opening-door");
       	// opening door animation
        StartCoroutine(OpenDoorAnimation(doorWing));
    }
    
    private IEnumerator OpenDoorAnimation(GameObject door)
    {
        float duration = 1.5f; // animation duration
        float startRotation = door.transform.eulerAngles.y;
        float endRotation = startRotation + 90;
        float time = 0;

        while (time < duration)
        {
            float yRotation = Mathf.Lerp(startRotation, endRotation, time / duration);
            door.transform.eulerAngles = new Vector3(door.transform.eulerAngles.x, yRotation, door.transform.eulerAngles.z);
            time += Time.deltaTime;
            yield return null;
        }

        door.transform.eulerAngles = new Vector3(door.transform.eulerAngles.x, endRotation, door.transform.eulerAngles.z);
    }
}
