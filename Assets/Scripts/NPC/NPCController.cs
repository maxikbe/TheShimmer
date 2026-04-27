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
    
    // TADY JE TA OPRAVA DIALOGŮ - Tohle jsi omylem smazal!
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Když hráč vleze do triggeru, postava není mrtvá a má co říct -> spustí se dialog
        if (collision.CompareTag("Player") && startingNode != null && !isDead)
        {
            Interact();
        }
    }
}