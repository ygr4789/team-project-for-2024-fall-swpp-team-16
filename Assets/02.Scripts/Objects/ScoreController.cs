using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Inspect()
    {
        InventoryManager.im.AddScore(gameObject);
        gameObject.SetActive(false);
    }
}
