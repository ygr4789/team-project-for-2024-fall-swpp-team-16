using System;
using UnityEngine;

namespace _12.Tests.PlayerDev
{
    public class PlayerMovement : MonoBehaviour
    {
        [Range(1f, 20f)]
        [SerializeField] private float _movementSpeed;
        
        [Tooltip("run multiplier of the movement speed")]
        [Range(1f, 20f)]
        [SerializeField] private float _runMultiplier;
        
        [SerializeField] private float _gravity = -9.81f;
        [Range(0f, 3f)]
        [SerializeField] private float _jumpHeight;
        
        [Tooltip("time taken to rotate when the direction of movement is changed")]
        [Range(0f, 0.1f)]
        [SerializeField] private float _smoothTime = 0.1f;

        private CharacterController characterController;
        private Transform playerTransform;
        private Animator playerAnimator;
        private Vector3 _controllerVelocity;

        // Start is called before the first frame update
        private void Start()
        {
            characterController = GetComponent<CharacterController>();
            playerTransform = transform.Find("Player");
            playerAnimator = playerTransform.GetComponent<Animator>();
        }

        // Update is called once per frame
        private void Update()
        {
            // stops the y velocity when player is on the ground and the velocity has reached 0
            if (characterController.isGrounded && _controllerVelocity.y < 0)
            {
                _controllerVelocity.y = 0;
            }

            // get the movement input
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");

            // moves the controller in the desired direction on the x- and z-axis
            Vector3 movement = transform.right * moveX + transform.forward * moveZ;
            movement = PassableComponent(movement);
            characterController.Move(movement * (_movementSpeed * Time.deltaTime));

            // set player's forward same as moving direction
            float currentVelocity = movement.magnitude;
            playerAnimator.SetBool("Moving", currentVelocity > 0);
            if (currentVelocity > 0)
            {
                float targetAngle = Mathf.Atan2(moveX, moveZ) * Mathf.Rad2Deg - 90;
                float angle = Mathf.SmoothDampAngle(playerTransform.eulerAngles.y, targetAngle, ref currentVelocity, _smoothTime);
                playerTransform.rotation = Quaternion.Euler(0, angle, 0);
            }
                
            // gravity affects the controller on the y-axis
            _controllerVelocity.y += _gravity * Time.deltaTime;

            // moves the controller on the y-axis
            characterController.Move(_controllerVelocity * Time.deltaTime);
            

            // the controller is able to jump when on the ground
            if (Input.GetButton("Jump") && characterController.isGrounded)
            {
                _controllerVelocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
            }

            // the controller is able to run
            if (Input.GetKey(KeyCode.LeftShift))
            {
                characterController.Move(movement * (Time.deltaTime * _runMultiplier));
            }
        }

        // Convert the direction of movement to avoid entering a impassible area
        private Vector3 PassableComponent(Vector3 movement)
        {
            if (movement.magnitude == 0) return movement;
            Vector3 moveDirection = movement.normalized;
            // Restricted distance for impassable areas
            float checkDistance = characterController.radius * 2f;
            // Inspection resolution
            int numStep = 10;

            // Stop if impassable to the move direction
            if (!CheckPassable(transform.position + moveDirection * checkDistance)) return Vector3.zero;
            
            // Scans a semicircular area in front of the player
            Vector3 leftPassibleDirection = movement;
            Vector3 rightPassibleDirection = movement;
            
            for (int i = 1; i < numStep; i++)
            {
                float checkAngle = 90f * i / numStep;
                Vector3 checkDirection = Quaternion.AngleAxis(checkAngle, Vector3.up) * moveDirection;
                if (!CheckPassable(transform.position + checkDirection * checkDistance)) break;
                leftPassibleDirection = checkDirection;
            }
            for (int i = 1; i < numStep; i++)
            {
                float checkAngle = -90f * i / numStep;
                Vector3 checkDirection = Quaternion.AngleAxis(checkAngle, Vector3.up) * moveDirection;
                if (!CheckPassable(transform.position + checkDirection * checkDistance)) break;
                rightPassibleDirection = checkDirection;
            }

            // Adjust the direction to bias it toward passable areas
            Vector3 newDirection = (leftPassibleDirection + rightPassibleDirection).normalized;
            return Vector3.Project(movement, newDirection);
        }

        // Check if a location is passable
        private bool CheckPassable(Vector3 position)
        {
            // Subject to modification based on development
            // Can also be used for slope limiting
            Ray ray = new Ray(position + Vector3.up * 0.5f, Vector3.down);
            if (!Physics.Raycast(ray, out RaycastHit hit)) return false;
            return !hit.transform.CompareTag("Water");
        }
    }
}
