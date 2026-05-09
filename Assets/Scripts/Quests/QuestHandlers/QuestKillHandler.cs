using UnityEngine;

[RequireComponent(typeof(Mob_combat))]
public class QuestKillHandler : MonoBehaviour
{
    [Header("Napojení na Quest")]
    [Tooltip("Který quest se má posunout, když tahle potvora natáhne bačkory?")]
    public QuestData questToAdvance;

    [Tooltip("Na kterém kroku questu musí hráč zrovna být? (Indexováno od 0)")]
    public int requiredQuestStepIndex;

    private Mob_combat myCombatScript;
    private bool hasTriggered = false;

    void Awake()
    {
        myCombatScript = GetComponent<Mob_combat>();
    }

    void Update()
    {
        // Hlídáme, jestli je mobka mrtvá (funguje to i na TBC při návratu do scény)
        if (!hasTriggered && myCombatScript != null && myCombatScript.isDead)
        {
            TryAdvanceQuest();
            hasTriggered = true; // Ať to nespamuje do konce věků
        }
    }

    private void TryAdvanceQuest()
    {
        if (questToAdvance == null || QuestManager.Instance == null) return;
        if (questToAdvance.currentState != QuestState.Active) return; // Quest musí být aktivní

        if (requiredQuestStepIndex < questToAdvance.questSteps.Length)
        {
            if (!questToAdvance.questSteps[requiredQuestStepIndex].isCompleted)
            {
                QuestManager.Instance.AdvanceQuest(questToAdvance);
                Debug.Log($"[QuestKillHandler] Mrtvola {gameObject.name} posunula quest {questToAdvance.questName}!");
            }
        }
    }
}