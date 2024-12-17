using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class DoorControllerTests
{
    private GameObject doorControllerObject;
    private DoorController doorController;

    [SetUp]
    public void Setup()
    {
        // Create a GameObject and attach the DoorController component
        doorControllerObject = new GameObject();
        doorController = doorControllerObject.AddComponent<DoorController>();

        // Set up the scoreUIPanel
        doorController.scoreUIPanel = new GameObject();
        doorController.scoreUIPanel.AddComponent<Canvas>();
        doorController.scoreUIPanel.SetActive(false);

        // Initialize answerNotes
        doorController.answerNotes = new int[] { 1, 2, 3 }; // Example notes

        // Set up GameManager mock
        GameManagerMock im = new GameManagerMock();
        GameManager.im = im; // Assign the mock instance
    }

    [Test]
    public void Inspect_ActivatesScoreUIPanel_WhenAllScoresCollected()
    {
        // Arrange
        GameManager.im.SetAllScoresCollected(true); // Simulate all scores collected
        GameObject floatingText = new GameObject("FloatingText"); // Mock floating text

        // Act
        doorController.Inspect(floatingText);

        // Assert
        Assert.IsTrue(doorController.scoreUIPanel.activeSelf, "Score UI panel should be active when all scores are collected.");
    }

    [TearDown]
    public void Teardown()
    {
        // Clean up
        Object.DestroyImmediate(doorControllerObject);
    }
}

// Mock GameManager for testing
public class GameManagerMock
{
    public static GameManagerMock im; // Static reference for test assignment
    private bool allScoresCollected;

    public void SetAllScoresCollected(bool collected)
    {
        allScoresCollected = collected;
    }

    public bool HasAllScores()
    {
        return allScoresCollected;
    }
}

// GameManager (Static class for global state)
public static class GameManager
{
    public static GameManagerMock im;
}