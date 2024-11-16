using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
	public List<GameObject> scores = new List<GameObject>(); // list of scores to collect
    public Dictionary<GameObject, bool> scoreMap = new Dictionary<GameObject, bool>(); // map of scores and their collection status
    public GameObject inventoryUI;
    public GameObject inventoryIconUI;
    public TMPro.TextMeshProUGUI scoreText;

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
        // press "I" to toggle inventory UI <- This is not necessary
        // if (Input.GetKeyDown(KeyCode.I))
        // {
        //     // toggle inventory UI
        //     if (inventoryUI)
        //     {
        //         inventoryUI.SetActive(!inventoryUI.activeSelf);
        //     }
        // }
        
        
        
        // update inventory UI text
        if (scoreText != null)
        {
            // count collected scores
            int collectedScores = 0;
            foreach (var hasTheScore in scoreMap.Values)
            {
                if (hasTheScore) collectedScores++;
            }
            scoreText.text = $"{collectedScores}/{scores.Count}";
            
            // change text color to yellow if all scores are collected
            if (HasAllScores())
            {
                scoreText.color = Color.yellow;
            }
            else
            {
                scoreText.color = Color.white;
            }
        }
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