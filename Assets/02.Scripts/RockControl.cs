using UnityEngine;

public class RockControl : MonoBehaviour
{
    public float maxSpeed = 10f;
    public float acceleration = 5f;
    public float deceleration = 5f;
    private float _currentSpeed = 0f;

    public GameObject player; // GameObject of the player to follow
    public bool followPlayer; // Flag to start following the player
    private Vector3 _offset; // Relative position offset to the player

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space)) // accelerate
        {
            _currentSpeed += acceleration * Time.deltaTime;
        }
        else // decelerate
        {
            _currentSpeed -= deceleration * Time.deltaTime;
        }

        _currentSpeed = Mathf.Clamp(_currentSpeed, 0f, maxSpeed); // clamp speed

        if (_currentSpeed > 0f) // roll rock
        {
            RollRock(Vector3.left, _currentSpeed); // 방향은 추후에 변수로 변경
        }

        if (followPlayer && player) // follow player
        {
            _offset = transform.position - player.transform.position;
            FollowPlayer();
            Float();
        }
    }

    private void RollRock(Vector3 direction, float speed)
    {
        direction = direction.normalized;
        transform.position += direction * (speed * Time.deltaTime);
        transform.Rotate(Vector3.Cross(Vector3.up, direction), speed * Time.deltaTime * 36, Space.World);
    }
    
    private void FollowPlayer()
    {
        transform.position = player.transform.position + _offset;
    }

    private void Float()
    {
        float floatSpeed = 2f;
        float floatHeight = Mathf.Sin(Time.time * floatSpeed) * 0.5f + 2f;
        transform.position = new Vector3(transform.position.x, player.transform.position.y + floatHeight, transform.position.z);
    }
}
