using Unity.VisualScripting;
using UnityEngine;

public class Animal_movement : MonoBehaviour
{
    [Header("Ghost Settings")]
    public GameObject ghostPrefab; // Sem přetáhni prefab ducha

    [Header("Stats")]
    public Ghost_movement.MobBehavior behavior; // Enum musíme vzít z Ghosta nebo ho dát mimo
    public float moveSpeed = 2f;
    public float runSpeed = 4f;
    public float visionRadius = 3f;
    
    [Header("Breaks settings")]
    public bool canHaveBreaks = false;
    public float minBreakTime = 0.5f;
    public float maxBreakTime = 3f;

    [Header("Patrol Settings")]
    public float patrolRadius = 5f;
    public float runningRadius = 10f;
    
    [Header("Flee Settings")]
    public float minRunningDistance = 1f;
    public float maxRunningDistance = 3f;

    [Header("References")]
    public Transform playerPosition; // Přetáhni hráče nebo ho najdi v Awake
    public Transform nestPosition;   // Přetáhni hnízdo

    // Reference na vytvořeného ducha, abychom ho mohli sledovat
    private Ghost_movement myGhost;

    void Start()
    {
        SpawnAndSetupGhost();
    }

    void SpawnAndSetupGhost()
    {
        if (ghostPrefab == null)
        {
            Debug.LogError($"Kokkotte, zapomněl jsi přiřadit Ghost Prefab v {gameObject.name}!");
            return;
        }

        // 1. Vytvoříme ducha na pozici zvířete
        GameObject ghostObj = Instantiate(ghostPrefab, transform.position, Quaternion.identity);
        ghostObj.name = $"{gameObject.name}_Ghost";

        // 2. Získáme jeho skript
        myGhost = ghostObj.GetComponent<Ghost_movement>();

        // 3. PŘEDÁME MU DATA (To je to kouzlo)
        // Předáváme "this", tedy tento skript s nastavením
        myGhost.Setup(this); 
    }

    void Update()
    {
        if (myGhost != null)
        {
            // Tady řešíš vizuální pohyb za duchem (Lerp), viz předchozí rada
            transform.position = Vector3.Lerp(transform.position, myGhost.transform.position, Time.deltaTime * 5f);
            
            // Tady bys mohl řešit animace podle myGhost.agent.velocity atd.
        }
    }
    
    // Uklidíme po sobě
    void OnDestroy()
    {
        if (myGhost != null) Destroy(myGhost.gameObject);
    }
}