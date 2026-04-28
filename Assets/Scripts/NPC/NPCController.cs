using UnityEngine;

public class NPCController : MonoBehaviour
{
    [Header("Master Identifikace (Základ pro Save)")]
    public string uniqueID; // Musí být unikátní ve scéně (např. "Vlk_01")
    // databaseID jsme jebli do pryč!

    [Header("Dialogy")]
    public string npcName; 
    public DialogueNode startingNode;

    [HideInInspector] public bool isDead = false;
    
    // Nová proměnná pro kontrolu, jestli jsi dost blízko
    private bool isPlayerInRange = false;

    // Metoda pro uložení stavu do tvého JSONu
    public void SaveMyState()
    {
        if (gameDataManager.currentGameData == null) return;

        // Najdeme nebo vytvoříme záznam v JSONu
        var state = gameDataManager.currentGameData.savedWorldNPCs.Find(n => n.uniqueID == uniqueID);
        if (state == null)
        {
            // Upraveno - už to po tobě nechce databaseID
            state = new NPCSaveState { uniqueID = uniqueID };
            gameDataManager.currentGameData.savedWorldNPCs.Add(state);
        }
        
        state.isDead = this.isDead;
        state.position = transform.position;
    }

    public void Interact()
    {
        Merchant myMerchant = GetComponent<Merchant>();
        FindObjectOfType<DialogueManager>().StartConversation(npcName, startingNode, myMerchant, gameObject);
    }
    
    private void Update()
    {
        // Hlídáme, jestli je hráč v zóně a zmáčknul E
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Když hráč vleze do triggeru, postava není mrtvá a má co říct -> jen nastavíme range a logneme
        if (collision.CompareTag("Player") && startingNode != null && !isDead)
        {
            isPlayerInRange = true;
            Debug.Log("Můžeš interagovat! Zmáčkni 'E' pro pokec s " + npcName);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Když hráč zdrhne z triggeru, interakce už není možná
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("Už jsi moc daleko od " + npcName + ", smůla.");
        }
    }
}