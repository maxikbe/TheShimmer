using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class QuestCondition
{
    public QuestData quest;
    public QuestState requiredState; 
}

// NOVÉ: Podmínka inventáře
[System.Serializable]
public class ItemCondition
{
    public Item requiredItem; // Co musí mít
    public int requiredAmount = 1; // Kolik kusů
    [Tooltip("Má se item odevzdat (zničit z báglu) po kliknutí?")]
    public bool consumeItem = true; 
}

public enum CommandType { None, WaitHere, FollowMe, AllWait, AllFollow }

[System.Serializable]
public class DialogueChoice
{
    public bool opensShop;
    public string choiceText; 
    public DialogueNode nextNode; 
    
    [Header("Quest Logika a Svět")]
    public QuestData questToStart; 
    public QuestData questToAdvance; // Posune probíhající quest dál
    public bool triggerCombat; // Speedrun tlačítko (začne fight)
    
    [Header("Příkazy pro NPC")]
    public CommandType npcCommand; 
    
    [Header("Podmínky pro zobrazení tlačítka")]
    public List<QuestCondition> conditions = new List<QuestCondition>(); 
    public List<ItemCondition> itemConditions = new List<ItemCondition>(); // Tlačítko se ukáže jen, když máš loot
}

[CreateAssetMenu(fileName = "NewDialogueNode", menuName = "Dialogue/Dialogue Node")]
public class DialogueNode : ScriptableObject
{
    [TextArea(3, 10)] 
    public string dialogueText; 
    public DialogueChoice[] choices; 
}