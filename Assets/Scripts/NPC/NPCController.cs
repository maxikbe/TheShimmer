using UnityEngine;

public class NPCController : MonoBehaviour
{
    [Header("Nastavení NPC")]
    public string npcName; 
    public DialogueNode startingNode; // prvni cast dialogu

    // pokud cokolic vsoupi do triggru
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // a pokud je to hrac
        if (collision.CompareTag("Player"))
        {
            Interact();
        }
    }

    public void Interact()
    {
        //pokud je obchodnik ulozime merchant script
        Merchant myMerchant = GetComponent<Merchant>();
        
        // TADY JE ZMĚNA: Přidáváme "gameObject" jako 4. parametr, abychom věděli, s jakým tělem mluvíme
        FindObjectOfType<DialogueManager>().StartConversation(npcName, startingNode, myMerchant, gameObject);
    }
}