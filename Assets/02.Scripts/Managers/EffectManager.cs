using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem effectPrefab;
    private ParticleSystem activeEffect;
    
    private List<Color> activeColors = new List<Color>();
    private float colorSwitchInterval = 0.5f;  // 색상 전환 간격 (초)
    private float colorSwitchTimer;
    
    private float defaultSize = 4;

    public void TriggerRipples(Vector3 position, Transform target, ColorType colorType, Vector3 targetScale)
    {
        Color color = GameManager.colors[(int)colorType];
        if (!activeColors.Contains(color))
            activeColors.Add(color);

        if (activeEffect == null)
        {
            activeEffect = Instantiate(effectPrefab, position, Quaternion.identity);
            activeEffect.transform.SetParent(target);
        }

        activeEffect.transform.position = position;
        float maxScale = Mathf.Max(targetScale.x, targetScale.y, targetScale.z);
        var mainModule = activeEffect.main;
        mainModule.startSize = maxScale*defaultSize;

        if (!activeEffect.isPlaying)
            activeEffect.Play();
    }

    public void RemoveColorFromRipples(ColorType colorType)
    {
        Color color = GameManager.colors[(int)colorType];
        activeColors.Remove(color);  // 특정 색상을 리스트에서 제거
    }

    public void StopRipples()
    {
        if (activeEffect != null && activeEffect.isPlaying)
        {
            activeEffect.Stop();
        }
        activeColors.Clear();  // 키가 떼어지면 색상 목록 초기화
    }
    
    private void Update()
    {
        if (activeColors.Count > 0)
        {
            colorSwitchTimer += Time.deltaTime;
            if (colorSwitchTimer >= colorSwitchInterval)
            {
                colorSwitchTimer = 0f;
                
                var mainModule = activeEffect.main;
                mainModule.startColor = activeColors[0];
                
                Color firstColor = activeColors[0];
                activeColors.RemoveAt(0);
                activeColors.Add(firstColor);
            }
        }
    }

}
