using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaterSinkHandler
{
    private WaterController waterController;
    private readonly List<Collider> sunkColliders;

    public WaterSinkHandler(WaterController waterController)
    {
        this.waterController = waterController;
        sunkColliders = new List<Collider>();
    }

    public void AddSinkingTarget(Transform transform)
    {
        if (!transform.TryGetComponent<RockController>(out _)) return;
        if (!transform.TryGetComponent<Collider>(out var collider)) return;
        if (!sunkColliders.Contains(collider)) sunkColliders.Add(collider);
    }

    public void RemoveSinkingTarget(Transform transform)
    {
        if (!transform.TryGetComponent<RockController>(out _)) return;
        if (!transform.TryGetComponent<Collider>(out var collider)) return;
        sunkColliders.Remove(collider);
    }

    public void Handle()
    {
        Debug.Log(sunkColliders.Count);
        
        waterController.currentWaterLevel = CalculateWaterHeight(
            ConvertToBoundsArray(sunkColliders),
            waterController.waterCollider.bounds,
            waterController.initialWaterLevel,
            waterController.currentWaterLevel);
    }
    
    private static Bounds[] ConvertToBoundsArray(List<Collider> colliders)
    {
        return colliders
            .ConvertAll(collider => collider.bounds)
            .ToArray();
    }

    private static float CalculateWaterHeight(Bounds[] sunkBoundsArray, Bounds waterBounds, float initialWaterHeight, float currentWaterHeight)
    {
        // Calculate the cross-sectional area of the cup
        var waterArea = (waterBounds.max.x - waterBounds.min.x) * (waterBounds.max.y - waterBounds.min.y);

        // Calculate the displaced volume
        var sunkVolume = (
            from sunkBounds in sunkBoundsArray
            let submergedMinZ = Mathf.Max(sunkBounds.min.z, waterBounds.min.z)
            let submergedMaxZ = Mathf.Min(sunkBounds.max.z, currentWaterHeight)
            where submergedMaxZ > submergedMinZ 
            select (sunkBounds.max.x - sunkBounds.min.x) * (sunkBounds.max.y - sunkBounds.min.y) * (submergedMaxZ - submergedMinZ)
            ).Sum();

        // Calculate the change in water height
        var deltaHeight = sunkVolume / waterArea;
        var updatedWaterHeight = initialWaterHeight + deltaHeight;

        // Clamp water height to the water bounds
        updatedWaterHeight = Mathf.Min(updatedWaterHeight, waterBounds.max.z);

        return updatedWaterHeight;
    }
}