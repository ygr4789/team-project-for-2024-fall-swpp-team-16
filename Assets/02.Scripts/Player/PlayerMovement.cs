using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerInput
{
    public float MoveX;
    public float MoveZ;
    public bool Jump;
    public bool Run;
    public bool Active = true;
    
    public void Clear()
    {
        MoveX = 0f;
        MoveZ = 0f;
        Jump = false;
        Run = false;
    }
}

public class PlayerMovement : MonoBehaviour
{
    private static readonly int Moving = Animator.StringToHash("Moving");
    private static readonly int Grounded = Animator.StringToHash("Grounded");
    private static readonly int Jump = Animator.StringToHash("Jump");

    [FormerlySerializedAs("_movementSpeed")]
    [Range(1f, 20f)]
    [SerializeField] private float movementSpeed;
    
    [FormerlySerializedAs("_runMultiplier")]
    [Tooltip("additional speed multiplier when running")]
    [Range(1f, 20f)]
    [SerializeField] private float runMultiplier;

    [FormerlySerializedAs("_smoothTime")]
    [Tooltip("time taken to rotate when the direction of movement is changed")]
    [Range(0f, 0.1f)]
    [SerializeField] private float smoothTime = 0.1f;

    private Camera _controllerCamera;
    private SurfaceContactController _playerController;
    private Rigidbody _playerRigidBody;
    private Collider _playerCollider;
    private Transform _playerTransform;
    private Animator _playerAnimator;
    private Renderer _playerMeshRenderer;
    private Vector3 _controllerVelocity;
    private Vector3 _lastStablePosition;
    
    private PlayerInput _input;
    
    private const float RespawnDelayTime = 0.5f;
    private const float RespawnPositionRestoreTime = 1f;
    private const int RespawnBlinkCount = 5;
    private const float RespawnBlinkTime = 0.1f;
    private bool _immune = false;

    private const int RunLayer = 1;
    private const float RunTransitionSpeed = 3f;
    private float _runLayerWeight = 0f;

    private Vector3 _directionX, _directionZ;

    // Start is called before the first frame update
    private void Awake()
    {
        var objectDetector = new GameObject
        {
            name = "objectDetector"
        };
        objectDetector.AddComponent<ObjectDetecter>();
        
        _input = new PlayerInput();
        _controllerCamera = Camera.main;
        _playerRigidBody = GetComponent<Rigidbody>();
        _playerController = GetComponent<SurfaceContactController>();
        _playerCollider = GetComponent<Collider>();
        _playerTransform = transform.Find("Player");
        _playerAnimator = _playerTransform.GetComponent<Animator>();
        _playerMeshRenderer = _playerTransform.GetComponentInChildren<Renderer>();
        _lastStablePosition = transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        GetInputs();
        ControlPlayer();
    }
    
    public PlayerInput GetPlayerInput()
    {
        return _input;
    }

    private void GetInputs()
    {
        _input.Clear();
        if(!_input.Active) return;
        
        // get the movement input
        _input.MoveX = Input.GetAxis("Horizontal");
        _input.MoveZ = Input.GetAxis("Vertical");
        _input.Jump = Input.GetButton("Jump");
        _input.Run = Input.GetKey(KeyCode.LeftShift);
    }

    private void ControlPlayer()
    {
        if (_immune) return;
        
        CheckStable();
        CalculateDirection();
        
        // moves the controller in the desired direction on the x- and z-axis
        var moveDirection = _directionX * _input.MoveX + _directionZ * _input.MoveZ;
        var movement = moveDirection * movementSpeed;

        HandleRunning(ref movement);
        HandleMovement(movement);
        HandleGroundJump();
    }

    private void HandleRunning(ref Vector3 movement)
    {
        // the controller is able to run
        if (_input.Run && _playerController.Grounded)
        {
            movement *= runMultiplier;

            if(_playerAnimator.GetBool(Grounded))
            {
                _runLayerWeight = Mathf.MoveTowards(_runLayerWeight, 1f, Time.deltaTime * RunTransitionSpeed);
            }
        }
        else
        {
            _runLayerWeight = Mathf.MoveTowards(_runLayerWeight, 0f, Time.deltaTime * RunTransitionSpeed);
        }
        _playerAnimator.SetLayerWeight(RunLayer, _runLayerWeight);
    }

