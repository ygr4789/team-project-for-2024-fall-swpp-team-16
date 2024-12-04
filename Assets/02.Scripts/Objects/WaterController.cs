using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Assertions;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(BoxCollider))]
public class WaterController : Interactable
{
    [SerializeField, Range(0f, 10f)] private float minWaterLevel = 0f; // Minimum height of water
    [SerializeField, Range(0f, 10f)] private float maxWaterLevel = 1f; // Maximum height of water
    [Range(0f, 10f)] public float initialWaterLevel = 0.5f; // Original height of water
    
    [SerializeField, Range(0f, 10f)] private float waterSurfaceSizeX = 1f; // Size of water surface
    [SerializeField, Range(0f, 10f)] private float waterSurfaceSizeY = 1f; // Size of water surface
    [SerializeField, Range(0f, 3f)] private float timeToReachTarget = 10f; // 목표에 도달할 시간
    [Tooltip("Water Surface Object (Must be child of this GameObject)")]
    [SerializeField] private Transform waterSurface;
    
    [HideInInspector] public float currentWaterLevel;
    [HideInInspector] public BoxCollider waterCollider;

    private PlayerMovement playerMovement;
    // private WaterSinkHandler waterSinkHandler;

    private void OnValidate()
    {
        Assert.IsNotNull(waterSurface, "Water Surface Object must be assigned");
        Assert.IsTrue(waterSurface.IsChildOf(transform), "Water Surface Object must be child of this GameObject");
        Assert.IsNotNull(waterSurface.GetComponent<Renderer>(), @"Water Surface must have renderer component");
        minWaterLevel = Mathf.Clamp(minWaterLevel, 0f, maxWaterLevel);
        initialWaterLevel = Mathf.Clamp(initialWaterLevel, minWaterLevel, maxWaterLevel);
        Init();
    }

    private void Awake()
    {
        Init();
    }
    
    private void Init()
    {
        waterCollider = gameObject.GetComponent<BoxCollider>();
        // waterSinkHandler = new WaterSinkHandler(this);
        currentWaterLevel = initialWaterLevel;
        SetWaterSurfacePosition();
        SetWaterSurfaceSize();
        SetWaterCollider();
    }

    private void Start()
    {
        playerMovement = GameManager.gm.controller.GetComponent<PlayerMovement>();
    }

    void Update()
    {
        CheckPlayerCollision();
        SetWaterSurfacePosition();
        // waterSinkHandler.Handle();
    }

    private void CheckPlayerCollision()
    {
        var playerPosition = GameManager.gm.controller.position;
        var sinkHeight = playerPosition.y - waterSurface.position.y;
        var waterLayer = 1 << LayerMask.NameToLayer("Water");
        Collider[] results = {};
        var size = Physics.OverlapSphereNonAlloc(playerPosition, 3f, results, waterLayer);
        if (Array.IndexOf(results, waterCollider) > -1 && sinkHeight < -0.5f)
        {
            playerMovement.Drown();
        }
    }

    private void SetWaterSurfacePosition()
    {
        var waterSurfacePosition = Vector3.zero;
        waterSurfacePosition.y = currentWaterLevel;
        waterSurface.localPosition = waterSurfacePosition;
    }

    private void SetWaterSurfaceSize()
    {
        var waterSurfaceSize = Vector3.one;
        waterSurfaceSize.x = waterSurfaceSizeX;
        waterSurfaceSize.y = waterSurfaceSizeY;
        waterSurface.localScale = waterSurfaceSize;
    }
    
    private void SetWaterCollider()
    {
        var bounds = waterSurface.GetComponent<Renderer>().bounds;
        var center = Vector3.up * (maxWaterLevel + minWaterLevel) / 2;
        var size = new Vector3(bounds.size.x, maxWaterLevel - minWaterLevel, bounds.size.z);
        waterCollider.center = center;
        waterCollider.size = size;
        waterCollider.isTrigger = true;
    }

    private void OnDrawGizmos()
    {
        var boxCollider = GetComponent<BoxCollider>();
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + boxCollider.center , boxCollider.size);
    }
}

