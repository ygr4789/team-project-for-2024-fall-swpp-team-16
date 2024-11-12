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
        GameManager.im.AddScore(this.gameObject);
        gameObject.SetActive(false);
        GameManager.sm.PlaySound("picking-score");
    }
}
