using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterController : MonoBehaviour
{
    // Configurations
    public float waterLevelMax = 100.0f; // Maximum height of water
    public float waterLevelMin = 0.0f; // Minimum height of water
    public float waterLevelTargetUnit = 1.0f; // Target height of water
    public float timeToReachTarget = 2.0f; // 목표에 도달할 시간
    
    // Internal variables (But made public for debugging)
    public float initialPositionY;
    public bool isHeightChanging = false;
    public float waterLevelTarget; // Target height of water
    
    private float elapsedTime = 0.0f;
    private float startHeight; // 현재 높이를 저장할 변수

    void Start()
    {
        initialPositionY = transform.position.y;
        waterLevelTarget = initialPositionY;
        
        // ERASE ME: below three lines of codes are for test
        Invoke("TriggerStepIncreaseWaterLevel", 0.0f);; // increase water level
        Invoke("TriggerStepIncreaseWaterLevel", 4.0f); // trigger increase water level 4 seconds later
        Invoke("TriggerStepDecreaseWaterLevel", 8.0f); // trigger decrease water level 8 seconds later
    }

    void Update()
    {
        if (isHeightChanging)
        {
            // 경과 시간 갱신
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / timeToReachTarget); // 진행률 계산

            // 부드러운 가속/감속을 적용하여 높이 변경
            float smoothStepValue = Mathf.SmoothStep(0, 1, t);
            float newY = Mathf.LerpUnclamped(startHeight, waterLevelTarget, smoothStepValue);

            transform.position = new Vector3(transform.position.x, newY, transform.position.z);

            // 목표에 도달했으면 높이 변경 중지 및 초기화
            if (t >= 1.0f)
            {
                transform.position = new Vector3(transform.position.x, waterLevelTarget, transform.position.z);
                isHeightChanging = false;
                elapsedTime = 0.0f; // 초기화
            }
        }
    }
    
    public void TriggerStepIncreaseWaterLevel()
    {
        waterLevelTarget += waterLevelTargetUnit;
        if (waterLevelTarget > waterLevelMax)
        {
            Debug.Log("Water level is already at the maximum height.");
            return;
        }
        TriggerChangeWaterLevel();
    }
    
    public void TriggerStepDecreaseWaterLevel()
    {
        waterLevelTarget -= waterLevelTargetUnit;
        if (waterLevelTarget < waterLevelMin)
        {
            Debug.Log("Water level is already at the minimum height.");
            return;
        }
        TriggerChangeWaterLevel();
    }
    
    private void TriggerChangeWaterLevel()
    {
        isHeightChanging = true;
        startHeight = transform.position.y; // 현재 높이를 시작 높이로 설정
        elapsedTime = 0.0f; // 새로운 변화가 시작되므로 경과 시간 초기화
    }
}