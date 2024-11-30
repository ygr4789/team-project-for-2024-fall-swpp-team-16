using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

public class RockController : MonoBehaviour
{
    [Tooltip("Flag to check if the rock is rolling")]
    [SerializeField] 
    public bool isRolling;
    
    [Tooltip("The distance at which the object stops approaching the player")]
    [Range(0f, 0.1f)]
    [SerializeField] 
    private float approachDistance = 3f;
    
    [Range(0f, 10f)]
    public float moveSpeed = 3f;
    
    [SerializeField]
    private Transform rockModel;
    
    private const string PlayerTag = "Player"; // Tag of the player to follow
    private GameObject _player; // Reference to the player object

    private float _radius; // Radius of round rock (only used when isRolling=true)
    private Vector3 _currentVelocity = Vector3.zero;
    private SurfaceContactRigidbody _rockBody;
    
    private void Awake()
    {
        _rockBody = GetComponent<SurfaceContactRigidbody>();
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
        if (Vector3.Distance(_player.transform.position, transform.position) < approachDistance) return;
        var directionToPlayer = (_player.transform.position - transform.position).normalized;
        _currentVelocity = directionToPlayer * moveSpeed;
    }

    private void MoveAwayFromPlayer()
    {
        var directionAwayFromPlayer = (transform.position - _player.transform.position).normalized;
        _currentVelocity = directionAwayFromPlayer * moveSpeed;
    }
}
