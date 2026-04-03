using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewQuestDatabase", menuName = "Quest/Quest Database")]
public class QuestDatabase : ScriptableObject
{
    [Header("All Quest in game")]
    public List<QuestData> allQuests;

    [ContextMenu("Quest Auto-Load")]
    public void LoadQuests()
    {
        // Unity prohledá ÚPLNĚ VŠECHNY složky s názvem "Resources" v celém projektu 
        // a najde tam všechny QuestData.
        allQuests = Resources.LoadAll<QuestData>("").ToList();
        
        Debug.Log($"Databáze aktualizována! Bylo nalezeno {allQuests.Count} questů.");
    }

    // pokud by bylo potřeba najít quest podle jména
    public QuestData GetQuestByID(string id)
    {
        return allQuests.Find(q => q.questID == id);
    }
}