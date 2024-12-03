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

    private bool TryGetInteractable(Transform target, out Interactable interactable)
    {
        if (target.CompareTag("Padding")) target = target.parent;
        return target.TryGetComponent(out interactable);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (TryGetInteractable(other.transform, out var interactable))
        {
            _interactable.InteractWith(interactable);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (TryGetInteractable(other.transform, out var interactable))
        {
            _interactable.InteractWith(interactable);
        }
    }
}
