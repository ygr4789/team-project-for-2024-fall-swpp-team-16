using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private void Start()
    {
        // locks cursor and makes it invisible
        Cursor.lockState = Cursor.lockState;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
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
