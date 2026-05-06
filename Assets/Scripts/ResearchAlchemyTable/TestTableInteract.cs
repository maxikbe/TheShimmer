using UnityEngine;

[RequireComponent(typeof(LabTable))] 
public class TestTableInteract : MonoBehaviour
{
    [Header("Odkaz na tvůj UI Manažer")]
    public LabUIManager uiManager; // ZMĚNA: Už nevoláme ResearchUI, ale ten hlavní Manager!

    private LabTable myTable;
    private bool playerInRange = false;

    private void Start()
    {
        myTable = GetComponent<LabTable>();

        if (uiManager == null)
        {
            uiManager = FindObjectOfType<LabUIManager>();
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyBoardSetting.Interact))
        {
            if (uiManager != null)
            {
                Debug.Log("Otevírám terminál stolu!");
                
                // Můžeš tady rovnou zapnout celý Canvas, pokud to máš tak nastavené
                uiManager.gameObject.SetActive(true); 
                
                uiManager.OpenLabTerminals(myTable); 
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Jsi u stolu. Zmáčkni 'E' pro výzkum.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            
            if (uiManager != null)
            {
                uiManager.CloseLabTerminals();
                uiManager.gameObject.SetActive(false); // Vypne Canvas, pokud odejdeš
            }
        }
    }
}