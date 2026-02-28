using UnityEngine;
using UnityEngine.Events; // Tohle je důležité přidat kvůli UnityEvent!

// --- 1. TADY JE TA CHYBĚJÍCÍ TŘÍDA ---
// Musí nad ní být [System.Serializable], jinak ji Unity neukáže v Inspectoru
[System.Serializable]
public class DialogueChoice
{
    public string choiceText; // Text na tlačítku
    public DialogueNode nextNode; // Kam to vede dál
    public UnityEvent onChoiceSelected; // Event na questy
}

// --- 2. TVŮJ PŮVODNÍ KÓD ---
[CreateAssetMenu(fileName = "NewDialogueNode", menuName = "Dialogue/Dialogue Node")]
public class DialogueNode : ScriptableObject
{
    
    [TextArea(3, 10)] 
    public string dialogueText; // Co říká NPC
    
    // Nyní už kompilátor ví, co je DialogueChoice, a chyba zmizí!
    public DialogueChoice[] choices; 
}