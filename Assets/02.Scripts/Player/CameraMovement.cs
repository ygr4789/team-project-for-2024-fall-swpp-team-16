using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float rotationSpeed = 45f;
    private float rotationAngle = 0f;
    
    private void Start()
    {
        // locks cursor and makes it invisible
        Cursor.lockState = Cursor.lockState;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleRotationInput();
        RotateCamera();
    }

    private void HandleRotationInput()
    {
        rotationAngle = 0f;
        if (Input.GetKey(KeyCode.E))
        {
            rotationAngle = rotationSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            rotationAngle = -rotationSpeed * Time.deltaTime;
        }
    }

    private void RotateCamera()
    {
        if (!transform.parent) return;
        var parentTransform = transform.parent;
        var rotationAxis = parentTransform.up;
        transform.RotateAround(parentTransform.position, rotationAxis, rotationAngle);
    }
    
    public void SetCursorVisible()
    {
        Cursor.visible = true;
    }
    
    public void SetCursorInvisible()
    {
        Cursor.visible = false;
    }
}
