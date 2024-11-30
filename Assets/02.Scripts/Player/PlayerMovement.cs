using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
    [Tooltip("additional speed multiplier when running")]
    [Range(1f, 20f)]
    [SerializeField] private float _runMultiplier;
    
    [Range(0f, 3f)]
    [SerializeField] private float _jumpHeight;
    
    [Tooltip("time taken to rotate when the direction of movement is changed")]
    [Range(0f, 0.1f)]
    [SerializeField] private float _smoothTime = 0.1f;

    private Rigidbody playerRigidBody;
    private Collider playerCollider;
    private Transform playerTransform;
    private Animator playerAnimator;
    private Renderer playerMeshRenderer;
    private Vector3 _controllerVelocity;
    private Vector3 _lastStablePosition;
    
    private PlayerInput input;
    
    private bool isJumpCooldown = false;
    private float jumpCooldownTime = 0.1f;
    private IEnumerator JumpCooldownCounter()
    {
        yield return new WaitForFixedUpdate();
        playerAnimator.SetBool("Jump", false);
        yield return new WaitForSeconds(jumpCooldownTime);
        isJumpCooldown = false;
    }

    private List<Collider> groundColliders = new();
    private bool isGrounded = true;
    
    private bool immune = false;
    private float respawnDelayTime = 0.5f;
    private float respawnPositionRestoreTime = 1f;
    private int respawnBlinkCount = 5;
    private float respawnBlinkTime = 0.1f;
    
    private int runLayer = 1;
    private float runLayerWeight = 0f;
    private float runTransitionSpeed = 3f;

    // Start is called before the first frame update
    private void Awake()
    {
        input = new PlayerInput();
        playerRigidBody = GetComponent<Rigidbody>();
        playerCollider = GetComponent<Collider>();
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
        if (immune) return;
        
        CheckStable();
        
        // moves the controller in the desired direction on the x- and z-axis
        Vector3 movement = transform.right * input.moveX + transform.forward * input.moveZ;
        movement *= _movementSpeed;

        // the controller is able to run
        if (input.run && isGrounded)
        {
            movement *= _runMultiplier;

            if(playerAnimator.GetBool("Grounded"))
            {
                runLayerWeight = Mathf.MoveTowards(runLayerWeight, 1f, Time.deltaTime * runTransitionSpeed);
            }
        }
        else
        {
            runLayerWeight = Mathf.MoveTowards(runLayerWeight, 0f, Time.deltaTime * runTransitionSpeed);
        }
        playerAnimator.SetLayerWeight(runLayer, runLayerWeight);
        
        float currentVelocity = movement.magnitude;
        
        // player movement is only controllable while being grounded
        playerAnimator.SetBool("Moving", currentVelocity > 0);
        playerRigidBody.velocity = new Vector3(movement.x, playerRigidBody.velocity.y, movement.z);
        if (currentVelocity > 0)
        {
            // set player's forward same as moving direction
            float targetAngle = Mathf.Atan2(input.moveX, input.moveZ) * Mathf.Rad2Deg - 90;
            float angle = Mathf.SmoothDampAngle(playerTransform.eulerAngles.y, targetAngle, ref currentVelocity, _smoothTime);
            playerTransform.rotation = Quaternion.Euler(0, angle, 0);
        }

        if (isGrounded && !isJumpCooldown)
        {
            playerAnimator.SetBool("Grounded", true);
        }
        
        // the controller is able to jump when on the ground
        if (input.jump && isGrounded && !isJumpCooldown)
        {
            playerAnimator.SetBool("Jump", true);
            playerAnimator.SetBool("Grounded", false);
            Vector3 jumpVelocity = Vector3.zero;
            jumpVelocity.y = Mathf.Sqrt(-_jumpHeight * 2f * Physics.gravity.y);
            playerRigidBody.AddForce(jumpVelocity, ForceMode.VelocityChange);
            isJumpCooldown = true;
            StartCoroutine(JumpCooldownCounter());
        }
    }

    // Convert the direction of movement to avoid entering a impassible area
    private void CheckStable()
    {
        Debug.DrawRay(_lastStablePosition, Vector3.up, Color.blue);
        if (!isGrounded) return;
        
        // Restricted distance for impassable areas
        float checkDistance = 2f;
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
        playerRigidBody.isKinematic = true;
        playerRigidBody.velocity = Vector3.zero;
        playerCollider.enabled = false;
        input.active = false;
        yield return new WaitForSeconds(0.5f);
        playerAnimator.SetBool("Grounded", true);
        playerAnimator.SetBool("Moving", false);
        yield return MoveSmooth(transform.position, _lastStablePosition, respawnPositionRestoreTime);
        yield return BlinkPlayer(respawnBlinkCount, respawnBlinkTime);
        Input.ResetInputAxes();
        input.active = true;
        playerMeshRenderer.enabled = true;
        playerRigidBody.isKinematic = false;
        playerCollider.enabled = true;
        immune = false;
    }
    
    // Smoothly move from startPos to endPos over finishTime
    private IEnumerator MoveSmooth(Vector3 startPos, Vector3 endPos, float finishTime)
    {
        float elapsedTime = 0f;
        while (elapsedTime < finishTime)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / finishTime;
            transform.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, normalizedTime));
            yield return null;
        }
    }
    
    // Repeat blinking for the specified time count times
    private IEnumerator BlinkPlayer(int count, float time)
    {
        for (int i=0; i<count; i++)
        {
            playerMeshRenderer.enabled = false;
            yield return new WaitForSeconds(time);
            playerMeshRenderer.enabled = true;
            yield return new WaitForSeconds(time);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.contacts[0].normal.y > 0.7f)
        {
            groundColliders.Add(other.collider);
            isGrounded = true;
        }
    }

    private void OnCollisionStay(Collision other)
    {
        foreach (var contact in other.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.red);
        }
        
        if (other.contacts[0].normal.y < 0.7f)
        {
            groundColliders.Remove(other.collider);
            if (groundColliders.Count == 0)
            {
                isGrounded = false;
            }
        }
        else if (!groundColliders.Contains(other.collider))
        {
            groundColliders.Add(other.collider);
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        groundColliders.Remove(other.collider);
        if (groundColliders.Count == 0) isGrounded = false;
    }
}
