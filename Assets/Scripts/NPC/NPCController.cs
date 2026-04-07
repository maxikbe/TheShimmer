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
        //pokud je obchodnik  ulozime merchant script
        Merchant myMerchant = GetComponent<Merchant>();
        
        // pripadne spustime konvezaci pres dialogue managera
        FindObjectOfType<DialogueManager>().StartConversation(npcName, startingNode, myMerchant);
    }
}