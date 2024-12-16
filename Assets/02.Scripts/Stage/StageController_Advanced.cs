using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController_Advanced : MonoBehaviour
{
    public PlateController plate;
    public BreakableObject breakable;
    
    private void Awake()
    {
        var observerDestroyObstacle = new ObserverDestroyObstacle(breakable);
        plate.RegisterObserver(observerDestroyObstacle);
    }
}
