using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem ripplesEffectPrefab;
    [SerializeField] private Transform playerTransform;

    // 객체별 파티클 효과와 색상 목록을 관리하는 딕셔너리
    private Dictionary<Transform, ParticleSystem> activeRipplesEffects = new Dictionary<Transform, ParticleSystem>();
    private Dictionary<Transform, List<Color>> activeRipplesColors = new Dictionary<Transform, List<Color>>();
    private Transform currentTarget;
    
    private float colorSwitchInterval = 0.5f;  // 색상 전환 간격 (초)
    private float colorSwitchTimer;
    private float defaultSize = 6;
    private float targetScaleMultiplier = 1.5f;
    
    private void Start()
    {
        currentTarget = null;
    }
    
    public void TriggerRipples(Transform target, ColorType colorType, Vector3 targetScale)
    {
        Color color = GameManager.colors[(int)colorType];
        
        // 파티클 효과가 없으면 새로 생성하고 관리 목록에 추가
        if (!activeRipplesEffects.ContainsKey(target))
        {
            ParticleSystem newEffect = Instantiate(ripplesEffectPrefab, target.position, Quaternion.identity);
            
            // 전체 bounds의 중심을 계산하여 초기 위치 설정
            Bounds totalBounds = new Bounds(target.position, Vector3.zero);
            Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                totalBounds.Encapsulate(renderer.bounds);
            }
            Vector3 initialPosition = totalBounds.center;
            
            newEffect.transform.position = initialPosition;
            newEffect.transform.SetParent(target, true); // true로 설정하여 초기 위치를 고정
            
            activeRipplesEffects[target] = newEffect;
            activeRipplesColors[target] = new List<Color>();
        }

        if (!activeRipplesColors[target].Contains(color))
        {
            activeRipplesColors[target].Add(color);
        }

        ParticleSystem activeEffect = activeRipplesEffects[target];
        float maxScale = Mathf.Max(targetScale.x, targetScale.y, targetScale.z);
        var mainModule = activeEffect.main;
        mainModule.startSize = maxScale * defaultSize;

        if (!activeEffect.isPlaying)
            activeEffect.Play();
    }

    public void RemoveColorFromRipples(Transform target, ColorType colorType)
    {
        if (activeRipplesColors.ContainsKey(target))
        {
            Color color = GameManager.colors[(int)colorType];
            activeRipplesColors[target].Remove(color);
        }
    }

    public void StopRipples(Transform target)
    {
        if (activeRipplesEffects.ContainsKey(target) && activeRipplesEffects[target].isPlaying)
        {
            activeRipplesEffects[target].Stop();
        }

        if (activeRipplesColors.ContainsKey(target))
        {
            activeRipplesColors[target].Clear();  // 키가 떼어지면 색상 목록 초기화
        }
        
        if (target == currentTarget) // 현재 타겟이 제거되면 null로 초기화
        {
            currentTarget = null;
        }
    }

    private void makeRipples()
    {
        colorSwitchTimer += Time.deltaTime;
        
        if (colorSwitchTimer >= colorSwitchInterval)
        {
            colorSwitchTimer = 0f;
            
            // 각 객체별로 색상 전환 처리
            foreach (var entry in activeRipplesEffects)
            {
                Transform target = entry.Key;
                ParticleSystem effect = entry.Value;

                if (activeRipplesColors.ContainsKey(target) && activeRipplesColors[target].Count > 0)
                {
                    var mainModule = effect.main;
                    mainModule.startColor = activeRipplesColors[target][0];
                    
                    // 색상 순서를 변경
                    Color firstColor = activeRipplesColors[target][0];
                    activeRipplesColors[target].RemoveAt(0);
                    activeRipplesColors[target].Add(firstColor);
                }
            }
        }
    }
    
    private void Update()
    {
        makeRipples();
        
        HandleTargetSwitch();
    }

    private void HandleTargetSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            List<Transform> targets = new List<Transform>(activeRipplesEffects.Keys);
            targets.Remove(playerTransform); // 플레이어는 제외

            if (targets.Count == 0) return;

            int currentIndex = currentTarget != null ? targets.IndexOf(currentTarget) : -1;
            int nextIndex = (currentIndex + 1) % targets.Count;

            SetCurrentTarget(targets[nextIndex]);
        }
    }

    private void SetCurrentTarget(Transform newTarget)
    {
        if (currentTarget != null && activeRipplesEffects.ContainsKey(currentTarget))
        {
            // 이전 타겟의 파티클 크기를 원래 크기로 되돌림
            var mainModule = activeRipplesEffects[currentTarget].main;
            mainModule.startSize = defaultSize;
        }

        currentTarget = newTarget;

        if (currentTarget != null && activeRipplesEffects.ContainsKey(currentTarget))
        {
            // 새로운 타겟의 파티클 크기를 1.5배로 설정
            var mainModule = activeRipplesEffects[currentTarget].main;
            mainModule.startSize = defaultSize * targetScaleMultiplier;
        }
    }
    
}
