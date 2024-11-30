using System;
using System.Collections;
using UnityEngine;

public class SurfaceContactRigidbody : MonoBehaviour
{
	[Tooltip("Movable terrain slope limits")]
	[SerializeField, Range(0, 90)]
	private float maxGroundAngle = 25f;
	[Tooltip("Ground detection distance")]
	[SerializeField, Min(0f)]
	private float probeDistance = 1f;
	[Tooltip("Ground layer mask")]
	[SerializeField]
	private LayerMask probeMask = -1;
	
	private Rigidbody body;
	private Vector3 velocity, currentVelocity; // desired velocity, internally calculated velocity
	private Vector3 contactNormal, steepNormal; // average normal of contacts
	private int groundContactCount, steepContactCount; // total number of contacts

	private bool OnGround => groundContactCount > 0;
	private bool OnSteep => steepContactCount > 1;
	private float minGroundDotProduct; // minimum y-component of movable normal
	private int stepsSinceLastGrounded; // number of frames since last grounded 

	public Vector3 Velocity
	{
		get => body.velocity;
		set
		{
			Vector3 flattenVelocity = value;
			flattenVelocity.y = 0f;
			velocity = flattenVelocity;
		}
	}
	
	private void OnValidate ()
	{
		minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
	}

	private void Awake ()
	{
		body = GetComponent<Rigidbody>();
		OnValidate();
	}
	
	private void FixedUpdate () 
	{
		UpdateState();
		AdjustVelocity();
		body.velocity = currentVelocity;
		ClearState();
	}

	// Initialize properties
	private void ClearState () 
	{
		groundContactCount = steepContactCount = 0;
		contactNormal = steepNormal = Vector3.zero;
	}

	// Calculate all properties
	private void UpdateState () 
	{
		stepsSinceLastGrounded += 1;
		currentVelocity = body.velocity;
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
	bool SnapToGround () 
	{
		if (stepsSinceLastGrounded > 1) return false;
		if (!Physics.Raycast(body.position, Vector3.down, out RaycastHit hit, probeDistance, probeMask)) return false;
		if (hit.normal.y < minGroundDotProduct) return false;

		groundContactCount = 1;
		contactNormal = hit.normal;
		float dot = Vector3.Dot(currentVelocity, hit.normal);
		if (dot > 0f) currentVelocity = (currentVelocity - hit.normal * dot).normalized * currentVelocity.magnitude;
		return true;
	}

	// Check if the object is stuck in terrain like crevasses
	bool CheckSteepContacts () 
	{
		if (OnSteep)
		{
			steepNormal.Normalize();
			if (steepNormal.y >= minGroundDotProduct)
			{
				steepContactCount = 0;
				groundContactCount = 1;
				contactNormal = steepNormal;
				return true;
			}
		}
		return false;
	}

	// Apply velocities projected in the normal plane only for the xz plane
	private void AdjustVelocity ()
	{
		Vector3 xAxis = ProjectOnContactPlane(Vector3.right).normalized;
		Vector3 zAxis = ProjectOnContactPlane(Vector3.forward).normalized;
		float currentX = Vector3.Dot(currentVelocity, xAxis);
		float currentZ = Vector3.Dot(currentVelocity, zAxis);
		currentVelocity += xAxis * (velocity.x - currentX) + zAxis * (velocity.z - currentZ);
	}

	private Vector3 ProjectOnContactPlane (Vector3 vector)
	{
		return vector - contactNormal * Vector3.Dot(vector, contactNormal);
	}
	
	private void OnCollisionEnter (Collision collision)
	{
		EvaluateCollision(collision);
	}

	private void OnCollisionStay (Collision collision)
	{
		EvaluateCollision(collision);
	}

	// Record collision counts and contact normal
	private void EvaluateCollision (Collision collision)
	{
		float minDot = minGroundDotProduct;
		for (int i = 0; i < collision.contactCount; i++)
		{
			Vector3 normal = collision.GetContact(i).normal;
			if (normal.y >= minDot)
			{
				groundContactCount += 1;
				contactNormal += normal;
			}
			else if (normal.y > -0.01f)
			{
				steepContactCount += 1;
				steepNormal += normal;
			}
		}
	}
}
