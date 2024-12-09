using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorTargeter : MonoBehaviour
{
    private PlayManager playManager;
    
    void Start()
    {
        playManager = GameManager.pm;
    }

    void Update()
    {
        if (playManager.currentTarget)
        {
            Debug.DrawRay(playManager.currentTarget.transform.position, Vector3.up);
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            Ray cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(cursorRay, out RaycastHit hit))
            {
                if (hit.transform.TryGetComponent<ResonatableObject>(out _))
                {
                    playManager.currentTarget = hit.transform;
                }
            }
        }
    }
}
