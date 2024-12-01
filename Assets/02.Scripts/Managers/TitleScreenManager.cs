using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    public string gameSceneName = "StageScene"; // Name of the main game scene

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) // Detect Enter key press
        {
            StartGame();
        }
    }

    void StartGame()
    {
        SceneManager.LoadScene(gameSceneName); // Load the main game scene
    }
}
