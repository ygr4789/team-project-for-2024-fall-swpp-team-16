using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(HingeJoint))]
[RequireComponent(typeof(CapsuleCollider))]
public class TreeController : Interactable
{
    [SerializeField] private int durability = 3;
    [SerializeField] private GameObject treePrefab;
    
    [SerializeField, Range(0f, 10f)] private float minHeight = 3f;
    [SerializeField, Range(0f, 10f)] private float maxHeight = 8f;
    [SerializeField, Range(0f, 10f)] private float currentHeight = 5f;
    [SerializeField, Range(0f, 3f)] private float radius = 0.7f;
    
    private readonly float heightChangeSmoothTime = 0.6f;
    private float heightChangeSpeed = 1f;
    private Vector3 currentVelocity = Vector3.zero;
    
    private HingeJoint hinge;
    private bool isCollapsed = false;

    private void OnValidate()
    {
        Assert.IsNotNull(treePrefab, "TreePrefab cannot be null.");
        Assert.IsTrue(treePrefab.TryGetComponent<MeshFilter>(out _), "TreePrefab must have MeshFilter component.");
        minHeight = Mathf.Clamp(minHeight, 0f, maxHeight);
        currentHeight = Mathf.Clamp(currentHeight, minHeight, maxHeight);
        treePrefab.transform.localScale = new Vector3(radius*2, maxHeight, radius*2);
        treePrefab.transform.localPosition = new Vector3(0f, currentHeight - maxHeight, 0f);
        var capsuleCollider = gameObject.GetComponent<CapsuleCollider>();
        capsuleCollider.height = maxHeight;
        capsuleCollider.radius = radius;
    }

    private void Awake()
    {
        hinge = gameObject.GetComponent<HingeJoint>();
        ResonatableObject resonatable = gameObject.AddComponent<ResonatableObject>();
        resonatable.properties = new[] { PitchType.So, PitchType.La };
        resonatable.resonate += TreeResonate;
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
        if (isCollapsed) return;
        Vector3 currentPosition = treePrefab.transform.localPosition;
        Vector3 targetPosition = new Vector3(0f, currentHeight - maxHeight, 0f);
        if (currentPosition != targetPosition)
        {
            treePrefab.transform.localPosition = Vector3.SmoothDamp(currentPosition, targetPosition, ref currentVelocity, heightChangeSmoothTime);
        }
    }

    public void SmoothIncreaseHeight()
    {
        currentHeight += heightChangeSpeed * Time.deltaTime;
        currentHeight = Mathf.Clamp(currentHeight, minHeight, maxHeight);
    }

    public void SmoothDecreaseHeight()
    {
        currentHeight -= heightChangeSpeed * Time.deltaTime;
        currentHeight = Mathf.Clamp(currentHeight, minHeight, maxHeight);
    }
    public void Damage(Transform damager)
    {
        if (isCollapsed) return;
        if (--durability <= 0)
        {
            var direction = transform.position - damager.position;
            direction.y = 0f;
            direction.Normalize();
            Collapse(direction);
        }
    }
    
    public void Collapse(Vector3 direction)
    {
        const float initialAngularVelocity = 1f;
        var cutOffset = radius + 0.1f;
        
        if (isCollapsed) return;
        var cutTree = Cutter.Cut(treePrefab, transform.position + Vector3.up * cutOffset, Vector3.up);
        cutTree.AddComponent<CapsuleCollider>();
        var cutRigidbody = cutTree.AddComponent<Rigidbody>();
        var axis = Vector3.Cross(direction, Vector3.up).normalized;
        var torque = axis * initialAngularVelocity;
        cutRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        cutRigidbody.centerOfMass = Vector3.up * maxHeight;
        hinge.connectedBody = cutRigidbody;
        hinge.anchor = Vector3.up * cutOffset;
        hinge.axis = axis;
        cutRigidbody.AddTorque(torque, ForceMode.VelocityChange);
        cutRigidbody.mass = 100;
        isCollapsed = true;
        this.enabled = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
        Gizmos.DrawCube(transform.position, new Vector3(3f, 0.01f, 3f));
        Gizmos.color = new Color(0f, 0f, 1f, 0.2f);
        Gizmos.DrawCube(transform.position + minHeight * Vector3.up, new Vector3(3f, 0.01f, 3f));
        Gizmos.DrawCube(transform.position + maxHeight * Vector3.up, new Vector3(3f, 0.01f, 3f));
    }
}
