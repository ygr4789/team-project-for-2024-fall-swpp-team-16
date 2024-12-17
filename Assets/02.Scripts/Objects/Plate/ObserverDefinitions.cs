public class ObserverWaterIncrease : IPlateObserver
{
    private readonly WaterController _waterController;
    private readonly float _waterIncrease;

    public ObserverWaterIncrease(WaterController waterController, float waterIncrease = 1f)
    {
        _waterController = waterController;
        _waterIncrease = waterIncrease;
    }
    
    public void OnPlateStateChanged(bool isPressed)
    {
        if (isPressed)
        {
            _waterController.AdjustWaterLevel(_waterIncrease);
        }
        else
        {
            _waterController.AdjustWaterLevel(-_waterIncrease);
        }
    }
}

public class ObserverDestroyObstacle : IPlateObserver
{
    private readonly BreakableObject _breakableObject;

    public ObserverDestroyObstacle(BreakableObject breakableObject)
    {
        _breakableObject = breakableObject;
    }
    
    public void OnPlateStateChanged(bool isPressed)
    {
        if (isPressed)
        {
            if (_breakableObject) _breakableObject.Damage();
        }
    }
}