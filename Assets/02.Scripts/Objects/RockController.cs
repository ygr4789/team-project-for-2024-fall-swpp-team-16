using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

public class RockController : Interactable
{
    [Tooltip("Flag to check if the rock is rolling")]
    [SerializeField] 
    public bool isRolling;
    
    [Tooltip("The distance at which the object stops approaching the player")]
    [Range(0f, 0.1f)]
    [SerializeField] private float approachDistance = 3f;
    
    [Range(0f, 10f)] public float moveSpeed = 3f;
    
    [SerializeField] private Transform rockModel;
    
    private const string PlayerTag = "Player"; // Tag of the player to follow
    private GameObject _player; // Reference to the player object

    private const float SpeedThreshold = 1.0f; // Speed threshold for playing sound
    private float _radius; // Radius of round rock (only used when isRolling=true)
    private bool _isPlayingSound = false;
    private Vector3 _currentVelocity = Vector3.zero;
    private Vector3 _targetPosition;
    private SurfaceContactController _rockBody;
    private GameObject _movingSound;
    
    private void Awake()
    {
        gameObject.AddComponent<InteractionHandler>();
        gameObject.AddComponent<PhysicsImmunity>();
        _rockBody = GetComponent<SurfaceContactController>();
        Assert.IsNotNull(_rockBody);
        
        SphereCollider rockCollider = GetComponent<SphereCollider>();
        if (isRolling)
        {
            Assert.IsNotNull(rockCollider);
            _radius = rockCollider.radius;
        }
        
        Assert.IsNotNull(rockModel);
        
        ResonatableObject resonatable = gameObject.AddComponent<ResonatableObject>();
        resonatable.properties = new[] { PitchType.Mi, PitchType.Fa };
        resonatable.resonate += RockResonate;
    }

    private void RockResonate(PitchType pitch)
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
        Assert.IsNotNull(_player);
    }

    // Update is called once per frame
    void Update()
    {
        ApplyVelocity();
        if (isRolling) RollRock(_rockBody.Velocity);
        HandleSound();
    }

    private void ApplyVelocity()
    {
        _rockBody.Velocity = _currentVelocity;
        _currentVelocity = Vector3.zero;
    }

    private void RollRock(Vector3 velocity)
    {
        // Move and rotate the rock
        Vector3 direction = velocity.normalized;
        float angle = 2f * (velocity.magnitude / _radius) * Time.deltaTime * Mathf.Rad2Deg;
        rockModel.Rotate(Vector3.Cross(Vector3.up, direction), angle, Space.World);
    }

    private void MoveToPlayer()
    {
        // if (Vector3.Distance(_player.transform.position, transform.position) < approachDistance) return;
        var directionToPlayer = (_player.transform.position - transform.position).normalized;
        _currentVelocity = directionToPlayer * moveSpeed;
    }

    private void MoveAwayFromPlayer()
    {
        var directionAwayFromPlayer = (transform.position - _player.transform.position).normalized;
        _currentVelocity = directionAwayFromPlayer * moveSpeed;
    }

    private void HandleSound()
    {
        // Check speed and play/stop sound
        if (_rockBody.Velocity.magnitude > SpeedThreshold && !_isPlayingSound)
        {
            PlayMovingSound();
            _isPlayingSound = true;
        }
        else if (_rockBody.Velocity.magnitude <= SpeedThreshold && _isPlayingSound)
        {
            StopMovingSound();
            _isPlayingSound = false;
        }
    }

    private void PlayMovingSound()
    {
        // Implement sound playing logic here
        _movingSound = GameManager.sm.PlayLoopSound("stone-moving");
        if (_movingSound.TryGetComponent<AudioSource>(out var source))
        {
            source.volume *= 0.3f; // Reduce volume by half
        }
    }

    private void StopMovingSound()
    {
        // Implement sound stopping logic here
        Destroy(_movingSound);
    }
}