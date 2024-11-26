using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlotageController : MonoBehaviour
{
    private const string PlayerTag = "Player"; // Tag of the player to follow
    
    [Range(0f, 10f)]
    public float moveSpeed = 3f;
    
    private GameObject _player; // Reference to the player object
    private Rigidbody _rigidbody;
    
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

    // Update is called once per frame
    void Update()
    {
        FlattenPosition();
    }

    private void FlattenPosition()
    {
        Vector3 flattenPosition = transform.localPosition;
        flattenPosition.y = 0f;
        transform.localPosition = flattenPosition;
    }

    private void MoveToPlayer()
    {
        if (_player is null) return;
        var directionToPlayer = (_player.transform.position - transform.position).normalized;
        directionToPlayer.y = 0;
        directionToPlayer.Normalize();
        var movement = directionToPlayer * (moveSpeed * Time.deltaTime);
        _rigidbody.MovePosition(transform.position + movement);
    }

    private void MoveAwayFromPlayer()
    {
        if (_player is null) return;
        var directionAwayFromPlayer = (transform.position - _player.transform.position).normalized;
        directionAwayFromPlayer.y = 0;
        directionAwayFromPlayer.Normalize();
        var movement = directionAwayFromPlayer * (moveSpeed * Time.deltaTime);
        _rigidbody.MovePosition(transform.position + movement);
    }
    
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log(other.gameObject.name);
    }

    private void OnCollisionExit(Collision other)
    {
        Debug.Log(other.gameObject.name);
    }
}

