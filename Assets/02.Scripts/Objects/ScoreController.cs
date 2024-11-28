using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    public void Inspect()
    {
        GameManager.im.AddScore(this.gameObject);
        gameObject.SetActive(false);
        GameManager.sm.PlaySound("picking-score");
    }
}
