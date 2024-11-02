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
        void Start()
        {
            characterController = GetComponent<CharacterController>();
            playerTransform = transform.Find("Player");
            playerAnimator = playerTransform.GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
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
            characterController.Move(movement * (_movementSpeed * Time.deltaTime));
            

            // set player's forward same as moving direction
            var currentVelocity = movement.magnitude;
            if (currentVelocity > 0)
            {
                var targetAngle = Mathf.Atan2(moveX, moveZ) * Mathf.Rad2Deg - 90;
                var angle = Mathf.SmoothDampAngle(playerTransform.eulerAngles.y, targetAngle, ref currentVelocity, _smoothTime);
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
    }
}
