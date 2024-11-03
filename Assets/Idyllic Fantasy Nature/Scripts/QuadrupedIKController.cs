using UnityEngine;

public class QuadrupedIKController : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;

    [Header("Foot Targets")]
    public Transform frontLeftFootTarget;
    public Transform frontRightFootTarget;
    public Transform backLeftFootTarget;
    public Transform backRightFootTarget;

    [Header("IK Settings")]
    public float raycastDistance = 2f;
    public float footOffset = 0.1f;
    public LayerMask terrainLayer;
    public float smoothSpeed = 10f;

    // Initial positions for interpolation
    private Vector3 frontLeftInitialPos;
    private Vector3 frontRightInitialPos;
    private Vector3 backLeftInitialPos;
    private Vector3 backRightInitialPos;

    // To alternate between front and back feet
    private bool isFrontFeetActive = true;
    private float footSwitchInterval = 0.5f; // Adjust as needed
    private float footSwitchTimer = 0f;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        // Store initial positions
        frontLeftInitialPos = frontLeftFootTarget.position;
        frontRightInitialPos = frontRightFootTarget.position;
        backLeftInitialPos = backLeftFootTarget.position;
        backRightInitialPos = backRightFootTarget.position;
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (animator)
        {
            if (isFrontFeetActive)
            {
                HandleFootIK(AvatarIKGoal.LeftFoot, frontLeftFootTarget, frontLeftInitialPos);
                HandleFootIK(AvatarIKGoal.RightFoot, frontRightFootTarget, frontRightInitialPos);
            }
            else
            {
                HandleFootIK(AvatarIKGoal.LeftFoot, backLeftFootTarget, backLeftInitialPos);
                HandleFootIK(AvatarIKGoal.RightFoot, backRightFootTarget, backRightInitialPos);
            }
        }
    }

    void Update()
    {
        UpdateFootTargetPosition(frontLeftFootTarget, frontLeftInitialPos);
        UpdateFootTargetPosition(frontRightFootTarget, frontRightInitialPos);
        UpdateFootTargetPosition(backLeftFootTarget, backLeftInitialPos);
        UpdateFootTargetPosition(backRightFootTarget, backRightInitialPos);

        // Timer to switch active feet
        footSwitchTimer += Time.deltaTime;
        if (footSwitchTimer >= footSwitchInterval)
        {
            isFrontFeetActive = !isFrontFeetActive;
            footSwitchTimer = 0f;
        }
    }

    void HandleFootIK(AvatarIKGoal foot, Transform footTarget, Vector3 initialPos)
    {
        // Set IK position and rotation weights
        animator.SetIKPositionWeight(foot, 1);
        animator.SetIKRotationWeight(foot, 1);

        // Smoothly interpolate foot position
        Vector3 targetPosition = footTarget.position;
        animator.SetIKPosition(foot, targetPosition + Vector3.down * footOffset);

        // Smoothly interpolate foot rotation
        Quaternion targetRotation = footTarget.rotation;
        animator.SetIKRotation(foot, targetRotation);
    }

    void UpdateFootTargetPosition(Transform footTarget, Vector3 initialPos)
{
    RaycastHit hit;

    // Adjust the rayOrigin to be relative to the bear's position
    Vector3 rayOrigin = footTarget.position + Vector3.up;

    int layerMask = terrainLayer.value;

    if (Physics.Raycast(rayOrigin, Vector3.down, out hit, raycastDistance, layerMask))
    {
        Vector3 newPos = hit.point;
        newPos.y += footOffset;
        footTarget.position = Vector3.Lerp(footTarget.position, newPos, Time.deltaTime * smoothSpeed);
    }
    else
    {
        // Instead of using initialPos, which is the starting position,
        // use the bear's current position plus an offset for the foot target.
        Vector3 bearPosition = transform.position;
        Vector3 direction = footTarget.position - bearPosition;
        Vector3 targetPos = bearPosition + direction.normalized * footOffset;
        footTarget.position = Vector3.Lerp(footTarget.position, targetPos, Time.deltaTime * smoothSpeed);
    }
}

      
    void OnDrawGizmos()
    {
        if (frontLeftFootTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(frontLeftFootTarget.position, 0.1f);
        }

        if (frontRightFootTarget != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(frontRightFootTarget.position, 0.1f);
        }

        if (backLeftFootTarget != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(backLeftFootTarget.position, 0.1f);
        }

        if (backRightFootTarget != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(backRightFootTarget.position, 0.1f);
        }
    }
}
