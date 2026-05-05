using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;
using System.Collections.Generic; // Zásadní pro List<>
using System.Linq;                // Zásadní pro .OrderBy()

[RequireComponent(typeof(SpriteRenderer))]
public class Mob_combat : MonoBehaviour
{
    
    private NPCController myIDCard;
    
    
    [Header("Combat Stats")]
    public int maxHealth = 10;
    private int currentHealth; 
    public bool isDead = false;
    
    [Header("Real-Time Combat Settings")]
    public bool canBeHitInRealTime = true;
    public bool startTurnBaseAfterHit = false;

    [Header("Encounter Settings")]
    public bool isTurnBasedMob = true;
    
    [ShowIf("isTurnBasedMob")]
    [Scene] 
    public string turnBasedScene; 

    [ShowIf("isTurnBasedMob")]
    public float encounterRadius = 1.5f;
    
    [Header("Databáze Nepřítele (Pro Turn-Based)")]
    public int turnBasedEnemyDatabaseID = 1; // TOHLE NASTAV V INSPEKTORU! Zastupuje to, co jsi dřív házel do listu

    [Header("Death Settings")]
    public Sprite deadMeatSprite; 
    public CorpseLoot lootScript; 

    private Animal_movement animalMovement;
    private SpriteRenderer spriteRenderer;
    private bool isTransitioning = false;
    
    // ZMĚNA: Přesunuto do Awake pro okamžitou dostupnost po Instantiate
    void Awake() 
    {
        animalMovement = GetComponent<Animal_movement>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        myIDCard = GetComponent<NPCController>(); // Získáme referenci na občanku
        currentHealth = maxHealth;
    }

    void Start()
    {
        // Kontrola z JSONu: Pokud se načteme zpátky a jsme mrtví, hned se změň na maso
        if (myIDCard != null && gameDataManager.currentGameData != null)
        {
            var state = gameDataManager.currentGameData.savedWorldNPCs.Find(n => n.uniqueID == myIDCard.uniqueID);
            if (state != null && state.isDead)
            {
                VisualDeath();
                return;
            }
        }

        if(lootScript != null && !isDead) 
            lootScript.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L) && !isDead)
        {
            Debug.Log("Cheater  zabil mobku stiskem klávesy L!");
            Die();
        }
        
        if (isDead || !isTurnBasedMob || animalMovement == null || animalMovement.playerPosition == null || isTransitioning || animalMovement.behavior != Ghost_movement.MobBehavior.Aggressive) 
            return;

        float distToPlayer = Vector3.Distance(transform.position, animalMovement.playerPosition.position);

        if (distToPlayer <= encounterRadius)
        {
            TriggerTurnBasedCombat();
        }
    }
    
    private void TriggerTurnBasedCombat()
    {
        if (string.IsNullOrEmpty(turnBasedScene)) return;
        isTransitioning = true; 
        
        // Najdeme všechny mobky v okruhu
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, encounterRadius);
        List<Mob_combat> validMobs = new List<Mob_combat>();

        foreach (var hit in hitColliders)
        {
            Mob_combat mob = hit.GetComponent<Mob_combat>();
            if (mob != null && mob.isTurnBasedMob && !mob.isDead && mob.myIDCard != null)
            {
                validMobs.Add(mob);
            }
        }

        // Seřadíme je podle vzdálenosti k TOHLEMU mobovi (nejbližší jdou do bitvy)
        validMobs = validMobs.OrderBy(m => Vector3.Distance(transform.position, m.transform.position)).ToList();

        // Omezíme to na max 3 mobky (1 tahle, co to spustila + 2 další)
        int mobsToPull = System.Math.Min(3, validMobs.Count);

        // Vyčistíme master paměť z minula
        gameDataManager.currentGameData.activeCombatNPCIDs.Clear();
        gameDataManager.currentGameData.activeCombatEnemyIDs.Clear();

        // Uložíme je do JSONu a pošleme do TurnBased scény
        for (int i = 0; i < mobsToPull; i++)
        {
            Mob_combat m = validMobs[i];
            m.myIDCard.SaveMyState(); // Ujistíme se, že má záznam

            var state = gameDataManager.currentGameData.savedWorldNPCs.Find(n => n.uniqueID == m.myIDCard.uniqueID);
            if (state != null) state.isInCombat = true;

            gameDataManager.currentGameData.activeCombatNPCIDs.Add(m.myIDCard.uniqueID);
            gameDataManager.currentGameData.activeCombatEnemyIDs.Add(m.turnBasedEnemyDatabaseID);
        }

        // MASTER SAVE a jde se do boje
        gameDataManager.SaveData();
        SceneManager.LoadScene(turnBasedScene);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"{gameObject.name} dostal za {damage}! Zbývá {currentHealth} HP.");

        if (animalMovement != null && animalMovement.behavior == Ghost_movement.MobBehavior.Neutral && isTurnBasedMob)
        {
            animalMovement.MakeAggressive();
        }
        
        if (currentHealth <= 0)
        {
            Die(); 
        }
    }

    private void VisualDeath()
    {
        isDead = true;
        if (deadMeatSprite != null && spriteRenderer != null) spriteRenderer.sprite = deadMeatSprite;
        if (animalMovement != null) Destroy(animalMovement); 
        if (lootScript != null) lootScript.enabled = true;
        this.enabled = false;
    }

    public void Die()
    {
        if (isDead) return;
        VisualDeath();
    
        // ZÁPIS DO MASTER SYSTÉMU:
        if (myIDCard != null)
        {
            myIDCard.isDead = true;
            myIDCard.SaveMyState(); 
        }
    }
    
    // pro encounter radius v editoru
    private void OnDrawGizmos()
    {
        if (isTurnBasedMob)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, encounterRadius);
        }
    }
}
