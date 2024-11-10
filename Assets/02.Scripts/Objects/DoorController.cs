using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    void Start()
    {
        
    }

    public void Inspect()
    {
        // if all scores are collected, the user needs to play music
		if (GameManager.im.HasAllScores())
        {
            // score UI pops up
			// if the play is correct, the door opens
        }
    }

	private void OpenDoor()
    {
       	// opening door animation
    }
}
