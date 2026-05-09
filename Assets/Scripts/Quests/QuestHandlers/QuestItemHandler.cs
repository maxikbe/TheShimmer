using UnityEngine;

public class QuestItemHandler : MonoBehaviour
{
    [Header("Napojení na Quest")]
    [Tooltip("Který quest se má posunout?")]
    public QuestData questToAdvance;

    [Tooltip("Na kterém kroku questu musí hráč zrovna být? (Indexováno od 0)")]
    public int requiredQuestStepIndex;

    [Header("Podmínka v Inventáři")]
    [Tooltip("Jaký item musí hráč získat (vycraftit/sebrat)?")]
    public Item requiredItem;
    
    [Tooltip("Kolik kusů musí nasbírat?")]
    public int requiredAmount = 1;

    // Optimalizace: Nebudeme to kontrolovat 60x za vteřinu, stačí 1x za vteřinu
    private float checkInterval = 1f;
    private float timer = 0f;
    private bool hasTriggered = false;

    void Update()
    {
        if (hasTriggered || questToAdvance == null || requiredItem == null) return;
        
        // Quest musí být aktivní
        if (questToAdvance.currentState != QuestState.Active) return;

        // Kontrolujeme, jestli jsme na správném kroku a není ještě splněný
        if (requiredQuestStepIndex < questToAdvance.questSteps.Length)
        {
            if (!questToAdvance.questSteps[requiredQuestStepIndex].isCompleted)
            {
                // Odpočet
                timer += Time.deltaTime;
                if (timer >= checkInterval)
                {
                    timer = 0f;
                    CheckInventoryForLoot();
                }
            }
        }
    }

    private void CheckInventoryForLoot()
    {
        // Pojistka proti prázdným datům
        if (gameDataManager.currentGameData == null) return;

        // Najdeme item v tvém JSON inventáři
        ItemSaveData foundItem = gameDataManager.currentGameData.OwnedItems.Find(i => i.id == requiredItem.id && i.isOwned);
        
        // Má ho a má jich dost?
        if (foundItem != null && foundItem.amount >= requiredAmount)
        {
            QuestManager.Instance.AdvanceQuest(questToAdvance);
            Debug.Log($"[QuestItemHandler] Hráč získal do inventáře {requiredItem.itemName}! Quest {questToAdvance.questName} se posouvá.");
            
            hasTriggered = true; 
            this.enabled = false; // Úkol splněn, skript jde spát, ať nežere výkon
        }
    }
}