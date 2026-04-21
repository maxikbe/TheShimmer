using UnityEngine;
using System.Collections.Generic; // Přidáno pro List

// Nová třída pro podmínky!
[System.Serializable]
public class QuestCondition
{
    public QuestData quest;
    public QuestState requiredState; // např. musí být "Completed", aby se volba ukázala
}

[System.Serializable]
public class DialogueChoice
{
    public bool opensShop;
    public string choiceText; 
    public DialogueNode nextNode; 
    public QuestData questToStart; 
    
    [Header("Witcher Podmínky")]
    // Seznam podmínek, které musí být splněny. Pokud je prázdný, ukáže se vždycky.
    public List<QuestCondition> conditions = new List<QuestCondition>(); 
}

[CreateAssetMenu(fileName = "NewDialogueNode", menuName = "Dialogue/Dialogue Node")]
public class DialogueNode : ScriptableObject
{
    [TextArea(3, 10)] 
    public string dialogueText; 
    public DialogueChoice[] choices; 
}