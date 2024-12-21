using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class StageTutorialPlateTest
{
    [SetUp]
    public void Setup()
    {
        // Load the scene
        string sceneName = "Tutorial Plate";
        SceneManager.LoadScene(sceneName);
    }

    [TearDown]
    public void Teardown()
    {

    }

    [UnityTest]
    public IEnumerator TestTree_GrowWhenResonate()
    {
        yield return null;

        GameObject tree = GameObject.Find("Tree");
        Assert.IsNotNull(tree, "Tree not found in the scene.");

        Debug.Log("tree: " + tree);

        TreeController treeController = tree.GetComponent<TreeController>();
        float y1 = treeController.currentHeight;

        Debug.Log("y1: " + y1);
        
        float interval = 0.2f; // Set the interval for triggering resonance
        float duration = 1f; // Total duration for triggering resonance

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            tree.GetComponent<ResonatableObject>().resonate(PitchType.So);
            elapsedTime += interval;
            yield return new WaitForSeconds(interval);
        }
        
        float y2 = treeController.currentHeight;

        Debug.Log("y2: " + y2);

        Assert.IsTrue(y2 > y1);

        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            tree.GetComponent<ResonatableObject>().resonate(PitchType.La);
            elapsedTime += interval;
            yield return new WaitForSeconds(interval);
        }
        
        float y3 = treeController.currentHeight;

        Debug.Log("y3: " + y3);

        Assert.IsTrue(y3 < y2);
    }
}