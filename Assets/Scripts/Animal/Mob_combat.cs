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
    public float encounterRadius = 1.5f; // Jak blízko musí být, aby začal boj

    // Reference na tvůj hlavní script, abychom z něj vytáhli pozici hráče
    private Animal_movement animalMovement;
    private bool isTransitioning = false; // Pojistka, ať nenačítáš scénu 60x za vteřinu
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        // Najdeme si Animal_movement na stejném objektu
        animalMovement = GetComponent<Animal_movement>();
    }

    
    void Update()
    {
           
        // Pokud nejsme tahová mobka, nebo chybí reference, nebo už načítáme, nic neděláme
        if (!isTurnBasedMob || animalMovement == null || animalMovement.playerPosition == null || isTransitioning || animalMovement != null || animalMovement.behavior != Ghost_movement.MobBehavior.Aggressive) 
            return;

        // TADY JE TVOJE DETEKCE
        float distToPlayer = Vector3.Distance(transform.position, animalMovement.playerPosition.position);

        // Pokud je hráč blíž nebo přesně na hranici radiusu
        if (distToPlayer <= encounterRadius)
        {
            TriggerTurnBasedCombat();
        }
    }
    
    private void TriggerTurnBasedCombat()
    {
        // Pojistka pro prázdnou scénu
        if (string.IsNullOrEmpty(turnBasedScene))
        {
            Debug.LogError("Nemáš nastavenou scénu u mobky!");
            return;
        }

        isTransitioning = true; // Zabráníme tomu, aby Update spamoval LoadScene
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

    // Abychom viděli encounter radius v editoru, podobně jako máš u visionRadius
    private void OnDrawGizmos()
    {
        if (isTurnBasedMob)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, encounterRadius);
        }
    }
}
