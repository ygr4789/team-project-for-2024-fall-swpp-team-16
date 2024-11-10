using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
	public List<GameObject> scores = new List<GameObject>(); // list of scores to collect
    public Dictionary<GameObject, bool> scoreMap = new Dictionary<GameObject, bool>(); // map of scores and their collection status
    public GameObject inventoryUI;
    public GameObject inventoryIconUI;

	void Start()
    {
        // initialize scoreMap
        foreach (var score in scores)
        {
            scoreMap[score] = false;
			Debug.Log("Score: " + score.name);
        }
    }
    
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
        if (scoreMap.ContainsKey(target))
        {
            scoreMap[target] = true;
        }
		else {
	        Debug.Log("Score is not in the list: " + target.name);
		}
    }
    
    public bool HasAllScores()
    {
        foreach (var hasTheScore in scoreMap.Values)
        {
            if (!hasTheScore)
            {
                return false;
            }
        }
        return true;
    }
}