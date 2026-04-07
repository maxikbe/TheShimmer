using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;

public class Mob_combat : MonoBehaviour
{
    [Header("Combat Stats")]
    public int maxHealth = 10;
    private int currentHealth;

    [Header("Encounter Settings")]
    public bool isTurnBasedMob = true;
    
    [ShowIf("isTurnBasedMob")]
    [Scene] 
    public string turnBasedScene; 

    [ShowIf("isTurnBasedMob")]
    public float encounterRadius = 1.5f; // vzdalenost pro turn based combat

    // pozice hráče
    private Animal_movement animalMovement;
    private bool isTransitioning = false; // at se scena nenacita 60x za vterinu
    
    void Start()
    {
        currentHealth = maxHealth;
        
        animalMovement = GetComponent<Animal_movement>();
    }

    
    void Update()
    {
           
        if (!isTurnBasedMob || animalMovement == null || animalMovement.playerPosition == null || isTransitioning || animalMovement.behavior != Ghost_movement.MobBehavior.Aggressive) 
            return;

        // vzdalenost k hraci
        float distToPlayer = Vector3.Distance(transform.position, animalMovement.playerPosition.position);

        // spousti turnbased
        if (distToPlayer <= encounterRadius)
        {
            TriggerTurnBasedCombat();
        }
    }
    
    private void TriggerTurnBasedCombat()
    {
        if (string.IsNullOrEmpty(turnBasedScene))
        {
            Debug.LogError("Nemáš nastavenou scénu u mobky!");
            return;
        }

        isTransitioning = true; // aby nespamoval update
        Debug.Log("Načítám scénu...");
        
        SceneManager.LoadScene(turnBasedScene);
    }

    // --- KLASICKÝ COMBAT ---
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} dostal za {damage}! Zbývá {currentHealth} HP.");

        if (animalMovement != null && animalMovement.behavior == Ghost_movement.MobBehavior.Neutral && isTurnBasedMob)
        {
            animalMovement.MakeAggressive();
            Debug.Log("Z neutrální mobky je agresivní!");
        }
        
        
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
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
