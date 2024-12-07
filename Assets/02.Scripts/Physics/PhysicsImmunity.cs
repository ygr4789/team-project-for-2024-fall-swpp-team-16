using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PhysicsImmunity : MonoBehaviour
{
    private const float PaddingSize = 0.05f;
    
    private void Awake()
    {
        var padding = new GameObject
        {
            transform =
            {
                parent = transform,
                localPosition = Vector3.zero,
                localRotation = Quaternion.identity,
                localScale = Vector3.one * (1f + PaddingSize)
            }
        };
        
        var originalCollider = GetComponent<Collider>();
        var shieldCollider = CopyComponent(originalCollider, padding);
        Physics.IgnoreCollision(originalCollider, shieldCollider);
        
        var shieldRigidbody = padding.AddComponent<Rigidbody>();
        shieldRigidbody.isKinematic = true;
        shieldRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        
        padding.name = "Padding";
        padding.tag = "Padding";
        padding.layer = LayerMask.NameToLayer("Ignore Raycast");
    }
    
    private static T CopyComponent<T>(T original, GameObject destination) where T : Component
    {
        var type = original.GetType();
        var copy = destination.AddComponent(type);
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.GetProperty);
        foreach (var property in properties)
        {
            if (!property.CanWrite) continue;
            property.SetValue(copy, property.GetValue(original));
        }
        return copy as T;
    }
}
