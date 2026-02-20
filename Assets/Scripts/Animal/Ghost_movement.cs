using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ghost_movement : MonoBehaviour
{
    // --- KOMPONENTY ---
    private NavMeshAgent agent;
    
    private GameObject myBody;

    // --- ENUMY (Musí být public, aby je viděl i Animal) ---
    public enum State { Patrolling, Fleeing, Returning, Chasing}
    private State currentState;

    public enum MobBehavior { Friendly, Neutral, Aggressive }
    private MobBehavior behavior; // Už není SerializeField, dostane to zvenčí

    // --- PROMĚNNÉ (Tyhle hodnoty nám pošle Animal přes Setup) ---
    private Transform playerPosition;
    private Transform nestPosition;

    private bool debugMode;
    private float moveSpeed;
    private float runSpeed;
    private float minRunningDistance;
    private float maxRunningDistance;
    private float patrolRadius;
    private float runningRadius;
    private float visionRadius;
    private bool canHaveBreaks;
    private float minBreakTime;
    private float maxBreakTime;
    private float breakChancePercent;
    
    // Nová proměnná pro čekání po ztrátě hráče
    private float waitAfterLostTime;
    // Interní proměnné pro Chasing logiku
    private Vector3 lastKnownPlayerPos;
    private float searchTimer;
    
    //Proměnné pro přestávky
    public bool isHavingBreak;
    private float currentBreakTimer;
    
    //vision
    private float viewAngle; 
    private Vector3 facingDirection = Vector3.up;

    // Tohle necháme nastavitelné na Prefabu ducha, je to spíš globální nastavení
    [SerializeField] private LayerMask wallLayer;

    // Pojistka: dokud nás Animal nenastaví, nic neděláme (Brain Dead)
    private bool isInitialized = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        // Nastavení agenta pro 2D top-down
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    // --- TOTO JE TA KLÍČOVÁ METODA ---
    // Animal_movement zavolá tuto metodu a předá sám sebe (stats)
    public void Setup(Animal_movement stats)
    {
        // 1. Zkopírujeme data (Injektáž)
        this.debugMode = stats.debugMode;
        
        this.behavior = stats.behavior;
        this.playerPosition = stats.playerPosition;
        this.nestPosition = stats.nestPosition;
        
        this.moveSpeed = stats.moveSpeed;
        this.runSpeed = stats.runSpeed;
        
        this.minRunningDistance = stats.minRunningDistance;
        this.maxRunningDistance = stats.maxRunningDistance;
        
        this.patrolRadius = stats.patrolRadius;
        this.runningRadius = stats.runningRadius;
        this.visionRadius = stats.visionRadius;
        
        this.canHaveBreaks = stats.canHaveBreaks;
        this.minBreakTime = stats.minBreakTime;
        this.maxBreakTime = stats.maxBreakTime;
        this.breakChancePercent = stats.breakChancePercent;
        
        this.waitAfterLostTime = stats.waitAfterLostTime;
        
        this.viewAngle = stats.viewAngle;
        // Uložíme si odkaz na hmotné tělo
        this.myBody = stats.gameObject; 
        
        // 2. Aplikujeme to na Agenta
        agent.speed = moveSpeed;

        // 3. Odstartujeme logiku
        currentState = State.Patrolling;
        GoToNextPatrolPoint();
        
        // 4. Jsme ready!
        isInitialized = true;
    }

    void Update()
    {
        if (!isInitialized) return;
        
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            facingDirection = agent.velocity.normalized;
        }

        bool canSeePlayer = CheckForPlayer();

        // --- DŮLEŽITÁ POJISTKA ---
        // Pokud uvidíme hráče, okamžitě rušíme pauzu!
        if (canSeePlayer)
        {
            isHavingBreak = false;
        }

        // --- NE-FRIENDLY LOGIKA ---
        if (behavior != MobBehavior.Friendly)
        {
            if (canSeePlayer)
            {
                if (behavior == MobBehavior.Aggressive)
                {
                    currentState = State.Chasing;
                    searchTimer = 0f;
                    lastKnownPlayerPos = playerPosition.position;
                    ChasePlayer();
                }
            }
            else
            {
                switch (currentState)
                {
                    case State.Chasing:
                        ChaseLostLogic();
                        break;

                    case State.Patrolling:
                        PatrolLogic();
                        break;
                }
            }
            return;
        }

        // --- FRIENDLY LOGIKA ---
        if (canSeePlayer)
        {
            //Debug.Log("Vidím hráče");
            // Vidím ho -> ZDRHÁM!
            currentState = State.Fleeing;
            RunAwayFromPlayer();
        }
        else
        {
            // Nevidím ho (buď je pryč, nebo jsem k němu zády)
            
            if (currentState == State.Fleeing)
            {
                //Debug.Log("Nevidím ho, ale zdrhám");
                // Pokud zrovna zdrhám, musím zkontrolovat, jestli už jsem dost daleko
                // Ignorujeme úhel pohledu, zajímá nás jen čistá vzdálenost ("Slyším ho za zády")
                float distToPlayer = Vector3.Distance(transform.position, playerPosition.position);

                if (distToPlayer < visionRadius)
                {
                    // Hráč je pořád moc blízko (i když ho nevidím), musím dál utíkat!
                    RunAwayFromPlayer();
                }
                else
                {
                    //Debug.Log("Uz je dostatečně daleko");
                    // Už je daleko -> Uklidním se
                    if (!agent.pathPending && agent.remainingDistance < 0.5f)
                    {
                        currentState = State.Patrolling;
                        agent.speed = moveSpeed;
                        PatrolLogic();
                    }
                }
            }
            else if (currentState == State.Patrolling)
            {
                PatrolLogic();
            }
        }
    }

    // --- LOGIKA POHYBU (Zůstala stejná) ---

    Vector3 PatrolPosition()
    {
        // Pojistka, kdyby nest nebyl nastaven
        Vector3 centerPoint = (nestPosition != null) ? nestPosition.position : transform.position;
        
        Vector3 rand = Random.insideUnitCircle * patrolRadius;
        Vector3 point = centerPoint + new Vector3(rand.x, rand.y, 0);
        return point;
    }

    private void GoToNextPatrolPoint()
    {
        agent.speed = moveSpeed; // Ujistíme se, že při hlídce chodíme pomalu

        Vector3 randomPoint = PatrolPosition();
        NavMeshHit hit;
        
        //Pokud najde poblíž podlahu
        if (NavMesh.SamplePosition(randomPoint, out hit, 2.0f, NavMesh.AllAreas))
        {
            // kdyz nasel nejaky bod
            agent.SetDestination(hit.position);
        }
        else
        {
            //pro pripadne dalsi funkce kdyz nenajde zadne misto
        }
    }

    private void PatrolLogic()
    {
        // 1. Pokud zrovna máme pauzu, řešíme jen čekání
        if (isHavingBreak)
        {
            breakActivites();
            return; // Nepokračujeme dál, dokud pauza neskončí
        }

        // 2. Pokud nemáme pauzu a došli jsme do cíle, zkusíme si ji hodit
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            breakLogic();
        }
    }

    // --- Fleeing Logic ---

    private bool CheckForPlayer()
    {
        if (playerPosition == null) return false;

        // 1. KONTROLA VZDÁLENOSTI (Máš správně)
        float distanceToPlayer = Vector3.Distance(transform.position, playerPosition.position);
        if (distanceToPlayer > visionRadius) return false;

        Vector3 directionToPlayer = (playerPosition.position - transform.position).normalized;

        // --- 2. NOVÁ KONTROLA ÚHLU (Zorné pole) ---
        // Vector3.Angle vrací úhel mezi dvěma vektory ve stupních (0-180)
        // Pokud je úhel větší než polovina našeho zorného pole, hráč je "za rohem" nebo za zády.
        if (Vector3.Angle(facingDirection, directionToPlayer) > viewAngle / 2f)
        {
            return false; // Nevidím ho, je mimo můj kužel pohledu
        }
        // ------------------------------------------

        // 3. RAYCAST (Máš správně - kontrola zdí)
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, directionToPlayer, visionRadius);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject == myBody || hit.collider.gameObject == gameObject) continue; 
            
            if (hit.collider.CompareTag("Player"))
            {
                return true; // Vidím ho!
            }
            
            if (!hit.collider.isTrigger)
            {
                return false; // Zeď
            }
        }

        return false;
    }

    private Vector3 GetFleePoint(Vector3 directionAwayFromPlayer)
    {
        float randomAngle = Random.Range(-60f, 60f);
        float randomDistance = Random.Range(minRunningDistance, maxRunningDistance);

        Quaternion rotation = Quaternion.AngleAxis(randomAngle, Vector3.forward); // Ve 2D rotujeme kolem osy Z (forward)!
        Vector3 finalDirection = rotation * directionAwayFromPlayer;

        return transform.position + finalDirection * randomDistance;
    }

    private Vector3 GetFLeePointCloserToNest(Vector3 directionAwayFromPlayer)
    {
        if (nestPosition == null) return GetFleePoint(directionAwayFromPlayer);

        Vector3 directionToNest = (nestPosition.position - transform.position).normalized;

        // Ve 2D používáme Vector3.forward pro rotaci
        Vector3 leftDirection = Quaternion.AngleAxis(-60f, Vector3.forward) * directionAwayFromPlayer;
        Vector3 rightDirection = Quaternion.AngleAxis(60f, Vector3.forward) * directionAwayFromPlayer;

        float dotLeft = Vector3.Dot(leftDirection, directionToNest);
        float dotRight = Vector3.Dot(rightDirection, directionToNest);

        Vector3 bestDirection;

        if (dotLeft > dotRight)
        {
            bestDirection = leftDirection;
        }
        else
        {
            bestDirection = rightDirection;
        }

        float jitterAngle = Random.Range(-10f, 10f);
        float randomDistance = Random.Range(minRunningDistance, maxRunningDistance);

        bestDirection = Quaternion.AngleAxis(jitterAngle, Vector3.forward) * bestDirection;

        return transform.position + bestDirection * randomDistance;
    }

    private void RunAwayFromPlayer()
    {
        agent.speed = runSpeed; // Přepneme na běh

        Vector3 fleeDestionation; // Pozor, máš tu překlep (Destionation -> Destination), ale to funkčnost nemění :D
        
        // Pojistka pro nestPosition
        float distanceToNest = (nestPosition != null) ? Vector3.Distance(transform.position, nestPosition.position) : float.MaxValue;

        Vector3 directionAwayFromPlayer = (transform.position - playerPosition.position).normalized;

        if (distanceToNest < runningRadius)
        {
            fleeDestionation = GetFLeePointCloserToNest(directionAwayFromPlayer);
        }
        else
        {
            fleeDestionation = GetFleePoint(directionAwayFromPlayer);
        }
        
        NavMeshHit hit;
        
        // --- TADY BOLA CHYBA ---
        // Původně jsi tu měl transform.position. Musíš tam dát ten vypočítaný cíl!
        if (NavMesh.SamplePosition(fleeDestionation, out hit, 5.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            // Pokud nenajdeš NavMesh v bodě útěku, zkus aspoň utéct směrem od hráče "naslepo"
            // nebo to prostě nechat být.
        }
    }

// --- LOGIKA CHASINGU (Když ztratíme vizuál) ---
    private void ChaseLostLogic()
    {
        agent.speed = runSpeed;

        // 1. Nastavíme cíl na poslední známou pozici
        agent.SetDestination(lastKnownPlayerPos);

        // 2. Kontrola, jestli jsme doběhli na místo, kde jsme ho naposledy viděli
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            // Jsme tam, ale hráč nikde (protože CheckForPlayer vrátil false)
            // Začneme odpočítávat čas
            searchTimer += Time.deltaTime;

            // Debug.Log($"Hledám hráče... {searchTimer}/{waitAfterLostTime}");

            if (searchTimer >= waitAfterLostTime)
            {
                // Čas vypršel, vzdáváme to -> Patrol
                currentState = State.Patrolling;
                GoToNextPatrolPoint();
            }
        }
    }

    private void ChasePlayer()
    {
        agent.speed = runSpeed;
        agent.SetDestination(playerPosition.position);
    }

    private void breakLogic()
    {
        // Nejdřív zjistíme, jestli jsou pauzy vůbec povolené
        if (canHaveBreaks)
        {
            float roll = Random.Range(0f, 100f);
            
            // Pokud hodíme méně než je šance (WIN)
            if (roll < breakChancePercent)
            {
                isHavingBreak = true;
                currentBreakTimer = Random.Range(minBreakTime, maxBreakTime);
                // Debug.Log($"<color=green>Pauza na {currentBreakTimer}s (Roll: {roll})</color>");
                return; // Ukončíme metodu, nejdeme na další bod
            }
        }

        // Pokud pauzy nejsou povolené NEBO jsme prohráli hod -> Jdeme dál
        GoToNextPatrolPoint();
    }

    private void breakActivites()
    {
        // Odečítáme čas
        currentBreakTimer -= Time.deltaTime;

        
        
        //----------------PŘÍPADNÁ PODMÍNKA PRO ANIMACI PŘI BREAKU---------------------------
        //Debug.Log($"<color=cyan>Odpočívám... {currentBreakTimer:F1}</color>");

        // Konec pauzy
        if (currentBreakTimer <= 0)
        {
            isHavingBreak = false;
            GoToNextPatrolPoint();
        }
    }
    
// Změněno z OnDrawGizmosSelected na OnDrawGizmos
    private void OnDrawGizmos()
    {
        // Pokud není zaplý debug mode, nic nekresli a vyskoč pryč
        if (!debugMode) return;

        // --- Zbytek je stejný ---
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRadius);

        Gizmos.color = Color.red;
        
        Vector3 lookDir = Application.isPlaying ? facingDirection : transform.up;

        Vector3 leftViewDir = Quaternion.Euler(0, 0, viewAngle / 2) * lookDir;
        Vector3 rightViewDir = Quaternion.Euler(0, 0, -viewAngle / 2) * lookDir;

        Gizmos.DrawLine(transform.position, transform.position + leftViewDir * visionRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightViewDir * visionRadius);
    }
}