using UnityEngine;

public class NPCController : MonoBehaviour
{
    [Header("Nastavení NPC")]
    public string npcName; // Tady napíšeš "Kovář" rovnou v Inspectoru na scéně
    public DialogueNode startingNode; // Tady mu dáš do ruky první část rozhovoru

    // Když jakýkoliv fyzikální objekt vstoupí do Trigger zóny tohoto NPC...
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Zkontrolujeme, jestli do nás narazil opravdu Hráč a ne něco jiného
        if (collision.CompareTag("Player"))
        {
            Interact();
        }
    }

    // Tuhle funkci odpálíme
    public void Interact()
    {
        // Najdeme našeho manažera a hodíme po něm jméno i scénář
        FindObjectOfType<DialogueManager>().StartConversation(npcName, startingNode);
    }
}