using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
	public GameObject scoreUIPanel; // Reference to the score UI panel

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

	private void OpenDoor()
    {
       	// opening door animation
    }
}
