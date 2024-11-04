using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterController : MonoBehaviour
{
    public bool isHeightChanging = false;
    public bool isRising = false; // true if height of water is rising, false if height of water is lowering
    
    public float waterLevelTarget = 1.0f; // Target height of water
    public float riseSpeed = 0.1f; // Speed at which water rises per second
    
    private Vector3 initialPosition;

    void Start()
    {
        TriggerIncreaseWaterLevel(49); // ERASE ME: this is for test
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
                IncreaseWaterLevel(direction * riseSpeed * Time.deltaTime);
            }
        }
    }
    
    public void TriggerIncreaseWaterLevel(float _waterLevelTarget)
    {
        isHeightChanging = true;
        waterLevelTarget = _waterLevelTarget;
        if (waterLevelTarget > transform.position.y)
        {
            isRising = true;
        }
        else
        {
            isRising = false;
        }
    }
    
    void IncreaseWaterLevel(float amount)
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