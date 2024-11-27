using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class FlotageController : MonoBehaviour
{
    private const string PlayerTag = "Player"; // Tag of the player to follow
    
    [Range(0f, 10f)]
    [Tooltip("movement speed at resonance")]
    public float moveSpeed = 3f;
    
    private GameObject _player; // Reference to the player object
    private Rigidbody _rigidbody; // Rigidbody component
    
    private void Awake()
    {
        ResonatableObject resonatable = gameObject.AddComponent<ResonatableObject>();
        resonatable.properties = new[] { PitchType.Mi, PitchType.Fa };
        resonatable.resonate += FlotageResonate;
        _rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    private void FlotageResonate(PitchType pitch)
    {
        switch (pitch)
        {
            case PitchType.Mi: { MoveToPlayer(); break; }
            case PitchType.Fa: { MoveAwayFromPlayer(); break; }
        }
    }

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag(PlayerTag);
    }

    void Update()
    {
        FlattenPosition();
        if (_rigidbody.velocity == Vector3.zero) _rigidbody.isKinematic = true;
    }

    // Anchor to the water surface
    private void FlattenPosition()
    {
        Vector3 flattenPosition = transform.localPosition;
        flattenPosition.y = 0f;
        transform.localPosition = flattenPosition;
    }
    
    private void MoveToPlayer()
    {
        if (_player is null) return;
        _rigidbody.isKinematic = false;
        var directionToPlayer = _player.transform.position - transform.position;
        directionToPlayer.y = 0;
        directionToPlayer.Normalize();
        _rigidbody.velocity = directionToPlayer * moveSpeed;
    }

    private void MoveAwayFromPlayer()
    {
        if (_player is null) return;
        _rigidbody.isKinematic = false;
        var directionAwayFromPlayer = transform.position - _player.transform.position;
        directionAwayFromPlayer.y = 0;
        directionAwayFromPlayer.Normalize();
        _rigidbody.velocity = directionAwayFromPlayer * moveSpeed;
    }
}

