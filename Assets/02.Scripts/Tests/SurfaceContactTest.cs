using UnityEngine;

using UnityEngine;

public class SurfaceContactTest : MonoBehaviour 
{
	[Tooltip("Max acceleration on ground")]
	[SerializeField, Range(10f, 100f)]
	private float maxAcceleration = 50f;

	[Tooltip("Max acceleration on air or unclimbable slope")]
	[SerializeField, Range(0f, 10f)]
	private float maxAirAcceleration = 5f;
	
	[SerializeField, Range(0f, 10f)]
	private float jumpHeight = 2f;

	[Tooltip("Max climbable slope")]
	[SerializeField, Range(0, 90)]
	private float maxGroundAngle = 25f;

	[Tooltip("Ground detection distance")]
	[SerializeField, Min(0f)]
	private float probeDistance = 1f;
	
	[Tooltip("Ground layer mask")]
	[SerializeField]
	private LayerMask probeMask = -1;

	private Rigidbody body;

	private Vector3 velocity; // internally calculated velocity
	private Vector3 desiredVelocity; // desired velocity (input)
	private bool desiredJump; // jump (input)

	private Vector3 contactNormal; // average normal of climbable contacts
	private Vector3 steepNormal; // average normal of positive-y-normal contacts

	private int groundContactCount; // total number of climbable contacts
	private int steepContactCount; // total number of positive-y-normal contacts

	private bool OnGround => groundContactCount > 0;
	private bool OnSteep => steepContactCount > 0;

	private float minGroundDotProduct; // minimum y-component of climbable slope normal

	private int stepsSinceLastGrounded; // number of fixed frames since last grounded
	private int stepsSinceLastJump; // number of fixed frames since last jump
	
	public Vector3 Velocity
	{
		get => body.velocity;
		set
		{
			Vector3 flattenVelocity = value;
			flattenVelocity.y = 0f;
			desiredVelocity = flattenVelocity;
		}
	}

	public bool Jump
	{
		set => desiredJump = value;
	}
	
	private void OnValidate () {
		minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
	}

	private void Awake () {
		body = GetComponent<Rigidbody>();
		OnValidate();
	}

	private void FixedUpdate () {
		UpdateState();
		AdjustVelocity();

		if (desiredJump) {
			desiredJump = false;
			TriggerJump();
		}
		
		body.AddForce(velocity - body.velocity, ForceMode.VelocityChange);
		ClearState();
	}

	// Initialize properties
	private void ClearState () {
		groundContactCount = steepContactCount = 0;
		contactNormal = steepNormal = Vector3.zero;
	}

	// Calculate all properties
	private void UpdateState () {
		stepsSinceLastGrounded += 1;
		stepsSinceLastJump += 1;
		velocity = body.velocity;
		if (OnGround || SnapToGround() || CheckSteepContacts()) {
			stepsSinceLastGrounded = 0;
			if (groundContactCount > 1) {
				contactNormal.Normalize();
			}
		}
		else {
			contactNormal = Vector3.up;
		}
	}

	// Returns whether surface contact is maintained
	// If true, removes the directional component velocity away from the surface
	private bool SnapToGround () {
		if (stepsSinceLastGrounded > 1 || stepsSinceLastJump <= 2) return false;
		if (!Physics.Raycast(body.position, Vector3.down, out var hit, probeDistance, probeMask)) return false;
		if (hit.normal.y < minGroundDotProduct) return false;

		groundContactCount = 1;
		contactNormal = hit.normal;
		float dot = Vector3.Dot(velocity, hit.normal);
		if (dot > 0f) velocity = Vector3.ProjectOnPlane(velocity, hit.normal);
		return true;
	}

	// Check if the object is stuck in terrain like crevasses
	private bool CheckSteepContacts () {
		if (steepContactCount > 1) {
			steepNormal.Normalize();
			if (steepNormal.y >= minGroundDotProduct) {
				steepContactCount = 0;
				groundContactCount = 1;
				contactNormal = steepNormal;
				return true;
			}
		}
		return false;
	}

	// Apply velocities projected to the contact normal plane
	private void AdjustVelocity () {
		float acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
		float maxSpeedChange = acceleration * Time.deltaTime;

		Vector3 currentProjectedVelocity = Vector3.ProjectOnPlane(velocity, contactNormal);
		Vector3 desiredProjectedVelocity = Vector3.ProjectOnPlane(desiredVelocity, contactNormal);
		desiredProjectedVelocity.Normalize();
		desiredProjectedVelocity *= desiredVelocity.magnitude;
		Vector3 newProjectedVelocity = Vector3.MoveTowards(currentProjectedVelocity, desiredProjectedVelocity, maxSpeedChange);
		
		velocity += (newProjectedVelocity - currentProjectedVelocity);
	}

	// Jump by jumpHeight 
	private void TriggerJump () {
		Vector3 jumpDirection = Vector3.up;
		if (!OnGround && !OnSteep) return;

		stepsSinceLastJump = 0;
		float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
		float alignedSpeed = Vector3.Dot(velocity, jumpDirection);
		if (alignedSpeed > 0f) jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
		
		velocity += jumpDirection * jumpSpeed;
	}

	private void OnCollisionEnter (Collision collision) {
		EvaluateCollision(collision);
	}

	private void OnCollisionStay (Collision collision) {
		EvaluateCollision(collision);
	}

	// Record collision counts and contact normal
	private void EvaluateCollision (Collision collision)
	{
		float minDot = minGroundDotProduct;
		for (int i = 0; i < collision.contactCount; i++) {
			Vector3 normal = collision.GetContact(i).normal;
			if (normal.y >= minDot) {
				groundContactCount += 1;
				contactNormal += normal;
			}
			else if (normal.y > -0.01f) {
				steepContactCount += 1;
				steepNormal += normal;
			}
		}
	}
}