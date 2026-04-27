using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ghost_movement : MonoBehaviour
{
    private NavMeshAgent agent;
    
    private GameObject myBody;

    public enum State { Patrolling, Fleeing, Returning, Chasing}
    private State currentState;

    public enum MobBehavior { Friendly, Neutral, Aggressive, Companion }
    private MobBehavior behavior; 

    // posílá animal pres setup
    private Transform playerPosition;
    private Transform nestPosition;
    
    private Animal_movement myAnimalStats; // odkazuje zpet na hmotne telo
    private MobBehavior originalBehavior;  
    
    private float calmDownTime; 
    private float currentCalmTimer; 
    private bool isCoolingDown; 
    
    private float minPatrolWait;
    private float maxPatrolWait;
    private float currentPatrolWaitTimer;
    private bool isWaitingAtPatrol;
    

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
    
    // cekani po ztrate hrace
    private float waitAfterLostTime;
    // chaing logika
    private Vector3 lastKnownPlayerPos;
    private float searchTimer;
    
    public bool isHavingBreak;
    private float currentBreakTimer;
    
    private float viewAngle; 
    private Vector3 facingDirection = Vector3.up;

    // nastavitelne na prefabu ducha toto je jenom globalni
    [SerializeField] private LayerMask wallLayer;
    
    //companion settings
    private float minFollowDistance ;
    private float maxFollowDistance;
    public bool isWaiting;
    
    
    private bool isInitialized = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        // nastaveni pro 2D
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    // předávání proměnných z animal sem
    public void Setup(Animal_movement stats)
    {
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
        // --- NOVÉ ---
        this.minPatrolWait = stats.minPatrolWait;
        this.maxPatrolWait = stats.maxPatrolWait;
        
        this.canHaveBreaks = stats.canHaveBreaks;
        this.minBreakTime = stats.minBreakTime;
        this.maxBreakTime = stats.maxBreakTime;
        this.breakChancePercent = stats.breakChancePercent;
        
        this.waitAfterLostTime = stats.waitAfterLostTime;
        
        this.minFollowDistance = stats.minFollowDistance;
        this.maxFollowDistance = stats.maxFollowDistance;
        this.isWaiting = stats.isWaiting;
        
        this.viewAngle = stats.viewAngle;
        // odkaz na hmotne telo
        this.myBody = stats.gameObject; 
        
        
        this.myAnimalStats = stats; 
        this.originalBehavior = stats.behavior; 
        this.calmDownTime = stats.calmDownTime;
        
        agent.speed = moveSpeed;

        currentState = State.Patrolling;
        GoToNextPatrolPoint();
        
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

        if (canSeePlayer)
        {
            isHavingBreak = false;
            isWaitingAtPatrol = false; 
            isCoolingDown = false; 
        }

        if (isCoolingDown && !canSeePlayer)
        {
            currentCalmTimer += Time.deltaTime;
            if (currentCalmTimer >= calmDownTime)
            {
                isCoolingDown = false;
                this.behavior = originalBehavior;
                
                if (myAnimalStats != null)
                {
                    myAnimalStats.behavior = originalBehavior; 
                }
                
                // Debug.Log("Nikdo nebyl nalezen, vracím se zpět do normálu");
            }
        }

        if (behavior == MobBehavior.Companion)
        {
            CompanionLogic();
            return;
        }
        
        // --- NO-FRIENDLY LOGIKA ---
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
            currentState = State.Fleeing;
            RunAwayFromPlayer();
        }
        else
        {
            //nevidí hráče
            if (currentState == State.Fleeing)
            {
                //Debug.Log("Nevidím ho, ale utíkám");
                // kdyz utika ignoruje uhel pohledu, resi jenom vzdalenost
                float distToPlayer = Vector3.Distance(transform.position, playerPosition.position);

                if (distToPlayer < visionRadius)
                {
                    // hrac je blizko
                    RunAwayFromPlayer();
                }
                else
                {
                    //Debug.Log("Hráč je dostatečně daleko");
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


    Vector3 PatrolPosition()
    {
        Vector3 centerPoint = (nestPosition != null) ? nestPosition.position : transform.position;
        
        Vector3 rand = Random.insideUnitCircle * patrolRadius;
        Vector3 point = centerPoint + new Vector3(rand.x, rand.y, 0);
        return point;
    }

    private void GoToNextPatrolPoint()
    {
        agent.speed = moveSpeed; //Chození pomalu pri patrolingu

        Vector3 randomPoint = PatrolPosition();
        NavMeshHit hit;
        
        //kdyz najde podlahu
        if (NavMesh.SamplePosition(randomPoint, out hit, 2.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            //pro nejake dlasi funkce kdyz podlahu nenajde
        }
    }

    private void PatrolLogic()
    {
        // ma pauzu resi jenom logiku pauz
        if (isHavingBreak)
        {
            breakActivites();
            return; 
        }

        // pokud dorazil na pozici
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            // pokud jeste neceka, spustit se cekani
            if (!isWaitingAtPatrol)
            {
                isWaitingAtPatrol = true;
                currentPatrolWaitTimer = Random.Range(minPatrolWait, maxPatrolWait);
            }
            // pokud se uz ceka, ceka se a pak se spusti jestli si da velkou pauzu nebo ne
            else
            {
                currentPatrolWaitTimer -= Time.deltaTime;
                
                if (currentPatrolWaitTimer <= 0)
                {
                    isWaitingAtPatrol = false;
                    breakLogic(); // rozhodne se velka pauza
                }
            }
        }
    }

    // --- Fleeing Logic ---

    private bool CheckForPlayer()
    {
        if (playerPosition == null) return false;

        // kontrola vzdalenosti
        float distanceToPlayer = Vector3.Distance(transform.position, playerPosition.position);
        if (distanceToPlayer > visionRadius) return false;

        Vector3 directionToPlayer = (playerPosition.position - transform.position).normalized;

        // --- 2. NOVÁ KONTROLA ÚHLU (Zorné pole) ---
        // pokud je uhel od hrace vetsi nez polovina zorneho pole, hrace nevidime
        if (Vector3.Angle(facingDirection, directionToPlayer) > viewAngle / 2f)
        {
            return false;
        }
        
        

        // kontrola zdí
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, directionToPlayer, visionRadius);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject == myBody || hit.collider.gameObject == gameObject) continue; 
            
            if (hit.collider.CompareTag("Player"))
            {
                return true; // vidi hrace
            }
            
            if (!hit.collider.isTrigger)
            {
                return false; // vidi zed
            }
        }

        return false;
    }

    private Vector3 GetFleePoint(Vector3 directionAwayFromPlayer)
    {
        float randomAngle = Random.Range(-60f, 60f);
        float randomDistance = Random.Range(minRunningDistance, maxRunningDistance);

        Quaternion rotation = Quaternion.AngleAxis(randomAngle, Vector3.forward);
        Vector3 finalDirection = rotation * directionAwayFromPlayer;

        return transform.position + finalDirection * randomDistance;
    }

    private Vector3 GetFLeePointCloserToNest(Vector3 directionAwayFromPlayer)
    {
        if (nestPosition == null) return GetFleePoint(directionAwayFromPlayer);

        Vector3 directionToNest = (nestPosition.position - transform.position).normalized;

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
        agent.speed = runSpeed; // prepina na beh speed

        Vector3 fleeDestionation;
        
        // pojistka pro nestposition
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
        

        if (NavMesh.SamplePosition(fleeDestionation, out hit, 5.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            // pokud nenajde zadny bod na navmeshi. Pro pridani dalsich funkci
        }
    }

// --- CHASING LOGIC ---
    private void ChaseLostLogic()
    {
        agent.speed = runSpeed;
        agent.SetDestination(lastKnownPlayerPos);

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            searchTimer += Time.deltaTime;

            if (searchTimer >= waitAfterLostTime)
            {
                // zpet na patroling, vyprsel cas
                currentState = State.Patrolling;
                GoToNextPatrolPoint();

                // pokud nebyl puvodne agresive, tak chvili trva nez se prepne zpět
                if (behavior == MobBehavior.Aggressive && originalBehavior != MobBehavior.Aggressive)
                {
                    isCoolingDown = true;
                    currentCalmTimer = 0f;
                }
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
        // zjistuje jestli jsou pauzy povolene
        if (canHaveBreaks)
        {
            float roll = Random.Range(0f, 100f);
            
            // pokud hodi mene nez je sance na prestavku
            if (roll < breakChancePercent)
            {
                isHavingBreak = true;
                currentBreakTimer = Random.Range(minBreakTime, maxBreakTime);
                // Debug.Log($"Pauza na {currentBreakTimer}s (Roll: {roll})");
                return; 
            }
        }

        // pokud nejsou povolene pauzy, nebo nevysel hod
        GoToNextPatrolPoint();
    }

    private void breakActivites()
    {
        currentBreakTimer -= Time.deltaTime;

        
        
        //----------------PRIPADNE TADY PODMINKA PRO ANIMACI PRI BREKAU---------------------------
        //Debug.Log($"Odpočívá... {currentBreakTimer:F1}");

        // konec pauzy
        if (currentBreakTimer <= 0)
        {
            isHavingBreak = false;
            GoToNextPatrolPoint();
        }
    }
    
// zase jenom pro editor
    private void OnDrawGizmos()
    {
        if (!debugMode) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRadius);

        Gizmos.color = Color.red;
        
        Vector3 lookDir = Application.isPlaying ? facingDirection : transform.up;

        Vector3 leftViewDir = Quaternion.Euler(0, 0, viewAngle / 2) * lookDir;
        Vector3 rightViewDir = Quaternion.Euler(0, 0, -viewAngle / 2) * lookDir;

        Gizmos.DrawLine(transform.position, transform.position + leftViewDir * visionRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightViewDir * visionRadius);
    }
    
    
    // --- ZMENA CHOVÁNÍ MIDGAME ---
    public void ChangeBehavior(MobBehavior newBehavior)
    {
        this.behavior = newBehavior;

        // pokud se zmeni na agresivni, okamzite se vypinaji vsechny prestavky a jde hledat hrace
        if (newBehavior == MobBehavior.Aggressive)
        {
            isHavingBreak = false;
            isWaitingAtPatrol = false; 
            isCoolingDown = false; 
            currentState = State.Chasing;
            searchTimer = 0f;
            lastKnownPlayerPos = playerPosition.position; 
            ChasePlayer();
        }
    }
    
    private void CompanionLogic()
    {
        if (isWaiting) 
        {
            // NPC stojí na místě (můžeš sem později hodit idle animaci)
            agent.ResetPath();
            return;
        }

        float distToPlayer = Vector3.Distance(transform.position, playerPosition.position);

        // Pokud je hráč moc daleko, najdeme si novou pozici v kruhu kolem něj
        if (distToPlayer > maxFollowDistance && !agent.pathPending)
        {
            agent.speed = runSpeed; // Pokud nestíhá, běží
        
            // Vygenerování náhodného bodu kolem hráče v povoleném radiusu
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            float randomDist = Random.Range(minFollowDistance, maxFollowDistance);
            Vector3 targetPos = playerPosition.position + new Vector3(randomDir.x, randomDir.y, 0) * randomDist;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(targetPos, out hit, 2.0f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }
        else if (distToPlayer <= minFollowDistance)
        {
            // Jsme dost blízko, jdeme normální rychlostí nebo zastavíme
            agent.speed = moveSpeed;
        }
    }
    
    
}