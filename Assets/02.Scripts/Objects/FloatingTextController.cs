using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingTextController : MonoBehaviour
{
    public GameObject target;
    public Vector3 offset = new Vector3(0, 3, 0);
    public string inspectGuideText = "Press E to inspect";
    
    void Start()
    {
        if (target == null)
        {
            Debug.LogError("FloatingTextController: target object is not set!");
        }
        transform.position = target.transform.position + offset;
        
        TextMeshPro textMeshPro = GetComponent<TextMeshPro>();
        if (textMeshPro != null)
        {
            textMeshPro.text = inspectGuideText;
        }
    }
    
    void Update()
    {
        // if E is pressed, trigger the inspect function of the target object
        if (Input.GetKeyDown(KeyCode.E))
        {
            target.SendMessage("Inspect");
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
