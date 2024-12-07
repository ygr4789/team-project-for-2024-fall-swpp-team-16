using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResonatableObject : MonoBehaviour
{
    [HideInInspector] public PitchType[] properties = {};
    private bool isPlayingRipples = false;
    private int colliderNum = 0;
    public Vector3 ripplesPositionOffset = Vector3.zero;

    [HideInInspector] public delegate void Resonate(PitchType pitch);
    [HideInInspector] public Resonate resonate;

    public void OnEnterRadius()
    {
        colliderNum++;
        if (!isPlayingRipples && colliderNum==1)
        {
            isPlayingRipples = true;
            foreach (PitchType pitch in properties)
            {
                TriggerRipplesEffect(pitch);
            }
        }
    }

    public void OnExitRadius()
    {
        colliderNum--;
        if (colliderNum <= 0)
        {
            colliderNum = 0;
            isPlayingRipples = false;
            GameManager.em.StopRipples(transform);
        }
    }
    
    private void TriggerRipplesEffect(PitchType pitchType)
    {
        Color color = GameParameters.PitchColors[(int)pitchType];
        GameManager.em.TriggerRipples(transform, color, transform.localScale, ripplesPositionOffset);
    }
}