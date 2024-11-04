using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResonatableObject : MonoBehaviour
{
    [SerializeField] private PitchType[] properties;
    private bool isPlayingRipples = false;
    [SerializeField] private int colliderNum = 0;

    public void OnEnterRadius()
    {
        colliderNum++;
        if (!isPlayingRipples)
        {
            foreach (PitchType pitch in properties)
            {
                isPlayingRipples = true;
                TriggerRepplesEffect(pitch);
            }
        }
    }

    public void OnExitRadius()
    {
        colliderNum--;
        if (colliderNum <= 0)
        {
            isPlayingRipples = false;
            Debug.Log("Ripple: Stopped!");
            GameManager.em.StopRipples(transform);
        }
    }
    
    private void TriggerRepplesEffect(PitchType pitchType)
    {
        Debug.Log("Ripple: Triggered!");
        GameManager.em.TriggerRipples(transform, (ColorType)(int)pitchType, transform.localScale);
    }
}