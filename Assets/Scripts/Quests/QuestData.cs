using UnityEngine;

public enum QuestState
{
    NotStarted,
    Active,
    Completed
}

public enum QuestType
{
    MainQuest,
    SideQuest
}

// cast pro stepy
[System.Serializable]
public class QuestStep
{
    [TextArea(2, 5)]
    public string objectiveDescription; // co ma delat hrac
    
    [TextArea(2, 5)]
    public string logText; // detailnejsi text do journalu
    
    public bool isCompleted; // jestli je cast hotova
    
    public QuestData[] questsToAddOnComplete;
    
    public QuestData questToStartOnComplete;
}

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quest/New Quest")]
public class QuestData : ScriptableObject
{
    [Header("Základní Info")]
    public string questID; // unikatni ID
    public string questName; 
    
    public QuestType questType;
    
    public QuestState currentState; // aktivni/hotovy,...

    [Header("Průběh Questu")]
    // seznam onech casti
    public QuestStep[] questSteps; 
    
}