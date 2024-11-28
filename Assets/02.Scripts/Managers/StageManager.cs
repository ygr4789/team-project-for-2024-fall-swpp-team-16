using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public int totalStages = 10; // Set this to the total number of stages
    
    private string filePath;
    private bool[] accomplishedStages;
    private string fileName = "progress.txt"; // file to save the stage progress data
    
    public void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, fileName);
        Debug.Log("progress file path: " + filePath);
        LoadStages();
    }

    private void LoadStages()
    {
        accomplishedStages = new bool[totalStages];

        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                if (int.TryParse(line, out int stage) && stage > 0 && stage <= totalStages)
                {
                    accomplishedStages[stage - 1] = true;
                }
            }
        } else
        {
            Debug.Log("No progress file found. Creating a new one.");
            SaveStages();
        }
    }
    
    public void CompleteStage(int stage)
    {
        if (stage > 0 && stage <= totalStages && !accomplishedStages[stage - 1])
        {
            accomplishedStages[stage - 1] = true;
            SaveStages();
        }
    }

    private void SaveStages()
    {
        File.WriteAllLines(filePath, accomplishedStages
            .Select((accomplished, index) => accomplished ? (index + 1).ToString() : null)
            .Where(stage => stage != null)
            .ToArray());
    }

    public bool IsStageAccomplished(int stage)
    {
        return stage > 0 && stage <= totalStages && accomplishedStages[stage - 1];
    }

    public bool[] GetStagesStatus()
    {
        return accomplishedStages;
    }
}