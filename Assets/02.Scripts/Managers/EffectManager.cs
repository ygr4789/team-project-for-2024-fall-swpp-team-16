using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem ripplesEffectPrefab;
    [SerializeField] private float colorSwitchInterval = 0.5f;
    private float colorSwitchTimer;
    private float defaultSize = 7;
    private float targetScaleMultiplier = 1.5f;
    
    
    public void TriggerRipples(Transform target, Color color, Vector3 targetScale, float heightRatio = 0.5f)
    {
        if (!GameManager.pm.activeRipplesEffects.ContainsKey(target))
        {
            ParticleSystem newEffect = Instantiate(ripplesEffectPrefab, target.position, Quaternion.identity);
            Bounds totalBounds = new Bounds(target.position, Vector3.zero);
            Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                totalBounds.Encapsulate(renderer.bounds);
            }

            // 높이 비율(heightRatio)을 이용해 y축 위치를 조정합니다.
            float effectYPosition = totalBounds.min.y + totalBounds.size.y * heightRatio;
            newEffect.transform.position = new Vector3(totalBounds.center.x, effectYPosition, totalBounds.center.z);
            newEffect.transform.SetParent(target, true);

            GameManager.pm.RegisterTarget(target, newEffect); // PlayManager에 등록
        }

        if (!GameManager.pm.activeRipplesColors[target].Contains(color))
        {
            GameManager.pm.activeRipplesColors[target].Add(color);
        }

        ParticleSystem activeEffect = GameManager.pm.activeRipplesEffects[target];
        var mainModule = activeEffect.main;
        mainModule.startSize = Mathf.Max(targetScale.x, targetScale.y, targetScale.z) * defaultSize;

        /*if (!activeEffect.isPlaying)
        {*/
        activeEffect.Play();
        //}
    }


    public void RemoveColorFromRipples(Transform target, Color color)
    {
        if (GameManager.pm.activeRipplesColors.ContainsKey(target))
        {
            GameManager.pm.activeRipplesColors[target].Remove(color);
        }
    }

    public void StopRipples(Transform target)
    {
        if (GameManager.pm.activeRipplesEffects.ContainsKey(target))
        {
            ParticleSystem effect = GameManager.pm.activeRipplesEffects[target];
            if (effect.isPlaying)
            {
                effect.Stop();
                GameManager.pm.UnregisterTarget(target); // PlayManager에서 타겟 제거
                StartCoroutine(DestroyAfterDuration(effect, target)); // 파티클 삭제 예약
            }
        }
    }
    
    private IEnumerator DestroyAfterDuration(ParticleSystem effect, Transform target)
    {
        yield return new WaitForSeconds(effect.main.duration); // duration만큼 대기
        Destroy(effect.gameObject); // 파티클 오브젝트 제거
    }


    private void makeRipples()
    {
        colorSwitchTimer += Time.deltaTime;
        
        if (colorSwitchTimer >= colorSwitchInterval)
        {
            colorSwitchTimer = 0f;
            
            foreach (var entry in GameManager.pm.activeRipplesEffects)
            {
                Transform target = entry.Key;
                ParticleSystem effect = entry.Value;

                if (GameManager.pm.activeRipplesColors.ContainsKey(target) && GameManager.pm.activeRipplesColors[target].Count > 0)
                {
                    var mainModule = effect.main;
                    mainModule.startColor = GameManager.pm.activeRipplesColors[target][0];
                    
                    Color firstColor = GameManager.pm.activeRipplesColors[target][0];
                    GameManager.pm.activeRipplesColors[target].RemoveAt(0);
                    GameManager.pm.activeRipplesColors[target].Add(firstColor);
                }
            }
        }
    }

    private void Update()
    {
        makeRipples();
    }
}
