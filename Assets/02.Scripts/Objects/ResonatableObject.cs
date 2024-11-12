using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResonatableObject : MonoBehaviour
{
    [HideInInspector] public PitchType[] properties = {};
    private bool isPlayingRipples = false;
    private int colliderNum = 0;
    private float ripplesHeightRatio = 0.5f;

    [HideInInspector] public delegate void Resonate(PitchType pitch);
    [HideInInspector] public Resonate resonate;

    public void Start()
    {
        SphereCollider collider = gameObject.AddComponent<SphereCollider>();
        collider.isTrigger = true;
        collider.radius = 0.1f;
    }

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
        Color color = GameParameters.pitchColors[(int)pitchType];
        GameManager.em.TriggerRipples(transform, color, transform.localScale, ripplesHeightRatio);
    }
}