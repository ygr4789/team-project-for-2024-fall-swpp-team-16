using UnityEngine;
using System.Collections;
using UnityEditor.IMGUI.Controls;

public enum AnimalState
{
    Idle,
    Rush,
    FollowPath
}

public class AnimalController : MonoBehaviour
{
    // Public variables for settings
    public AnimalState currentState = AnimalState.Idle;
    public float idleSpeed = 2f;
    public float rushSpeed = 15f;
    public float rushDuration = 3f;
    public ParticleSystem rushParticleEffect;
    public AnimalState baseState = AnimalState.Idle;
    public Transform[] pathPoints;

    // Private variables
    private Animator animator;
    private int currentPathIndex = 0;
    private bool isRushing = false;
    private float rushTimer = 0f;
    
    [Range(0f, 2f)]
    [SerializeField] private float colliderOriginHeight;
    
    [SerializeField] private Vector3 footOffsetLF;
    [SerializeField] private Vector3 footOffsetRF;
    [SerializeField] private Vector3 footOffsetLB;
    [SerializeField] private Vector3 footOffsetRB;
    [SerializeField] private LayerMask groundLayer;
    
    [Tooltip("waiting time before moving to the next waypoint")]
    [Range(0f, 5f)] 
    [SerializeField] private float pathWaitTime = 0.3f;
    
    private void Awake()
    {
        ResonatableObject resonatable = gameObject.AddComponent<ResonatableObject>();
        resonatable.properties = new[] { PitchType.Do };
        resonatable.resonate += AnimalResonate;
    }

    private void AnimalResonate(PitchType pitch)
    {
        switch (pitch)
        {
            case PitchType.Do: { isRushing = true; break; }
        }
    }

    void Start()
    {
        // Get the Animator component
        animator = GetComponent<Animator>();

        // Start the initial behavior based on the current state
        if (currentState == AnimalState.Idle)
        {
            StartCoroutine(IdleBehavior());
        }
        else if (currentState == AnimalState.FollowPath)
        {
            StartCoroutine(FollowPathBehavior());
        }
    }

    void Update()
    {
        if (isRushing)
        {
            isRushing = false;
            
            if (currentState != AnimalState.Rush)
            {
                TriggerRush();
            }
    
            // Move forward while rushing
            if (!CollidingFront()) transform.Translate(Vector3.forward * (rushSpeed * Time.deltaTime));
            StickToGround();
        }
        else
        {
            if (currentState == AnimalState.Rush)
            {
                StopRush(); // Stop rushing when the key is released
            }
        }
    }
    
    private void StickToGround()
    {
        Vector3[] footOffsets = new Vector3[4];
        footOffsets[0] = footOffsetLF;
        footOffsets[1] = footOffsetRF;
        footOffsets[2] = footOffsetLB;
        footOffsets[3] = footOffsetRB;

        float groundHeight = 0f;
        Vector3 groundNormal = Vector3.up * 0.01f;
        foreach (Vector3 footOffset in footOffsets)
        {
            Vector3 rayOrigin = transform.position + transform.TransformDirection(footOffset) + transform.up * colliderOriginHeight;
            Ray ray = new Ray(rayOrigin, Vector3.down);
            Debug.DrawRay(rayOrigin, Vector3.down, Color.green);
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                groundHeight += hit.point.y;
                groundNormal += hit.normal;
            }
            gameObject.layer = LayerMask.NameToLayer("Default");
        }
        groundHeight /= footOffsets.Length;
        groundNormal.Normalize();
        Vector3 groundPosition = transform.position;
        groundPosition.y = groundHeight;
        transform.position = groundPosition;
        
        Vector3 flattenedForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
        float u = -Vector3.Dot(flattenedForward, groundNormal);
        float f = Vector3.Dot(Vector3.up, groundNormal);
        Vector3 forward = (flattenedForward * f + Vector3.up * u).normalized;
        
