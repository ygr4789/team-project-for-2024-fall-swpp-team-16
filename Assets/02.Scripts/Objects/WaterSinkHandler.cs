using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaterSinkHandler
{
    private readonly WaterController _waterController;
    private readonly List<Collider> _sunkColliders;

    public WaterSinkHandler(WaterController waterController)
    {
        _waterController = waterController;
        _sunkColliders = new List<Collider>();
    }

    public void AddSinkingTarget(Transform transform)
    {
        if (!transform.TryGetComponent<Collider>(out var collider)) return;
        if (!_sunkColliders.Contains(collider)) _sunkColliders.Add(collider);
    }

    public void RemoveSinkingTarget(Transform transform)
    {
        if (!transform.TryGetComponent<Collider>(out var collider)) return;
        _sunkColliders.Remove(collider);
    }

    public void Handle()
    {
        _waterController.currentWaterLevel = CalculateWaterLevel(_waterController.currentWaterLevel);
    }

    private float CalculateWaterLevel(float currentWaterLevel)
    {
        var waterBounds = _waterController.Bounds;
        
        // Calculate the cross-sectional area of the cup
        var waterArea = (waterBounds.max.x - waterBounds.min.x) * (waterBounds.max.z - waterBounds.min.z) * _waterController.ExposedSurfaceRatio;

        // Calculate the displaced volume
        var sunkVolume = (
            from sunkCollider in _sunkColliders
            let sunkBounds = sunkCollider.bounds
            let submergedMinY = Mathf.Max(sunkBounds.min.y, waterBounds.min.y)
            let submergedMaxY = Mathf.Min(sunkBounds.max.y, waterBounds.min.y + currentWaterLevel)
            where submergedMaxY > submergedMinY 
            select (sunkBounds.max.x - sunkBounds.min.x) * (sunkBounds.max.z - sunkBounds.min.z) * (submergedMaxY - submergedMinY)
            ).Sum();

        // Calculate the change in water height
        var deltaHeight = sunkVolume / waterArea;
        var updatedWaterLevel = _waterController.InitialWaterLevel + deltaHeight;
        return updatedWaterLevel;
    }
}