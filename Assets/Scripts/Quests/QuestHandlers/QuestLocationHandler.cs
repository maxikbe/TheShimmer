using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class QuestLocationHandler : MonoBehaviour
{
    [Header("Napojení na Quest")]
    public QuestData questToAdvance;
    public int requiredQuestStepIndex;
    
    [Tooltip("Vypnout tenhle trigger po objevení lokace?")]
    public bool disableAfterTrigger = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TryAdvanceQuest();
        }
    }

    private void TryAdvanceQuest()
    {
        if (questToAdvance == null || QuestManager.Instance == null) return;
        if (questToAdvance.currentState != QuestState.Active) return;

        if (requiredQuestStepIndex < questToAdvance.questSteps.Length)
        {
            if (!questToAdvance.questSteps[requiredQuestStepIndex].isCompleted)
            {
                QuestManager.Instance.AdvanceQuest(questToAdvance);
                Debug.Log($"[QuestLocationHandler] Hráč dorazil na místo! Quest {questToAdvance.questName} se posouvá.");
                
                if (disableAfterTrigger) gameObject.SetActive(false);
            }
        }
    }
}