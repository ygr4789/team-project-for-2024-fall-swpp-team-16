using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class Stage4Test
{
    [SetUp]
    public void Setup()
    {
        // Load the scene
        string sceneName = "Stage4";
        SceneManager.LoadScene(sceneName);
    }

    [TearDown]
    public void Teardown()
    {

    }

    [UnityTest]
    public IEnumerator TestWater_PlayerDiesAndRespawn()
    {
        yield return null;

        GameObject player = GameObject.Find("Player");
        Vector3 waterPosition = new Vector3(0, 0.5f, 0);
        
        // player falls into water
        player.transform.position = waterPosition;

        yield return new WaitForSeconds(1);

        // player transform should be respawned
        Assert.AreNotEqual(waterPosition, player.transform.position);
    }
}