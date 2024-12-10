using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceContactModel : MonoBehaviour
{
    public Vector3[] offsets;

    private void StickToGround()
    {
        float groundHeight = 0f;
        Vector3 groundNormal = Vector3.up * 0.01f;
        foreach (Vector3 offset in offsets)
        {
            Vector3 rayOrigin = transform.position + transform.TransformDirection(offset) + transform.up;
            Ray ray = new Ray(rayOrigin, Vector3.down);
            Debug.DrawRay(rayOrigin, Vector3.down, Color.green);
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                groundHeight += hit.point.y;
                groundNormal += hit.normal;
            }

            gameObject.layer = LayerMask.NameToLayer("Default");
        }

        groundHeight /= offsets.Length;
        groundNormal.Normalize();
        Vector3 groundPosition = transform.position;
        groundPosition.y = groundHeight;
        transform.position = groundPosition;

        Vector3 flattenedForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
        float u = -Vector3.Dot(flattenedForward, groundNormal);
        float f = Vector3.Dot(Vector3.up, groundNormal);
        Vector3 forward = (flattenedForward * f + Vector3.up * u).normalized;

        transform.rotation = Quaternion.LookRotation(forward, groundNormal);
    }
}