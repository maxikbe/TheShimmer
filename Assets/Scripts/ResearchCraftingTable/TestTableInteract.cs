using UnityEngine;

[RequireComponent(typeof(ResearchTable))] // Pojistka: zaručí, že tam ResearchTable fakt je
public class TestTableInteract : MonoBehaviour
{
    [Header("Odkaz na tvůj UI Manažer")]
    public ResearchUI uiManager;

    private ResearchTable myTable;
    private bool playerInRange = false;

    private void Start()
    {
        // Najdeme si stůl, který je na stejném objektu
        myTable = GetComponent<ResearchTable>();

        // Kdybys zapomněl přetáhnout UI v Inspektoru, zkusíme si ho najít sami
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<ResearchUI>();
        }
    }

    private void Update()
    {
        // Pokud stojíme blízko a zmáčkneme 'E'
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (uiManager != null)
            {
                Debug.Log("Otevírám terminál stolu!");
                uiManager.OpenCanvas(myTable);
            }
            else
            {
                Debug.LogError("Chybí odkaz na ResearchUI, Kokkotte!");
            }
        }
    }

    // Klasická trigger detekce jako máš u lootu
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
            
            // Kdybys odešel od stolu, zatímco do něj čučíš, rovnou ho zavřeme
            if (uiManager != null)
            {
                uiManager.CloseCanvas();
            }
        }
    }
}