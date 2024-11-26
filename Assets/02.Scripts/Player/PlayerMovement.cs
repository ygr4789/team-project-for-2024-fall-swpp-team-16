using System;
using System.Collections;
using UnityEngine;

namespace _12.Tests.PlayerDev
{
    public class PlayerInput
    {
        public float moveX;
        public float moveZ;
        public bool jump;
        public bool run;
        public bool active = true;
        
        public void Clear()
        {
            moveX = 0f;
            moveZ = 0f;
            jump = false;
            run = false;
        }
    }
    
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
        private Renderer playerMeshRenderer;
        private Vector3 _controllerVelocity;
        private Vector3 _lastStablePosition;
        
        private PlayerInput input;
        
        private IEnumerator jumpCheckGroundAvoider; 
        private IEnumerator JumpCheckGroundAvoider()
        {
            yield return new WaitForFixedUpdate();
            playerAnimator.SetBool("Jump", false);
        }

        private bool immune = false;
        
        private int runLayer = 1;
        private float runLayerWeight = 0f;
        private float runTransitionSpeed = 3f;

        // Start is called before the first frame update
        private void Awake()
        {
            input = new PlayerInput();
            characterController = GetComponent<CharacterController>();
            playerTransform = transform.Find("Player");
            playerAnimator = playerTransform.GetComponent<Animator>();
            playerMeshRenderer = playerTransform.GetComponentInChildren<Renderer>();
        }

        // Update is called once per frame
        private void Update()
        {
            GetInputs();
            ControlPlayer();
        }

        private void GetInputs()
        {
            input.Clear();
            if(!input.active) return;
            
            // get the movement input
            input.moveX = Input.GetAxis("Horizontal");
            input.moveZ = Input.GetAxis("Vertical");
            input.jump = Input.GetButton("Jump");
            input.run = Input.GetKey(KeyCode.LeftShift);
        }

        private void ControlPlayer()
        {
            // stops the y velocity when player is on the ground and the velocity has reached 0
            if (characterController.isGrounded && _controllerVelocity.y < 0)
            {
                _controllerVelocity.y = 0;
            }

            if (immune) return;
            
            CheckStable();
            
            // moves the controller in the desired direction on the x- and z-axis
            Vector3 movement = transform.right * input.moveX + transform.forward * input.moveZ;
            characterController.Move(movement * (_movementSpeed * Time.deltaTime));

            // the controller is able to run
            if (input.run)
            {
                characterController.Move(movement * (Time.deltaTime * _runMultiplier));

                if(playerAnimator.GetBool("Grounded"))
                {
                    runLayerWeight = Mathf.MoveTowards(runLayerWeight, 1f, Time.deltaTime * runTransitionSpeed);
                }
            }else{
                runLayerWeight = Mathf.MoveTowards(runLayerWeight, 0f, Time.deltaTime * runTransitionSpeed);
            }
            playerAnimator.SetLayerWeight(runLayer, runLayerWeight);
            
            // set player's forward same as moving direction
            float currentVelocity = movement.magnitude;
            playerAnimator.SetBool("Moving", currentVelocity > 0);
            if (currentVelocity > 0)
            {
                float targetAngle = Mathf.Atan2(input.moveX, input.moveZ) * Mathf.Rad2Deg - 90;
                float angle = Mathf.SmoothDampAngle(playerTransform.eulerAngles.y, targetAngle, ref currentVelocity, _smoothTime);
                playerTransform.rotation = Quaternion.Euler(0, angle, 0);
            }
                
            // gravity affects the controller on the y-axis
            _controllerVelocity.y += _gravity * Time.deltaTime;

            // moves the controller on the y-axis
            characterController.Move(_controllerVelocity * Time.deltaTime);
            
            if (characterController.isGrounded) playerAnimator.SetBool("Grounded", true);

            // the controller is able to jump when on the ground
            if (input.jump && characterController.isGrounded)
            {
                playerAnimator.SetBool("Jump", true);
                playerAnimator.SetBool("Grounded", false);
                _controllerVelocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
                
                if(jumpCheckGroundAvoider != null)
                {
                    StopCoroutine(jumpCheckGroundAvoider);
                }
                jumpCheckGroundAvoider = JumpCheckGroundAvoider();
                StartCoroutine(jumpCheckGroundAvoider);
            }
        }

        // Convert the direction of movement to avoid entering a impassible area
        private void CheckStable()
        {
            Debug.DrawRay(_lastStablePosition, Vector3.up, Color.blue);
            if (!characterController.isGrounded) return;
            
            // Restricted distance for impassable areas
            float checkDistance = characterController.radius * 10f;
            // Inspection resolution
            int numStep = 20;
            
            for (int i = 0; i < numStep; i++)
            {
                float checkAngle = 360f * i / numStep;
                Vector3 checkDirection = Quaternion.AngleAxis(checkAngle, Vector3.up) * Vector3.forward;
                if (!CheckPassable(transform.position + checkDirection * checkDistance)) return;
            }

            _lastStablePosition = transform.position;
        }

        // Check if a location is passable
        private bool CheckPassable(Vector3 position)
        {
            // Subject to modification based on development
            Ray ray = new Ray(position + Vector3.up * 1.5f, Vector3.down);
            if (!Physics.Raycast(ray, out RaycastHit hit)) return false;
            return !hit.transform.CompareTag("Water");
        }

        public void Drown()
        {
            // 플레이어가 물에 빠지는 이펙트 발생 필요
            if (immune) return;
            immune = true;
            StartCoroutine(RespawnPlayer());
        }
        
        private IEnumerator RespawnPlayer()
        {
            playerMeshRenderer.enabled = false;
            input.active = false;
            yield return new WaitForSeconds(0.5f);
            playerAnimator.SetBool("Grounded", true);
            playerAnimator.SetBool("Moving", false);
            yield return MoveSmooth(transform.position, _lastStablePosition);
            yield return BlinkPlayer();
            Input.ResetInputAxes();
            input.active = true;
            immune = false;
        }
        
        private IEnumerator MoveSmooth(Vector3 startPos, Vector3 endPos)
        {
            float delayTime = 1f;
            float elapsedTime = 0f;
            
            characterController.enabled = false;
            while (elapsedTime < delayTime)
            {
                elapsedTime += Time.deltaTime;
                float normalizedTime = elapsedTime / delayTime;
                transform.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, normalizedTime));
                yield return null;
            }
            characterController.enabled = true;
        }
        
        private IEnumerator BlinkPlayer()
        {
            for (int i=0; i<5; i++)
            {
                playerMeshRenderer.enabled = false;
                yield return new WaitForSeconds(0.1f);
                playerMeshRenderer.enabled = true;
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
