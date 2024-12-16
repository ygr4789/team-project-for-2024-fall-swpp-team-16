using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        // 메인 카메라 참조
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Billboard: Main Camera not found!");
        }
    }

    void LateUpdate()
    {
        if (mainCamera != null)
        {
            // 항상 카메라를 바라보도록 설정
            Vector3 cameraForward = mainCamera.transform.forward;
            transform.rotation = Quaternion.LookRotation(cameraForward, Vector3.up);
        }
    }
}