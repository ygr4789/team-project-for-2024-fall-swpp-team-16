using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private string action = ""; // Action 이름
    [SerializeField] private float triggerDistance = 5f; // Trigger 반응 거리
    [SerializeField] private bool isMutipleCallable = true; // 여러 번 호출 가능한지 여부
    private Transform playerTransform; // 플레이어 Transform
    private DialogueManager dialogueManager; // DialogueManager 참조
    private bool hasTriggered = false; // 트리거 실행 여부 관리
    private bool wasPlayerInRange = false; // 이전 프레임에서 플레이어가 범위 안에 있었는지

    private void Start()
    {
        // 플레이어와 DialogueManager를 동적으로 찾음
        playerTransform = GameManager.gm.controller.transform;
        dialogueManager = FindObjectOfType<DialogueManager>();

        if (playerTransform == null)
        {
            Debug.LogError("Player not found! Make sure the player has the 'Player' tag.");
        }

        if (dialogueManager == null)
        {
            Debug.LogError("DialogueManager not found! Make sure it exists in the scene.");
        }
        else
        {
            // DialogueManager의 OnDialogueEnd 이벤트 구독
            dialogueManager.OnDialogueEnd += ResetTrigger;
        }
    }

    private void Update()
    {
        if (playerTransform == null || dialogueManager == null) return;

        // 현재 프레임에서 플레이어와의 거리 계산
        float distance = Vector3.Distance(playerTransform.position, transform.position);
        bool isPlayerInRange = distance <= triggerDistance;

        // 이전 프레임에서 범위 밖에 있다가 현재 범위 안으로 들어왔을 때만 실행
        if (isPlayerInRange && !wasPlayerInRange && !hasTriggered)
        {
            TriggerDialogue();
        }

        // 현재 상태를 저장하여 다음 프레임에서 비교
        wasPlayerInRange = isPlayerInRange;
    }

    private void TriggerDialogue()
    {
        // DialogueManager를 통해 대화 시작
        dialogueManager.StartDialogue(SceneManager.GetActiveScene().name, action, isMutipleCallable);

        // 트리거 실행 여부 업데이트
        if (!isMutipleCallable)
        {
            hasTriggered = true;
        }
    }

    private void ResetTrigger()
    {
        if (isMutipleCallable)
        {
            hasTriggered = false; // 대화 종료 시 트리거 재활성화
        }
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if (dialogueManager != null)
        {
            dialogueManager.OnDialogueEnd -= ResetTrigger;
        }
    }

    private void OnDrawGizmos()
    {
        // Trigger 범위를 시각적으로 표시
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, triggerDistance);
    }
}
