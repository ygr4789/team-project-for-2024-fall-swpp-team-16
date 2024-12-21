using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInputHandler : MonoBehaviour
{
    private PlayerInput input;
    private SurfaceContactController body;

    [Range(0f, 10f)]
    [SerializeField] private float speed = 3f;
    
    private void Awake()
    {
        input = new PlayerInput();
        body = GetComponent<SurfaceContactController>();
    }

    private void Update()
    {
        GetInputs();
        ApplyVelocity();
    }
    
    private void GetInputs()
    {
        input.Clear();
        if(!input.Active) return;
        
        // get the movement input
        input.MoveX = Input.GetAxis("Horizontal");
        input.MoveZ = Input.GetAxis("Vertical");
        input.Jump = Input.GetButton("Jump");
        input.Run = Input.GetKey(KeyCode.LeftShift);
    }

    private void ApplyVelocity()
    {
        Vector3 velocity = transform.right * input.MoveX + transform.forward * input.MoveZ;
        velocity *= speed; 
        body.Velocity = velocity;
        body.Jump = input.Jump;
    }
}
