using NUnit.Framework;
using UnityEngine;

public class DoorControllerCheckNotesTest
{
    private GameObject doorControllerObject;
    private DoorController doorController;

    [SetUp]
    public void Setup()
    {
        doorControllerObject = new GameObject();
        doorController = doorControllerObject.AddComponent<DoorController>();

        // Initialize answerNotes for testing
        doorController.answerNotes = new int[] { 1, 2, 3 };
    }

    [Test]
    public void CheckNotes_ReturnsTrue_WhenNotesMatch()
    {
        // Arrange
        doorController.playedNotes.Add(1);
        doorController.playedNotes.Add(2);
        doorController.playedNotes.Add(3);

        // Act
        bool result = doorController.CheckNotes();

        // Assert
        Assert.IsTrue(result, "CheckNotes should return true when played notes match answer notes.");
    }

    [Test]
    public void CheckNotes_ReturnsFalse_WhenNotesDoNotMatch()
    {
        // Arrange
        doorController.playedNotes.Add(1);
        doorController.playedNotes.Add(2);
        doorController.playedNotes.Add(4); // Incorrect note

        // Act
        bool result = doorController.CheckNotes();

        // Assert
        Assert.IsFalse(result, "CheckNotes should return false when played notes do not match answer notes.");
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(doorControllerObject);
    }
}