using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionHandler : MonoBehaviour
{
    private Interactable _interactable;

    private void Awake()
    {
        _interactable = GetComponent<Interactable>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        var other = collision.gameObject.GetComponent<Interactable>();
        if (other != null)
        {
            _interactable.InteractWith(other);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Interactable>(out var interactable))
        {
            _interactable.InteractWith(interactable);
        }
    }
}
