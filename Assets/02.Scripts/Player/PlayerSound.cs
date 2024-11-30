using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    private Animator playerAnimator;
    
    public string[] stepSounds;

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
    }
    public void PlayStepSound()
    {
        GameManager.sm.PlayRandomSound(stepSounds);
    }
}
