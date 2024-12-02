using UnityEngine;
using System.Collections;
using UnityEditor.IMGUI.Controls;
using UnityEngine.Assertions;

public enum AnimalState
{
    Idle,
    Rush,
    FollowPath
}

public class AnimalController : MonoBehaviour
{
    // SerializeFields
    [SerializeField] private AnimalState currentState = AnimalState.FollowPath;
    [Range(0f, 5f)]
    [SerializeField] private float idleSpeed = 2f;
    [Range(0f, 10f)]
    [SerializeField] private float rushSpeed = 4f;
    [SerializeField] private ParticleSystem rushParticleEffect;
    [SerializeField] private Transform[] pathPoints;
    [SerializeField] private Transform animalModel;
    [Tooltip("waiting time before moving to the next waypoint")]
    [Range(0f, 5f)] 
    [SerializeField] private float pathWaitTime = 0.3f;
    
    private SurfaceContactController animalBody;
    private Animator animalAnimator;
    private Vector3 currentForward;
    private int currentPathIndex = 0;
    private bool isRushing = false;
    
    private void Awake()
    {
        Assert.IsNotNull(animalModel);
        animalAnimator = animalModel.GetComponent<Animator>();
        Assert.IsNotNull(animalAnimator);
        animalBody = GetComponent<SurfaceContactController>();
        Assert.IsNotNull(animalBody);

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
        currentForward = animalModel.forward;
        
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
        animalModel.forward = currentForward;
        animalAnimator.SetBool("isMoving", animalBody.Velocity.magnitude > 0.1f);
        
        if (isRushing)
        {
            isRushing = false;
            if (currentState != AnimalState.Rush) TriggerRush();
    
            // Move forward while rushing
            animalBody.Velocity = currentForward * rushSpeed;
        }
        else if (currentState == AnimalState.Rush) StopRush(); // Stop rushing when the key is released
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
            animalAnimator.SetLayerWeight(animalAnimator.GetLayerIndex("Rush"), 1f);
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
        animalAnimator.SetLayerWeight(animalAnimator.GetLayerIndex("Rush"), 0f);
        animalBody.Velocity = Vector3.zero;
    
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
        {   
            yield return new WaitForSeconds(3f);
            animalAnimator.Play("stand_to_sit");
            yield return new WaitForSeconds(3f);
            animalAnimator.Play("sit_to_stand");
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

            // Rotate towards the target
            Vector3 targetDirection = Vector3.ProjectOnPlane(targetPosition - transform.position, Vector3.up).normalized;
            Vector3 currentDirection = Vector3.ProjectOnPlane(currentForward, Vector3.up).normalized;
            while (Vector3.Angle(targetDirection, currentDirection) > 0.5f)
            {
                currentDirection = Vector3.ProjectOnPlane(currentForward, Vector3.up).normalized;
                currentForward = Vector3.RotateTowards(currentDirection, targetDirection, idleSpeed * 3 * Time.deltaTime, 0f);
                yield return null;

                if (currentState != AnimalState.FollowPath) yield break;
            }

            // Move towards the waypoint
            float flattenDistance = Vector3.Distance(targetPosition, transform.position);
            while (flattenDistance > 0.5f)
            {
                Vector3 flattenDisplacement = targetPosition - transform.position;
                flattenDisplacement.y = 0f;
                flattenDistance = flattenDisplacement.magnitude;
                
                targetDirection = Vector3.ProjectOnPlane(targetPosition - transform.position, Vector3.up).normalized;
                currentForward = targetDirection;
                animalBody.Velocity = currentForward * idleSpeed;
                yield return null;

                if (currentState != AnimalState.FollowPath) yield break;
            }

            // Stop walking animation
            animalBody.Velocity = Vector3.zero;

            // Move to the next waypoint
            currentPathIndex = (currentPathIndex + 1) % pathPoints.Length;

            // Wait before moving to the next waypoint
            yield return new WaitForSeconds(pathWaitTime);
        }
    }
}
