using Unity.VisualScripting;
using UnityEngine;

public class Animal_movement : MonoBehaviour
{
    [Header("Ghost Settings")]
    public GameObject ghostPrefab; // Sem přetáhni prefab ducha
    public bool debugMode = false;

    [Header("Stats")]
    public Ghost_movement.MobBehavior behavior; // Enum musíme vzít z Ghosta nebo ho dát mimo
    public float moveSpeed = 2f;
    public float runSpeed = 4f;
    
    [Header("Vision Settings")]
    public float visionRadius = 3f;
    [Range(0, 360)] public float viewAngle = 90f;
    
    [Header("Breaks settings")]
    public bool canHaveBreaks = false;
    public float minBreakTime = 0.5f;
    public float maxBreakTime = 3f;
    [SerializeField, Range(0f, 100f)] 
    public float breakChancePercent = 25f;

    [Header("Patrol Settings")]
    public float patrolRadius = 5f;
    public float runningRadius = 10f;
    
    [Header("Flee Settings")]
    public float minRunningDistance = 1f;
    public float maxRunningDistance = 3f;
    
    [Header("Chase Settings")]
    public float waitAfterLostTime = 2f;

    [Header("References")]
    public Transform playerPosition; // Přetáhni hráče nebo ho najdi v Awake
    public Transform nestPosition;   // Přetáhni hnízdo

    
    
    
    //Lokální proměnné!!!!!!
    //pro animace při breaku
    private bool isFacingRight = true;
    
    // Reference na vytvořeného ducha, abychom ho mohli sledovat
    private Ghost_movement myGhost;
    
    private Vector3 lastPosition;
    private const float MovementTreshold = 0.005f; //citlivost

    void Start()
    {
        SpawnAndSetupGhost();
    }

    void SpawnAndSetupGhost()
    {
        if (ghostPrefab == null)
        {
            Debug.LogError($"chybí ghost prefab v:  {gameObject.name}!");
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
        //-------------PRo animaci při breaku
        /*
        if (myGhost.isHavingBreak)
        {
            if (isFacingRight)
            {
                Debug.Log("Ghost is having break on right");
            }
            else
            {
                Debug.Log("Ghost is having break on left");
            }
        }
        */
        
        if (myGhost != null)
        {
            Vector3 previousPos = transform.position;
            // Tady řešíš vizuální pohyb za duchem (Lerp), viz předchozí rada
            transform.position = Vector3.Lerp(transform.position, myGhost.transform.position, Time.deltaTime * 5f);
            
            UpdateAnimationDirection(transform.position - previousPos);
            // Tady bys mohl řešit animace podle myGhost.agent.velocity atd.
        }
    }
    
    // --- NOVÁ METODA NA SMĚRY ---
    void UpdateAnimationDirection(Vector3 movementVector)
    {
        // Pokud je pohyb moc malý, považujeme to za IDLE a neřešíme směr
        if (movementVector.magnitude < MovementTreshold)
        {
            // Debug.Log("IDLE (Stojím)");
            return;
        }

        // --- pro breaky  ---
        if (movementVector.x > 0) 
        {
            isFacingRight = true; // Jde doprava
        }
        else if (movementVector.x < 0) 
        {
            isFacingRight = false; // Jde doleva
        }
        // ---
        
        // Normalizujeme vektor (chceme jen směr, ne délku)
        Vector3 dir = movementVector.normalized;

        // vraci v radialech prevadime na stupne
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        if (angle < 0) angle += 360;

        //podle stupnu
        float step = 360f / 8f;
        int sector = Mathf.FloorToInt((angle + step / 2) % 360 / step);
        //---------------------------- ZAKOMENTOVANY SWITCH ABY TO NESPAMOBALO-------------------------------------------------"""""""!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /*
        switch (sector)
        {
            case 0:
                Debug.Log("RIGHT (Východ) ➡");
                break;
            case 1:
                Debug.Log("UP-RIGHT (Severovýchod) ↗");
                break;
            case 2:
                Debug.Log("UP (Sever) ⬆");
                break;
            case 3:
                Debug.Log("UP-LEFT (Severozápad) ↖");
                break;
            case 4:
                Debug.Log("LEFT (Západ) ⬅");
                break;
            case 5:
                Debug.Log("DOWN-LEFT (Jihozápad) ↙");
                break;
            case 6:
                Debug.Log("DOWN (Jih) ⬇");
                break;
            case 7:
                Debug.Log("DOWN-RIGHT (Jihovýchod) ↘");
                break;
        }
        */
    }
    
    // Uklidíme po sobě
    void OnDestroy()
    {
        if (myGhost != null) Destroy(myGhost.gameObject);
    }
}