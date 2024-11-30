using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    private Animator playerAnimator;
    
    public string[] stepSounds;

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        if (playerAnimator != null)
        {
            Debug.Log("Animator component found");
        }
        else
        {
            Debug.Log("Animator component not found");
        }
        
        stepSounds = new string[]
        {
            "Footsteps_Walk_01",
            "Footsteps_Walk_02",
            "Footsteps_Walk_03",
            "Footsteps_Walk_04",
            "Footsteps_Walk_05",
            "Footsteps_Walk_06",
            "Footsteps_Walk_07",
            "Footsteps_Walk_08",
            "Footsteps_Walk_09",
            "Footsteps_Walk_10",
        };
    }

    public void PlayStepSound()
    {
        Debug.Log("PlayStepSound");
        GameManager.sm.PlayRandomSound(stepSounds, .4f);
    }
    
    public void PlayLandSound()
    {
        Debug.Log("PlayLandSound");
        GameManager.sm.PlaySound("Footsteps_Land_01", .3f);
    }
}
