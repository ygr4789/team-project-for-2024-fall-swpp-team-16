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
    
    void Start()
    {
        if (target == null)
        {
            Debug.LogError("FloatingTextController: target object is not set!");
        }
        
        TextMeshPro textMeshPro = GetComponent<TextMeshPro>();
        if (textMeshPro != null)
        {
            textMeshPro.text = inspectGuideText;
        }
    }
    
    void Update()
    {
        // move the floating text to the target object's position
        transform.position = target.transform.position + offset;
        
        // if E is pressed, trigger the inspect function of the target object
        if (Input.GetKeyDown(KeyCode.E))
        {
            // if distance between player and target is less than 3, trigger inspect
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
