using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class SurfaceContactController : MonoBehaviour
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
	private float maxGroundAngle = 45f;

	[Tooltip("Ground detection distance")]
	[SerializeField, Min(0f)]
	private float probeDistance = 0.5f;
	
	[Tooltip("Ground layer mask")]
	[SerializeField]
	private LayerMask probeMask = -1;

	private float originHeight; // The height from the lowest point of the collider to the origin of the rigidbody
	private Rigidbody body;

	private Vector3 velocity; // internally calculated velocity
	private Vector3 desiredVelocity; // desired velocity (input)
	private bool desiredJump; // jump (input)

	private Vector3 contactNormal; // average normal of climbable contacts
	private Vector3 steepNormal; // average normal of positive-y-normal contacts

	private int groundContactCount; // total number of climbable contacts
	private int steepContactCount; // total number of positive-y-normal contacts

	private float minGroundDotProduct; // minimum y-component of climbable slope normal

	private int stepsSinceLastGrounded; // number of fixed frames since last grounded
	private int stepsSinceLastJump; // number of fixed frames since last jump
	private bool ContactingGround => groundContactCount > 0;
	public bool Grounded => stepsSinceLastGrounded == 0;
	
	public Vector3 Velocity
	{
		get => body.velocity;
		set
		{
			var flattenVelocity = value;
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
		Assert.IsNotNull(body);
		
		var controllerCollider = GetComponent<Collider>();
		Assert.IsNotNull(controllerCollider);
		originHeight = body.position.y - controllerCollider.bounds.min.y;
		
		OnValidate();
	}

	private void FixedUpdate () {
		// print(groundContactCount);
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
		if (ContactingGround || SnapToGround() || CheckSteepContacts()) {
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
		// If originHeight = 0, the raycast will start below the ground if it is in contact and OnCollisionStay is not working
		// Biasing ray origin upward to avoid this
		const float bias = 0.1f;
		
		if (stepsSinceLastGrounded > 1 || stepsSinceLastJump <= 2) return false;
		var detectDistance = originHeight + probeDistance + bias;
		if (!Physics.Raycast(body.position + Vector3.up * bias, Vector3.down, out var hit, detectDistance, probeMask)) return false;
		if (hit.normal.y < minGroundDotProduct) return false;

		groundContactCount = 1;
		contactNormal = hit.normal;
		var dot = Vector3.Dot(velocity, hit.normal);
		if (dot > 0f) velocity = Vector3.ProjectOnPlane(velocity, hit.normal);
		return true;
	}

	// Check if the object is stuck in terrain like crevasses
	private bool CheckSteepContacts () {
		if (steepContactCount <= 1) return false;
		steepNormal.Normalize();
		if (steepNormal.y < minGroundDotProduct) return false;
		steepContactCount = 0;
		groundContactCount = 1;
		contactNormal = steepNormal;
		return true;
	}

	// Apply velocities projected to the contact normal plane
	private void AdjustVelocity () {
		var acceleration = ContactingGround ? maxAcceleration : maxAirAcceleration;
		var maxSpeedChange = acceleration * Time.deltaTime;

		var currentProjectedVelocity = Vector3.ProjectOnPlane(velocity, contactNormal);
		var desiredProjectedVelocity = Vector3.ProjectOnPlane(desiredVelocity, contactNormal);
		desiredProjectedVelocity.Normalize();
		desiredProjectedVelocity *= desiredVelocity.magnitude;
		var newProjectedVelocity = Vector3.MoveTowards(currentProjectedVelocity, desiredProjectedVelocity, maxSpeedChange);
		
		velocity += (newProjectedVelocity - currentProjectedVelocity);
	}

	// Jump by jumpHeight 
	private void TriggerJump () {
		if (!ContactingGround) return;

		stepsSinceLastJump = 0;
		var jumpDirection = Vector3.up;
		var jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
		var alignedSpeed = Vector3.Dot(velocity, jumpDirection);
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
		var minDot = minGroundDotProduct;
		for (var i = 0; i < collision.contactCount; i++) {
			var normal = collision.GetContact(i).normal;
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
