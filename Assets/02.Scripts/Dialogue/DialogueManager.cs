using UnityEngine;
using System;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public event Action OnDialogueEnd; // 대화 종료 이벤트

    [SerializeField] private DialogueData dialogueData; // ScriptableObject 대화 데이터
    [SerializeField] private GameObject dialogueUI; // 대화 UI 패널
    [SerializeField] private TextMeshProUGUI dialogueText; // 대화 텍스트 UI
    [SerializeField] private Image characterImage; // 캐릭터 이미지 UI
    [SerializeField] private Sprite[] characterSprites; // 캐릭터 이미지 배열 (ID 0-3)
    [SerializeField] private float waitTimeAfterDialogue = 2f; // 다음 대화로 넘어가기 전 대기 시간

    private Queue<DialogueData.DialogueEntry> currentDialogues; // 현재 재생 중인 대화 큐
    private bool isDialogueActive = false; // 대화 진행 상태 여부
    private float dialogueTimer = 0f; // 대화 경과 시간

    private void Start()
    {
        dialogueUI.SetActive(false); // 초기 상태: UI 비활성화
        currentDialogues = new Queue<DialogueData.DialogueEntry>();
    }

    private void Update()
    {
        if (isDialogueActive)
        {
            dialogueTimer += Time.deltaTime;

            if (dialogueTimer >= waitTimeAfterDialogue || Input.GetKeyDown(KeyCode.Return)) // 시간 초과 또는 Enter 입력
            {
                PlayNextDialogue();
            }
        }
    }

    public void StartDialogue(string sceneName, string action, bool isMultipleCallable)
    {
        if (isDialogueActive)
        {
            Debug.LogWarning("A dialogue is already active. Ignoring this request.");
            return; // 대화 중이면 새로운 대화 시작을 무시
        }

        // 현재 대화 초기화
        currentDialogues.Clear();

        // 주어진 SceneName과 Action에 맞는 대화 추가
        foreach (var entry in dialogueData.dialogueEntries)
        {
            if (entry.sceneName == sceneName && entry.action == action && entry.isCallable)
            {
                currentDialogues.Enqueue(entry);

                // 한 번 호출되면 isCallable을 isMultipleCallable로 설정
                entry.isCallable = isMultipleCallable;
            }
        }

        if (currentDialogues.Count > 0)
        {
            // UI 활성화
            dialogueUI.SetActive(true);
            isDialogueActive = true;

            // 첫 번째 대화 출력 시작
            PlayNextDialogue();
        }
        else
        {
            Debug.LogWarning("No dialogues found for the given sceneName and action.");
        }
    }

    private void PlayNextDialogue()
    {
        if (currentDialogues.Count > 0)
        {
            dialogueTimer = 0f; // 타이머 초기화
            var dialogueEntry = currentDialogues.Dequeue();
            DisplayDialogue(dialogueEntry);
        }
        else
        {
            EndDialogue();
        }
    }

    private Coroutine typingCoroutine; // 타이핑 효과 코루틴

    private void DisplayDialogue(DialogueData.DialogueEntry entry)
    {
        // 캐릭터 이미지 설정
        if (entry.characterImageId >= 0 && entry.characterImageId < characterSprites.Length)
        {
            characterImage.sprite = characterSprites[entry.characterImageId];
            characterImage.gameObject.SetActive(true);
        }
        else
        {
            characterImage.gameObject.SetActive(false); // 유효하지 않은 ID라면 이미지 숨김
        }

        // 기존 코루틴 중단
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        // 새로운 대사에 대한 타이핑 효과 시작
        typingCoroutine = StartCoroutine(TypeDialogue(entry.dialogue));
    }

    private IEnumerator TypeDialogue(string dialogue)
    {
        dialogueText.text = ""; // 초기화
        foreach (char letter in dialogue)
        {
            dialogueText.text += letter; // 한 글자씩 추가
            yield return new WaitForSeconds(0.05f); // 타이핑 속도 조절
        }
    }

    private void EndDialogue()
    {
        // 모든 대화가 끝난 후 처리
        dialogueUI.SetActive(false);
        dialogueText.text = ""; // 텍스트 초기화
        characterImage.sprite = null; // 캐릭터 이미지 초기화
        isDialogueActive = false; // 대화 상태 비활성화

        Debug.Log("Dialogue ended.");
        OnDialogueEnd?.Invoke();
    }
}
