using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;

[RequireComponent(typeof(SpriteRenderer))]
public class Mob_combat : MonoBehaviour
{
    [Header("Combat Stats")]
    public int maxHealth = 10;
    private int currentHealth; 
    public bool isDead = false;

    [Header("Encounter Settings")]
    public bool isTurnBasedMob = true;
    
    [ShowIf("isTurnBasedMob")]
    [Scene] 
    public string turnBasedScene; 

    [ShowIf("isTurnBasedMob")]
    public float encounterRadius = 1.5f;

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
        currentHealth = maxHealth;
    }

    void Start()
    {
        if(lootScript != null && !isDead) 
            lootScript.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L) && !isDead)
        {
            Debug.Log("Cheater Kokkott zabil mobku stiskem klávesy L!");
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
        
        // ULOŽENÍ GAME MANAGER MOBKY A SPUSTENI
        // GameStateManager.Instance.lastEngagedMobID = ...
        
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

    // api metoda pro chabra
    public void Die()
    {
        if (isDead) return;

        isDead = true;
        currentHealth = 0; 
        Debug.Log($"{gameObject.name} zařval. Můžeš lootovat.");

        //  Změna textury na maso
        if (deadMeatSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = deadMeatSprite;
        }

        // vypnutí AI pohybu
        if (animalMovement != null)
        {
            Destroy(animalMovement); 
        }

        // zapne looting script
        if (lootScript != null)
        {
            lootScript.enabled = true;
        }
        
        // vypne combat script
        this.enabled = false; 
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
