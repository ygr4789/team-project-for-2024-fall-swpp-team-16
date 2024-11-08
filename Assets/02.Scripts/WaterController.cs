using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterController : MonoBehaviour
{
    // Configurations
    public float waterLevelMax = 100.0f; // Maximum height of water
    public float waterLevelMin = 0.0f; // Minimum height of water
    public float waterLevelTargetUnit = 1.0f; // Target height of water
    public float riseSpeed = 0.1f; // Speed at which water rises per second
    
    // Internal variables (But made public for debugging)
    public float initialPositionY;
    public bool isHeightChanging = false;
    public bool isRising = false; // true if height of water is rising, false if height of water is lowering
    public float waterLevelTarget; // Target height of water

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
            if (isRising && transform.position.y >= waterLevelTarget)
            {                    
                isHeightChanging = false;    
            }
            else if (!isRising && transform.position.y <= waterLevelTarget)
            {
                isHeightChanging = false;
            }
            else
            {
                float direction = isRising ? 1 : -1;
                ChangeWaterLevel(direction * riseSpeed * Time.deltaTime);
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
        if (waterLevelTarget > transform.position.y)
        {
            isRising = true;
        }
        else
        {
            isRising = false;
        }
    }
    
    private void ChangeWaterLevel(float amount)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + amount, transform.position.z);
    }
    
    void OnTriggerEnter(Collider other)
    {
        
    }

    void OnTriggerExit(Collider other)
    {
        
    }
}