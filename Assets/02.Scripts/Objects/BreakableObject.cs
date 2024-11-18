using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    private BoxCollider _triggerCollider;

    void Start()
    {
        _triggerCollider = gameObject.AddComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<RockController>())
        {
            Debug.Log("Enter" + other.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<RockController>())
        {
            Debug.Log("Exit" + other.name);
        }
    }
}
