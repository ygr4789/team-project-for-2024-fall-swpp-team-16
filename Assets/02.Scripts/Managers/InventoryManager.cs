using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager im;
    
    public Dictionary<GameObject, bool> scoreMap = new Dictionary<GameObject, bool>();
    
    public void AddScore(GameObject target)
    {
        Debug.Assert(target.tag == "Score", "Target must have the tag 'Score'");
        if (!scoreMap.ContainsKey(target))
        {
            scoreMap[target] = true;
        }
    }
    
    public bool HasAllScores()
    {
        foreach (var score in scoreMap.Values)
        {
            if (!score)
            {
                return false;
            }
        }
        return true;
    }
}