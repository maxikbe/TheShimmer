using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

public class Animal_movement : MonoBehaviour
{
    [Header("Ghost Settings")]
    public GameObject ghostPrefab; 
    public bool debugMode = false;

    [Header("Stats")]
    public Ghost_movement.MobBehavior behavior; //mob behavior z ghosta
    public float moveSpeed = 2f;
    public float runSpeed = 4f;
    
    [Header("Vision Settings")]
    public float visionRadius = 3f;
    [Range(0, 360)] public float viewAngle = 90f;
    
    [Header("Breaks settings")]
    public bool canHaveBreaks = false;
    [ShowIf("canHaveBreaks")]
    public float minBreakTime = 0.5f;
    [ShowIf("canHaveBreaks")]
    public float maxBreakTime = 3f;
    [ShowIf("canHaveBreaks")]
    [SerializeField, Range(0f, 100f)] 
    public float breakChancePercent = 25f;

    [Header("Patrol Settings")]
    public float patrolRadius = 5f;
    public float runningRadius = 10f;
    
    [Tooltip("Minimální čas, jak dlouho mobka stojí na hlídkovacím bodu")]
    public float minPatrolWait = 0.5f;
    [Tooltip("Maximální čas, jak dlouho mobka stojí na hlídkovacím bodu")]
    public float maxPatrolWait = 1.5f;
    
    
    [Header("Flee Settings")]
    public float minRunningDistance = 1f;
    public float maxRunningDistance = 3f;
    
    [Header("Chase Settings")]
    public float waitAfterLostTime = 2f;
    [Tooltip("Jak dlouho mobka hlídkuje jako agresivní, než se uklidní")]
    public float calmDownTime = 30f; 

    [Header("References")]
    public Transform playerPosition; // hráč objekt
    public Transform nestPosition;   // nest animal

    
    
    
    //Lokální proměnné!!!!!!
    //pro animace při breaku
    private bool isFacingRight = true;
    
    // Reference na vytvořeného ducha, pro sledování
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

        // vytvoří ducha na pozici zvířete
        GameObject ghostObj = Instantiate(ghostPrefab, transform.position, Quaternion.identity);
        ghostObj.name = $"{gameObject.name}_Ghost";

        // získá jeho sript
        myGhost = ghostObj.GetComponent<Ghost_movement>();

        // předá nastavení z tohoto scirptu
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
            // pohyb
            transform.position = myGhost.transform.position;
            
            UpdateAnimationDirection(transform.position - previousPos);
        }
    }
    
    void UpdateAnimationDirection(Vector3 movementVector)
    {
        // pohyb malý - iddle
        if (movementVector.magnitude < MovementTreshold)
        {
            // Debug.Log("IDLE (Stojím)");
            return;
        }

        // --- pro breaky  ---
        if (movementVector.x > 0) 
        {
            isFacingRight = true; //  doprava
        }
        else if (movementVector.x < 0) 
        {
            isFacingRight = false; //  doleva
        }
        // ---
        
        // jenom směr ne vzdálenost
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
    
    void OnDestroy()
    {
        if (myGhost != null) Destroy(myGhost.gameObject);
    }
    
    // pro combat
    public void MakeAggressive()
    {
        // měníme pro sebe (kvuli inspektoru)
        behavior = Ghost_movement.MobBehavior.Aggressive; 

        // pošleme povel ghostu
        if (myGhost != null)
        {
            myGhost.ChangeBehavior(behavior);
        }
    }
    
}