using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class Stage1Test
{
    [SetUp]
    public void Setup()
    {
        // Load the scene
        string sceneName = "Stage1";
        SceneManager.LoadScene(sceneName);
    }

    [TearDown]
    public void Teardown()
    {

    }

    [UnityTest]
    public IEnumerator TestSettingsModal()
    {
        yield return null;

        var canvas = GameObject.Find("Canvas");
        Assert.IsNotNull(canvas, "Canvas not found in the scene.");

        var settingsModal = canvas.transform.Find("SettingsModal")?.gameObject;
        Assert.IsNotNull(settingsModal, "SettingsModal not found under Canvas.");
        Assert.IsFalse(settingsModal.activeSelf, "SettingsModal should start inactive.");

        // Input.GetKeyDown(KeyCode.Escape);
        // yield return null;
        // Assert.IsTrue(settingsModal.activeSelf, "SettingsModal should be active after ESC key is pressed.");
    }

    [UnityTest]
    public IEnumerator TestScoreCountInit()
    {
        yield return null;

        Assert.IsFalse(GameManager.im.HasAllScores(), "All scores are not collected yet!");
    }
}