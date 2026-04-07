using UnityEngine;
using UnityEditor;
using UnityEngine.Events; 


[System.Serializable]
public class DialogueChoice
{

    public bool opensShop;
    
    public string choiceText; // Text na talcitku
    
    public DialogueNode nextNode; // kam vede da choice
    

    public QuestData questToStart; 
    
}

[CreateAssetMenu(fileName = "NewDialogueNode", menuName = "Dialogue/Dialogue Node")]
public class DialogueNode : ScriptableObject
{
    
    [TextArea(3, 10)] 
    public string dialogueText; // co NPC rika

    public DialogueChoice[] choices; 
}