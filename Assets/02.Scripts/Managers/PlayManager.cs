using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayManager : MonoBehaviour
{
    public Transform playerTransform;

    // 관리할 파티클 효과와 색상 목록을 딕셔너리로 유지
    public Dictionary<Transform, ParticleSystem> activeRipplesEffects = new Dictionary<Transform, ParticleSystem>();
    public Dictionary<Transform, List<Color>> activeRipplesColors = new Dictionary<Transform, List<Color>>();
    public Transform currentTarget;

    private float defaultSize = 6;
    private float targetScaleMultiplier = 1.2f;

    private void Start()
    {
        currentTarget = null;
        Debug.Log("PlayManager Start");
    }

    private void Update()
    {
        if (playerTransform is null)
        {
            // Debug.Log("Player Transform is null");
            // playerTransform = FindObjectOfType<PlayerMovement>().transform;
        } else {
            // Debug.Log("Player Transform is not null");
        }
        // HandleTargetSwitch();
        // HandleResonance();
    }
    
    private void HandleTargetSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            List<Transform> targets = new List<Transform>(activeRipplesEffects.Keys);
            targets.Remove(playerTransform);

            if (targets.Count == 0) return;

            int currentIndex = currentTarget is not null ? targets.IndexOf(currentTarget) : -1;
            int nextIndex = (currentIndex + 1) % targets.Count;

            SetCurrentTarget(targets[nextIndex]);
        }
    }
    private void HandleResonance()
    {
        if (currentTarget is null) return;
        ResonatableObject resonatable = currentTarget.GetComponent<ResonatableObject>();
        if (resonatable is null) return;
        foreach (PitchType pitch in Enum.GetValues(typeof(PitchType)))
        {
            KeyCode key = GameParameters.PitchKeys[(int)pitch];
            if (Input.GetKey(key)) resonatable.resonate(pitch);
        }
    }

    private void SetCurrentTarget(Transform newTarget)
    {
        if (currentTarget == newTarget) return;
        if (currentTarget is not null && activeRipplesEffects.ContainsKey(currentTarget))
        {
            Debug.Log($"[Return Size] Target: {currentTarget.name}");
            GameManager.em.SetRippleSize(currentTarget, positionOffset: currentTarget.GetComponent<ResonatableObject>().ripplesPositionOffset);
        }

        currentTarget = newTarget;

        if (currentTarget is not null && activeRipplesEffects.ContainsKey(currentTarget))
        {
            GameManager.em.SetRippleSize(currentTarget, 4.5f, positionOffset:currentTarget.GetComponent<ResonatableObject>().ripplesPositionOffset);
        }
    }
    
    
    // 타겟을 등록하는 메서드
    public void RegisterTarget(Transform target, ParticleSystem effect)
    {
        if (!activeRipplesEffects.ContainsKey(target))
        {
            activeRipplesEffects[target] = effect;
            activeRipplesColors[target] = new List<Color>();
        }
    }

    // 타겟을 제거하는 메서드
    public void UnregisterTarget(Transform target)
    {
        if (activeRipplesEffects.ContainsKey(target))
        {
            activeRipplesEffects.Remove(target);
            activeRipplesColors.Remove(target);
        }
        
        // 현재 타겟과 동일한 경우 초기화
        if (currentTarget == target)
        {
            currentTarget = null;
        }
    }
}