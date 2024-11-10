using UnityEngine;

public class RockController : MonoBehaviour
{
    public float maxSpeed = 10f;
    public float defaultAcceleration = 10f;
    public float defaultDeceleration = 5f;
    private Vector3 _currentVelocity = Vector3.zero;

    private const string PlayerTag = "Player"; // Tag of the player to follow
    public bool followPlayer; // Flag to start following the player
    public bool isRolling; // Flag to check if the rock is rolling
    private Vector3 _offset; // Relative position offset to the player
    private GameObject _player; // Reference to the player object
    public float hoverHeight = 1.0f; // Height above ground to maintain
    public LayerMask groundLayer; // Layer mask to specify ground layer

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag(PlayerTag);
        StickToGround();
    }

    // Update is called once per frame
    void Update()
    { 
        // for test
        if (Input.GetKey(KeyCode.L))
            Accelerate(Vector3.left, defaultAcceleration);
        if (Input.GetKey(KeyCode.F))
            AccelerateToPlayer(defaultAcceleration);
        if (Input.GetKey(KeyCode.G))
            AccelerateAwayFromPlayer(defaultAcceleration);
            
            
        _currentVelocity -= _currentVelocity.normalized * (defaultDeceleration * Time.deltaTime); // default deceleration
        _currentVelocity = Vector3.ClampMagnitude(_currentVelocity, maxSpeed); // clamp speed

        if (_currentVelocity.magnitude > 0f) // rock moving motion
        {
            RollRock(_currentVelocity); // update rock's position and rotation
        }

        if (followPlayer && _player) // follow player *not yet implemented
        {
            _offset = transform.position - _player.transform.position;
            FollowPlayer();
            Float();
        }

        StickToGround();
    }

    private void Accelerate(Vector3 direction, float acceleration)
    {
        _currentVelocity += direction.normalized * (acceleration * Time.deltaTime);
        _currentVelocity = Vector3.ClampMagnitude(_currentVelocity, maxSpeed); // Ensure speed stays within limits
    }

    private void RollRock(Vector3 velocity)
    {
        // Move and rotate the rock
        Vector3 direction = velocity.normalized;
        transform.position += velocity * Time.deltaTime;
        if (isRolling)
            transform.Rotate(Vector3.Cross(Vector3.up, direction), velocity.magnitude * Time.deltaTime * 36, Space.World);
    }

    private void Float()
    {
        float floatSpeed = 2f;
        float floatHeight = Mathf.Sin(Time.time * floatSpeed) * 0.5f + 2f;
        transform.position = new Vector3(transform.position.x, _player.transform.position.y + floatHeight, transform.position.z);
    }

    private void FollowPlayer()
    {
        transform.position = _player.transform.position + _offset;
    }

    private void StickToGround()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            float groundHeight = hit.point.y;
            if (!Mathf.Approximately(transform.position.y, groundHeight + hoverHeight))
            {
                transform.position = new Vector3(transform.position.x, groundHeight + hoverHeight, transform.position.z);
            }
        }
    }

    public void AccelerateToPlayer(float acceleration = 10f)
    {
        if (_player is null) return;
        var directionToPlayer = (_player.transform.position - transform.position).normalized;
        Accelerate(directionToPlayer, acceleration);
    }

    public void AccelerateAwayFromPlayer(float acceleration = 10f)
    {
        if (_player is null) return;
        var directionAwayFromPlayer = (transform.position - _player.transform.position).normalized;
        Accelerate(directionAwayFromPlayer, acceleration);
    }
}
