using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public Dictionary<GameObject, bool> scoreMap = new Dictionary<GameObject, bool>();
    public GameObject inventoryUI;
    public GameObject inventoryIconUI;
    
    void Update()
    {
        // press "I" to toggle inventory UI
        if (Input.GetKeyDown(KeyCode.I))
        {
            // toggle inventory UI
            if (inventoryUI)
            {
                inventoryUI.SetActive(!inventoryUI.activeSelf);
            }
        }
        
        // Inventory Icon UI Glows when all scores are collected
        // unimplemented //
    }
    
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