    private void HandleMovement(Vector3 movement)
    {
        _playerController.Velocity = movement;
        var currentVelocity = movement.magnitude;
        _playerAnimator.SetBool(Moving, currentVelocity > 0);
        if (!(currentVelocity > 0)) return;
        // set player's forward same as moving direction
        var targetAngle = Mathf.Atan2(-movement.z, movement.x) * Mathf.Rad2Deg + 90;
        var angle = Mathf.SmoothDampAngle(_playerTransform.eulerAngles.y, targetAngle, ref currentVelocity, smoothTime);
        _playerTransform.rotation = Quaternion.Euler(0, angle, 0);
    }

    private void HandleGroundJump()
    {
        // the controller is able to jump when on the ground
        _playerAnimator.SetBool(Grounded, _playerController.Grounded);
        if (_input.Jump && _playerController.Grounded)
        {
            _playerAnimator.SetBool(Jump, true);
            _playerAnimator.SetBool(Grounded, false);
            _playerController.Jump = true;
        }
        else
        {
            _playerAnimator.SetBool(Jump, false);
        }
    }

    private void CalculateDirection()
    {
        _directionX = _controllerCamera.transform.right;
        _directionZ = Vector3.Cross(_directionX, Vector3.up);
    }

    // Convert the direction of movement to avoid entering a impassible area
    private void CheckStable()
    {
        Debug.DrawRay(_lastStablePosition, Vector3.up, Color.blue);
        if (!_playerController.Grounded) return;
        
        // Restricted distance for impassable areas
        const float checkDistance = 2f;
        // Inspection resolution
        const int numStep = 20;
        
        for (var i = 0; i < numStep; i++)
        {
            var checkAngle = 360f * i / numStep;
            var checkDirection = Quaternion.AngleAxis(checkAngle, Vector3.up) * Vector3.forward;
            if (!CheckPassable(transform.position + checkDirection * checkDistance)) return;
        }

        _lastStablePosition = transform.position;
    }

    // Check if a location is passable
    private bool CheckPassable(Vector3 position)
    {
        // Subject to modification based on development
        var ray = new Ray(position + Vector3.up * 1.5f, Vector3.down);
        Debug.DrawRay(position + Vector3.up * 1.5f, Vector3.down * 1.5f, Color.red);
        if (!Physics.Raycast(ray, out var hit)) return false;
        return !hit.transform.CompareTag("Water");
    }

    public void Drown()
    {
        // 플레이어가 물에 빠지는 이펙트 발생 필요
        if (_immune) return;
        _immune = true;
        StartCoroutine(RespawnPlayer());
    }
    
    private IEnumerator RespawnPlayer()
    {
        _immune = true;
        _input.Active = false;
        _playerMeshRenderer.enabled = false;
        _playerRigidBody.isKinematic = true;
        _playerCollider.enabled = false;
        _playerRigidBody.velocity = Vector3.zero;
        yield return new WaitForSeconds(RespawnDelayTime);
        _playerAnimator.SetBool(Grounded, true);
        _playerAnimator.SetBool(Moving, false);
        yield return MoveSmooth(transform.position, _lastStablePosition, RespawnPositionRestoreTime);
        yield return BlinkPlayer(RespawnBlinkCount, RespawnBlinkTime);
        Input.ResetInputAxes();
        _immune = false;
        _input.Active = true;
        _playerMeshRenderer.enabled = true;
        _playerRigidBody.isKinematic = false;
        _playerCollider.enabled = true;
    }
    
    // Smoothly move from startPos to endPos over finishTime
    private IEnumerator MoveSmooth(Vector3 startPos, Vector3 endPos, float finishTime)
    {
        var elapsedTime = 0f;
        while (elapsedTime < finishTime)
        {
            elapsedTime += Time.deltaTime;
            var normalizedTime = elapsedTime / finishTime;
            transform.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, normalizedTime));
            yield return null;
        }
    }

    // Walk from startPos to endPos over finishTime
    public IEnumerator WalkToPoint(Vector3 targetPosition, float duration)
    {
        _input.Active = false;
        var currentPosition = transform.position;
        var movement = (targetPosition - currentPosition) / duration;
        movement.y = 0f;
        var elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            HandleMovement(movement);
            if (Vector3.Distance(_playerTransform.position, targetPosition) < 0.5f) break;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _input.Active = true;
    }
    
    // Repeat blinking for the specified time count times
    private IEnumerator BlinkPlayer(int count, float time)
    {
        for (var i=0; i<count; i++)
        {
            _playerMeshRenderer.enabled = false;
            yield return new WaitForSeconds(time);
            _playerMeshRenderer.enabled = true;
            yield return new WaitForSeconds(time);
        }
    }
}
