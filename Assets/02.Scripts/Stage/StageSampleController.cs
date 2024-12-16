using System;
using UnityEngine;

public class StageSampleController : MonoBehaviour
{
    public PlateController plate1;
    public PlateController plate2;
    
    public WaterController water;
    public BreakableObject breakable;
    
    private void Awake()
    {
        var observer1 = new ObserverWaterIncrease(water);
        var observer2 = new ObserverDestroyObstacle(breakable);
        
        plate1.RegisterObserver(observer1);
        plate2.RegisterObserver(observer2);
    }
}