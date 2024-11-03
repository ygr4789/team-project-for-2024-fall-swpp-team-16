using UnityEngine;

public class BearMovementController : MonoBehaviour
{
    public Animator animator;
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float currentSpeed = 0f;

    private bool isRunning = false;

    void Update()
    {
        HandleInput();
        MoveBear();
    }

    void HandleInput()
    {
        // Toggle running with the "Left Shift" key
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isRunning = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isRunning = false;
        }

        // Example: Move forward with "W" key
        if (Input.GetKey(KeyCode.W))
        {
            if (isRunning)
                currentSpeed = Mathf.Lerp(currentSpeed, runSpeed, Time.deltaTime * 5f);
            else
                currentSpeed = Mathf.Lerp(currentSpeed, walkSpeed, Time.deltaTime * 5f);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0f, Time.deltaTime * 5f);
        }

        // Update Animator parameters
        animator.SetFloat("Speed", currentSpeed);
        animator.SetBool("IsRunning", isRunning);
    }

    void MoveBear()
    {
        // Move the bear forward based on currentSpeed
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }
}
