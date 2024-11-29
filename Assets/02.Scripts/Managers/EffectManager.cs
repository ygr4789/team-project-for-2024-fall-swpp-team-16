using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [SerializeField] public ParticleSystem ripplesEffectPrefab;
    [SerializeField] private float colorSwitchInterval = 0.5f;
    private float colorSwitchTimer;
    private float defaultSize = 7;
    
    
    public void TriggerRipples(Transform target, Color color, Vector3 targetScale, float heightRatio = 0.5f, bool isPlayer = false)
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

        if (isPlayer)
        {
            var mainModule = activeEffect.main;
            mainModule.startSize = Mathf.Max(targetScale.x, targetScale.y, targetScale.z) * defaultSize;
        }
        else { SetRippleSize(target); }
        
        if (!isPlayer || !activeEffect.isPlaying)
        {
            activeEffect.Play();
        }
    }
    
    public void SetRippleSize(Transform target, float multiplier = 0.5f, float minParticleSize = 2.5f, float maxParitlceSixe = 3.5f)
    {
        if (!GameManager.pm.activeRipplesEffects.ContainsKey(target)) return;

        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            // 2D 기준 크기 계산: X, Y 축만 사용
            Bounds totalBounds = new Bounds(target.position, Vector3.zero);
            foreach (Renderer renderer in renderers)
            {
                totalBounds.Encapsulate(renderer.bounds);
            }

            float width = totalBounds.size.x;
            float height = totalBounds.size.y;

            // 2D 평균 크기 계산 (Bounding Box X와 Y만 사용)
            float averageDimension = (width + height) / 2;
            
            Vector3 localScale = target.localScale;
            float scaleFactor = Mathf.Max(localScale.x, localScale.y, localScale.z); // 가장 큰 축의 스케일 선택
            float scaledSize = averageDimension * scaleFactor;

            // 크기 제한 및 설정
            float particleSize = Mathf.Clamp(scaledSize * multiplier, minParticleSize, maxParitlceSixe); // 비율 조정 및 최소/최대 크기 제한
            Debug.Log($"[SetRippleSize] Target: {target.name}, ParticleSize: {particleSize}");

            ParticleSystem effect = GameManager.pm.activeRipplesEffects[target];
            var mainModule = effect.main;
            mainModule.startSize = particleSize; // 최종 크기 적용
        }
    }


    public void UpdateRipplePosition(Transform target, Vector3 newPosition)
    {
        if (GameManager.pm.activeRipplesEffects.ContainsKey(target))
        {
            ParticleSystem effect = GameManager.pm.activeRipplesEffects[target];
            effect.transform.position = newPosition;
        }
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
        yield return new WaitForSeconds(effect.main.duration/2); // duration만큼 대기
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
