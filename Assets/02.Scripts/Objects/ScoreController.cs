using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    public void Inspect(GameObject floatingText)
    {
        GameManager.im.AddScore(this.gameObject);
        gameObject.SetActive(false);
        floatingText.SendMessage("Hide");
        GameManager.sm.PlaySound("picking-score");
    }
}