        transform.rotation = Quaternion.LookRotation(forward, groundNormal);
    }
    
    private bool CollidingFront()
    {
        Vector3 moveDirection = transform.forward;
        Vector3 checkOffset = transform.up * colliderOriginHeight;
        float checkDistance = 10f;
        // Inspection resolution
        int numStep = 10;

        for (int i = -numStep; i <= numStep; i++)
        {
            float checkAngle = 90f * i / numStep;
            Vector3 checkDirection = Quaternion.AngleAxis(checkAngle, Vector3.up) * moveDirection;
            Ray outRay = new Ray(transform.position + checkOffset, checkDirection);
            if (Physics.Raycast(outRay, out RaycastHit hit, checkDistance, groundLayer))
            {
                Ray inRay = new Ray(hit.point, -checkDirection);
                float hitDistance = hit.distance;
                Physics.Raycast(inRay, out hit, hitDistance, groundLayer);
                
                if (hit.transform != transform)
                {
                    return true;
                }
            }
        }

        return false;
    }
    
    public void TriggerRush()
    {
        if (currentState != AnimalState.Rush)
        {
            // Change state to Rush
            currentState = AnimalState.Rush;
    
            // Stop other behaviors to prevent conflicts
            StopAllCoroutines();
    
            // Play particle effect if available
            if (rushParticleEffect)
            {
                rushParticleEffect.Play();
            }
    
            // Set animator parameter
            animator.SetBool("isRushing", true);
        }
    }
    
    void StopRush()
    {
        isRushing = false;
    
        // Stop particle effect if playing
        if (rushParticleEffect)
        {
            rushParticleEffect.Stop();
        }
    
        // Reset animator parameter
        animator.SetBool("isRushing", false);
    
        // Return to the previous behavior
        if (pathPoints != null && pathPoints.Length > 0)
        {
            currentState = AnimalState.FollowPath;
            StartCoroutine(FollowPathBehavior());
        }
        else
        {
            currentState = AnimalState.Idle;
            StartCoroutine(IdleBehavior());
        }
    }


    IEnumerator IdleBehavior()
    {
        while (currentState == AnimalState.Idle)
        {   animator.SetBool("isMoving", false);
            // Randomly select an idle animation
            int randomIndex = Random.Range(0, 3);

            switch (randomIndex)
            {
                case 0:
                    // Play turn left animation
                    animator.Play("turn_90_L");
                    yield return new WaitForSeconds(2f);
                    break;

                case 1:
                    // Play sit and stand animations
                    animator.Play("sit_to_stand");
                   // yield return new WaitForSeconds(2f);
                    animator.Play("stand_to_sit");
                    yield return new WaitForSeconds(8f);
                    break;

                case 2:
                    // Play turn right animation
                    animator.Play("turn_90_R");
                    yield return new WaitForSeconds(2f);
                    break;
            }

            // Wait before performing the next idle action
            yield return new WaitForSeconds(Random.Range(2f, 5f));
        }
    }

    IEnumerator FollowPathBehavior()
    {
        while (currentState == AnimalState.FollowPath)
        {
            if (pathPoints.Length == 0)
                yield break;

            // Get the next waypoint
            Vector3 targetPosition = pathPoints[currentPathIndex].position;
            targetPosition.y = transform.position.y; // Keep the same height

            // Rotate towards the target
            Vector3 targetDirection = Vector3.ProjectOnPlane(targetPosition - transform.position, Vector3.up).normalized;
            Vector3 currentDirection = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            while (Vector3.Angle(targetDirection, currentDirection) > 0.5f)
            {
                currentDirection = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
                transform.forward = Vector3.RotateTowards(currentDirection, targetDirection, idleSpeed * 3 * Time.deltaTime, 0f);
                StickToGround();
                yield return null;
            
                if (currentState != AnimalState.FollowPath)
                    yield break;
            }

            // Start walking animation
            animator.SetBool("isMoving", true);

            // Move towards the waypoint
            while (Vector3.Distance(transform.position, targetPosition) > 0.5f)
            {
                if (!CollidingFront()) transform.position = Vector3.MoveTowards(transform.position, targetPosition, idleSpeed * Time.deltaTime);
                yield return null;
                StickToGround();

                if (currentState != AnimalState.FollowPath)
                    yield break;
            }

            // Stop walking animation
            // animator.SetBool("isMoving", false);

            // Move to the next waypoint
            currentPathIndex = (currentPathIndex + 1) % pathPoints.Length;

            // Wait before moving to the next waypoint
            yield return new WaitForSeconds(pathWaitTime);
        }
    }
}
