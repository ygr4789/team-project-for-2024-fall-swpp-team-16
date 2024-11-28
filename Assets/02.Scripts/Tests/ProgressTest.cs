using NUnit.Framework;
using System.IO;
using UnityEngine;
using UnityEngine.TestTools; // Include this namespace for Play Mode tests
using System.Collections;

[TestFixture]
public class StageManagerTests
{
    private StageManager stageManager;
    private string testFilePath;

    [SetUp]
    public void SetUp()
    {
        GameObject gameObject = new GameObject();
        stageManager = gameObject.AddComponent<StageManager>();
        stageManager.totalStages = 5;
        testFilePath = Path.Combine(Application.persistentDataPath, "progress.txt");
        if (File.Exists(testFilePath))
        {
            File.Delete(testFilePath);
        }
    }

    [TearDown]
    public void TearDown()
    {
        if (File.Exists(testFilePath))
        {
            File.Delete(testFilePath);
        }
    }

    [UnityTest] // Use UnityTest for Play Mode tests
    public IEnumerator TestLoadStages_NoFile_CreatesNewFile()
    {
        stageManager.Awake();
        yield return null; // Wait a frame if necessary
        Assert.IsTrue(File.Exists(testFilePath));
    }

    [UnityTest]
    public IEnumerator TestCompleteStage_UpdatesAccomplishedStages()
    {
        stageManager.Awake();
        stageManager.CompleteStage(1);
        yield return null;
        Assert.IsTrue(stageManager.IsStageAccomplished(1));
    }

    [UnityTest]
    public IEnumerator TestGetStagesStatus_ReturnsCorrectStatus()
    {
        stageManager.Awake();
        stageManager.CompleteStage(1);
        stageManager.CompleteStage(3);
        yield return null;
        bool[] status = stageManager.GetStagesStatus();
        Assert.IsTrue(status[0]);
        Assert.IsFalse(status[1]);
        Assert.IsTrue(status[2]);
        Assert.IsFalse(status[3]);
        Assert.IsFalse(status[4]);
    }
}