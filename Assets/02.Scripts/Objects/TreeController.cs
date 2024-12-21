using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(HingeJoint))]
[RequireComponent(typeof(CapsuleCollider))]
public class TreeController : Interactable
{
    [SerializeField] private int durability = 1;
    [SerializeField] private GameObject treePrefab;
    
    [SerializeField, Range(3f, 15f)] private float prefabHeight = 13.5f;
    [SerializeField, Range(3f, 15f)] private float minHeight = 3f;
    [SerializeField, Range(3f, 15f)] private float maxHeight = 13f;
    [SerializeField, Range(3f, 15f)] public float currentHeight = 8f;
    [SerializeField, Range(0f, 1f)] private float radius = 0.7f;

    private const float HeightChangeSmoothTime = 0.6f;
    private const float HeightChangeSpeed = 1f;
    private Vector3 _currentVelocity = Vector3.zero;
    
    private HingeJoint _hinge;
    private CapsuleCollider _capsuleCollider;
    
    private GameObject _movingSound;

    private void OnValidate()
    {
        Assert.IsNotNull(treePrefab, "TreePrefab cannot be null.");
        Assert.IsTrue(treePrefab.TryGetComponent<MeshFilter>(out _), "TreePrefab must have MeshFilter component.");
        maxHeight = Mathf.Clamp(maxHeight, 0f, prefabHeight);
        minHeight = Mathf.Clamp(minHeight, 0f, maxHeight);
        currentHeight = Mathf.Clamp(currentHeight, minHeight, maxHeight);
        treePrefab.transform.localPosition = new Vector3(0f, currentHeight - prefabHeight, 0f);
        treePrefab.transform.localScale = Vector3.one;
        _capsuleCollider = gameObject.GetComponent<CapsuleCollider>();
        _capsuleCollider.height = 2 * currentHeight;
        _capsuleCollider.radius = radius;
    }

    private void Awake()
    {
        _hinge = gameObject.GetComponent<HingeJoint>();
        _capsuleCollider = gameObject.GetComponent<CapsuleCollider>();
        _capsuleCollider.height = 2 * currentHeight;
        _capsuleCollider.radius = radius;
        var resonatable = gameObject.AddComponent<ResonatableObject>();
        resonatable.properties = new[] { PitchType.So, PitchType.La };
        resonatable.resonate += TreeResonate;
        resonatable.ripplesPositionOffset = Vector3.up * 0.5f;
    }

    private void TreeResonate(PitchType pitch)
    {
        switch (pitch)
        {
            case PitchType.So: { SmoothIncreaseHeight(); break; }
            case PitchType.La: { SmoothDecreaseHeight(); break; }
        }
    }

    private void Update()
    {
        SetPrefabPosition();
        HandleSound();
    }
    
    private void SetPrefabPosition()
    {
        Vector3 currentPosition = treePrefab.transform.localPosition;
        Vector3 targetPosition = new Vector3(0f, currentHeight - prefabHeight, 0f);
        _capsuleCollider.height = 2 * currentHeight;
        treePrefab.transform.localPosition = Vector3.SmoothDamp(currentPosition, targetPosition, ref _currentVelocity, HeightChangeSmoothTime);
    }

    private void SmoothIncreaseHeight()
    {
        currentHeight += HeightChangeSpeed * Time.deltaTime;
        currentHeight = Mathf.Clamp(currentHeight, minHeight, maxHeight);
    }

    private void SmoothDecreaseHeight()
    {
        currentHeight -= HeightChangeSpeed * Time.deltaTime;
        currentHeight = Mathf.Clamp(currentHeight, minHeight, maxHeight);
    }
    
    private void HandleSound()
    {
        switch (_currentVelocity.y)
        {
            case < -0.1f:
                PlayTreeSound("decrease");
                break;
            case > 0.1f:
                PlayTreeSound("increase");
                break;
            default:
                StopTreeSound();
                break;
        }
    }
    
    private void PlayTreeSound(string action)
    {
        if (_movingSound) return;
        _movingSound = GameManager.sm.PlayLoopSound("tree-" + action);
        if (_movingSound.TryGetComponent<AudioSource>(out var source))
        {
            source.volume *= 1f; // Reduce volume by half
        }
    }
    
    private void StopTreeSound()
    {
        if (_movingSound) Destroy(_movingSound);
    }
    
    public void Damage(Transform damager)
    {
        if (--durability > 0) return;
        var direction = transform.position - damager.position;
        direction.y = 0f;
        direction.Normalize();
        Collapse(direction);
    }

    private void Collapse(Vector3 direction)
    {
        const float initialAngularVelocity = 1f;
        var cutOffset = radius + 0.1f;
        
        var cutPoint = transform.position + Vector3.up * cutOffset;
        var cutTree = Cutter.Cut(treePrefab, cutPoint, Vector3.up);
        var cutCollider = cutTree.AddComponent<CapsuleCollider>();

        var remainHeight = cutPoint.y - treePrefab.transform.position.y;
        var cutHeight = prefabHeight - remainHeight;
        cutCollider.center = new Vector3(0f, (prefabHeight + remainHeight) / 2f, 0f);
        cutCollider.height = cutHeight;
        cutCollider.radius = radius;
        
        cutTree.AddComponent<PhysicsImmunity>();
        
        var cutRigidbody = cutTree.AddComponent<Rigidbody>();
        var axis = Vector3.Cross(Vector3.up, direction).normalized;
        var torque = axis * initialAngularVelocity;
        
        cutRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        cutRigidbody.centerOfMass = Vector3.up * prefabHeight;
        
        _hinge.connectedBody = cutRigidbody;
        _hinge.anchor = Vector3.up * cutOffset;
        _hinge.axis = axis;
        
        cutRigidbody.AddTorque(torque, ForceMode.VelocityChange);
        cutRigidbody.mass = 100;
        
        _capsuleCollider.enabled = false;
        GameManager.em.StopRipples(transform);
        GameManager.pm.UnregisterTarget(transform);
        PlayCollapseSound();
        
        this.enabled = false;
        Destroy(gameObject.GetComponent<ResonatableObject>());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
        Gizmos.DrawCube(transform.position, new Vector3(3f, 0.01f, 3f));
        Gizmos.color = new Color(0f, 0f, 1f, 0.2f);
        Gizmos.DrawCube(transform.position + minHeight * Vector3.up, new Vector3(3f, 0.01f, 3f));
        Gizmos.DrawCube(transform.position + maxHeight * Vector3.up, new Vector3(3f, 0.01f, 3f));
    }
    
    private void PlayCollapseSound()
    {
        GameManager.sm.PlaySound("collapse", 2f);
    }
}

