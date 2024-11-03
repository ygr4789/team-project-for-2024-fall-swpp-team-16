using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject target;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 100f))
            {
                if (raycastHit.transform)
                {
                    //Our custom method.
                    Cutter.Cut(raycastHit.transform.gameObject, raycastHit.point, Vector3.up);
                }
            }
        }
    }
}
