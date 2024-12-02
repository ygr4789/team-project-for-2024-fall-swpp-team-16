using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInputHandler : MonoBehaviour
{
    private PlayerInput input;
    private SurfaceContactRigidbody body;

    [Range(0f, 10f)]
    [SerializeField] private float speed = 3f;
    
    private void Awake()
    {
        input = new PlayerInput();
        body = GetComponent<SurfaceContactRigidbody>();
    }

    private void Update()
    {
        GetInputs();
        ApplyVelocity();
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

    private void ApplyVelocity()
    {
        Vector3 velocity = transform.right * input.moveX + transform.forward * input.moveZ;
        velocity *= speed; 
        body.Velocity = velocity;
        body.Jump = input.jump;
    }
}
