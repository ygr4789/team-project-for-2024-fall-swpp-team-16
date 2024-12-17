using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue System/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [Serializable]
    public class DialogueEntry
    {
        public string sceneName = ""; // 씬 이름
        public string action = ""; // 특정 행동
        [TextArea] public string dialogue = ""; // 대화 내용
        public int characterImageId = 0; // 사용할 캐릭터 이미지 ID (0-3)
        public bool isCallable = true;
    }

    public DialogueEntry[] dialogueEntries; // 대화 리스트
}
