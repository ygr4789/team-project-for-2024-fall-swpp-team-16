using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;

public class DoorController : MonoBehaviour
{
    public List<GameObject> scores = new List<GameObject>(); // list of scores to collect
	public GameObject scoreUIPanel; // Reference to the score UI panel
    public int[] answerNotes; // Array to store notes for this door
    public List<int> playedNotes = new List<int>(); // List to store played notes
    public GameObject doorWing; // Reference to the door object
    
    public GameObject rippleEffectPrefab; // Reference to the ripple effect prefab
    public GameObject[] coloredNotes; // Array to store note prefabs

    void Start()
    {
        scoreUIPanel.SetActive(false);
    }

    public void Inspect(GameObject floatingText)
    {
        // if all scores are collected, the user needs to play music
		if (GameManager.im.HasAllScores())
        {
            // Activate the Score UI to display it
            scoreUIPanel.SetActive(true);
            ClearColoredNotes();
			
			Debug.Log("Play the music to open the door");
            floatingText.SendMessage("Hide");
        }
        else
        {
            Debug.Log("You need to collect all the scores to open the door");
        }
    }

    void Update()
    {
        // Playing notes
        if (scoreUIPanel.activeSelf)
        {
            for (int i = 1; i <= 8; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + (i - 1)))
                {
                    playedNotes.Add(i);

                    if (CheckNotes())
                    {
                        // 색 음표 생성
                        SpawnColoredNote(i);
                        
                        // 정답이라면 잠깐의 딜레이 후 문 열기
                        if (playedNotes.Count == answerNotes.Length)
                        {
                            Debug.Log("Correct notes played. Door is opening.");
                            GameManager.stm.CompleteCurrentStage();
                            StartCoroutine(OpenDoor());
                            playedNotes.Clear();
                            // camera effect
                            GameManager.em.FadeOutCircleTransition();
                            // Go Back to the stage selection scene
                            StartCoroutine(GameManager.stm.WaitAndLoadScene("StageScene"));
                        }
                    }
                    else
                    {
                        Debug.Log("Incorrect notes played. Try again.");
                        GameManager.sm.PlaySound("wrong-answer");
                        playedNotes.Clear();
                        ClearColoredNotes();
                    }
                }
            }
        }
    }
    
    private void SpawnColoredNote(int note)
    {
        if (playedNotes.Count > coloredNotes.Length)
        {
            Debug.LogError("Not enough colored notes to spawn. Add more colored notes to the coloredNotes array.");
            return;
        }
        // 색 음표 생성 (색 음표 배열이 있고 그게 뿅 하는 효과와 함께 나타남)
        GameObject notePrefab = coloredNotes[playedNotes.Count - 1];
        notePrefab.SetActive(true);
        
        // Create the small effect at the note's position
        GameObject spawnNoteEffect = Instantiate(rippleEffectPrefab, notePrefab.transform.position + new Vector3(0, 0, -1), Quaternion.identity);
        spawnNoteEffect.transform.SetParent(notePrefab.transform);
        spawnNoteEffect.transform.localScale = Vector3.one * 20; // Ensure the correct initial scale
        // Destroy the particle effect after a certain duration
        Destroy(spawnNoteEffect, 1.0f);
        
        // Start the scaling animation for both the note and the small effect
        StartCoroutine(ScaleEffect(notePrefab.transform));
    }
    
    private IEnumerator ScaleEffect(Transform targetTransform, bool isEffect = false)
    {
        float duration = 0.15f; // Duration of the scaling effect
        Vector3 initialScale = Vector3.zero; // Start as a small dot
        Vector3 finalScale = Vector3.one; // Final size

        float time = 0;
        while (time < duration)
        {
            targetTransform.localScale = Vector3.Lerp(initialScale, finalScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        targetTransform.localScale = finalScale;
    }
    
    private void ClearColoredNotes()
    {
        // 색 음표 제거
        foreach (GameObject notePrefab in coloredNotes)
        {
            notePrefab.SetActive(false);
        }
    }

    private bool CheckNotes()
    {
        for (int i = 0; i < playedNotes.Count; i++)
        {
            if (playedNotes[i] != answerNotes[i])
            {
                return false;
            }
        }
        return true;
    }        
    
	private IEnumerator OpenDoor()
    {
        // delay
        yield return new WaitForSeconds(1.5f);
        
        // UI off
        scoreUIPanel.SetActive(false);
        
       	// opening door sound & animation
		GameManager.sm.PlaySound("opening-door");
        StartCoroutine(OpenDoorAnimation(doorWing));
    }
    
    private IEnumerator OpenDoorAnimation(GameObject door)
    {
        float duration = 1f; // animation duration
        float startRotation = door.transform.eulerAngles.y;
        float endRotation = startRotation + 90;
        float time = 0;

        while (time < duration)
        {
            float yRotation = Mathf.Lerp(startRotation, endRotation, time / duration);
            door.transform.eulerAngles = new Vector3(door.transform.eulerAngles.x, yRotation, door.transform.eulerAngles.z);
            time += Time.deltaTime;
            yield return null;
        }

        door.transform.eulerAngles = new Vector3(door.transform.eulerAngles.x, endRotation, door.transform.eulerAngles.z);
    }
}
