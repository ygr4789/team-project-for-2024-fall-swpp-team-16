using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingTextController : MonoBehaviour
{
    public GameObject target;
    public Vector3 offset = new Vector3(0, 3, 0);
    public string inspectGuideText = "Press [E]";
    public float inspectMaxDistance = 3;
    private Camera mainCamera;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("FloatingTextController: target object is not set!");
        }

        mainCamera = Camera.main; // 메인 카메라 참조
        if (mainCamera == null)
        {
            Debug.LogError("FloatingTextController: Main Camera not found!");
        }

        // 텍스트 설정
        TextMeshProUGUI textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
        if (textMeshPro != null)
        {
            textMeshPro.text = inspectGuideText;
        }

        // 초기 위치 설정
        if (target != null)
        {
            transform.position = target.transform.position + offset;
        }
    }

    void LateUpdate()
    {
        // 위치는 target 기준으로 고정
        if (target != null)
        {
            transform.position = target.transform.position + offset;
        }

        // 항상 카메라를 향하도록 회전
        if (mainCamera != null)
        {
            Vector3 cameraForward = mainCamera.transform.forward;
            transform.rotation = Quaternion.LookRotation(cameraForward, Vector3.up);
        }

        // if E is pressed, trigger the inspect function of the target object
        if (Input.GetKeyDown(KeyCode.E))
        {
            // if distance between player and target is less than inspectMaxDistance, trigger inspect
            Transform playerTransform = GameManager.pm.playerTransform;
            if (Vector3.Distance(target.transform.position, playerTransform.position) <
                inspectMaxDistance)
            {
                target.SendMessage("Inspect", gameObject);
            }
        }
    }

    // triggered when "inspect" action is performed
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
