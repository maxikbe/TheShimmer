using UnityEngine;

// Stavy, ve kterých se quest může nacházet
public enum QuestState
{
    NotStarted,
    Active,
    Completed
}

// Tohle reprezentuje jednu fázi questu (to postupné odkrývání)
[System.Serializable]
public class QuestStep
{
    [TextArea(2, 5)]
    public string objectiveDescription; // Co máš zrovna udělat (např. "Najdi kovářovu dceru")
    
    [TextArea(2, 5)]
    public string logText; // Detailnější text, co se připíše do deníku, když tuhle fázi odemkneš
    
    public bool isCompleted; // Je tahle konkrétní část hotová?
    
    public QuestData[] questsToAddOnComplete;
    
    public QuestData questToStartOnComplete;
}

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quest/New Quest")]
public class QuestData : ScriptableObject
{
    [Header("Základní Info")]
    public string questID; // Unikátní jméno (např. "Q_Main_01_Kovar")
    public string questName; // To, co uvidí hráč v UI (např. "Ztracená dcera")
    
    public QuestState currentState; // Jestli je aktivní, hotový, atd.

    [Header("Průběh Questu")]
    // Tohle je ten tvůj seznam fází. Hráč uvidí vždy jen texty z fází, ve kterých už je nebo je prošel.
    public QuestStep[] questSteps; 
    
}