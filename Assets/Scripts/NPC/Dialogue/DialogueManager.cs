using UnityEngine;
using UnityEngine.UI;
using TMPro; // Nutnost pro kvalitní fonty!

public class DialogueManager : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject dialoguePanel; // Celé to UI okno
    public TextMeshProUGUI npcNameText; // Kdo mluví
    public TextMeshProUGUI dialogueText; // Co říká
    
    [Header("Tlačítka")]
    public GameObject buttonPrefab; // Šablona tlačítka (Prefab)
    public Transform buttonContainer; // Složka, kam se tlačítka sází pod sebe

    // Tady si DialogueManager "pamatuje", s kým zrovna mluví
    private string currentSpeakerName;

    // 1. ZCELA NOVÁ FUNKCE - Volá ji skript na NPC (např. Kováři), když rozhovor začíná
    public void StartConversation(string npcName, DialogueNode firstNode)
    {
        // Uložíme si jméno do inventáře, ať ho máme po ruce pro další texty
        currentSpeakerName = npcName; 
        
        // A pustíme samotné vykreslení dialogu
        ContinueDialogue(firstNode);
    }

    // 2. TVÁ PŮVODNÍ FUNKCE - Upravená pro tenhle nový systém
    public void ContinueDialogue(DialogueNode node)
    {
        // Zapneme okno dialogu
        dialoguePanel.SetActive(true);

        // Použijeme uložené jméno místo toho, abychom ho tahali ze ScriptableObjectu!
        npcNameText.text = currentSpeakerName; 
        dialogueText.text = node.dialogueText;

        // Brutální čistka: Smažeme stará tlačítka z minulého uzlu
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        // Naklonujeme nová tlačítka pro každou odpověď v tomto uzlu
        foreach (DialogueChoice choice in node.choices)
        {
            GameObject newButton = Instantiate(buttonPrefab, buttonContainer);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = choice.choiceText;

            // --- MAGIE PRO QUESTY A VĚTVENÍ ---
            newButton.GetComponent<Button>().onClick.AddListener(() => 
            {
                // Odpálí to všechno, co sis v Inspectoru naklikal pro tuhle odpověď
                choice.onChoiceSelected?.Invoke(); 

                // Zkontrolujeme, jestli rozhovor pokračuje na další uzel
                if (choice.nextNode != null)
                {
                    // TADY JE ZMĚNA: Voláme ContinueDialogue místo původního StartDialogue
                    ContinueDialogue(choice.nextNode); 
                }
                else
                {
                    // Pokud je nextNode prázdný, NPC s tebou domluvilo a okno se zavře
                    dialoguePanel.SetActive(false);
                }
            });
        }
    }
}