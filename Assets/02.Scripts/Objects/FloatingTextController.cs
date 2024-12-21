using System;
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
    private bool isPlayerNear = false;
    private TextMeshPro fText;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("FloatingTextController: target object is not set!");
        }

        // 텍스트 설정
        fText = transform.GetComponent<TextMeshPro>();
        if (fText != null)
        {
            fText.text = "";
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
            UpdateTarget();
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

    private void UpdateTarget()
    {
        Collider[] cols = Physics.OverlapSphere(target.transform.position, inspectMaxDistance, 1 << LayerMask.NameToLayer("Player"));

        if(cols.Length > 0 && !isPlayerNear)
        {
            fText.text = inspectGuideText;
            isPlayerNear = true;
        } else if (cols.Length <= 0 && isPlayerNear)
        {
            fText.text = "";
            isPlayerNear = false;
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