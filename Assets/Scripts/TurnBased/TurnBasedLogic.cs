using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.Intrinsics;



public class TurnBasedLogic : MonoBehaviour
{
    GameData data = new GameData(); 
    private List<Character> characters;
    private Character currentCharacter;
    private List<Enemy> enemies = new List<Enemy>();
    private Enemy currentEnemy;
    private int numberOfEnemies = 1;
    private int EnemyID;
    private int currentTurn;
    private List<TurnType> turnOrder = new List<TurnType>();
    private int maxVisibleTurns = 5;
    enum TurnType
    {
        Enemy,
        Enemy2,
        Enemy3,
        Player1,
        Player2,
        Player3,    
        Player4,
        Player5,
        Animation
    }

    [SerializeField] private List<GameObject> enemyPosition = new List<GameObject>();
    [SerializeField] private  List<GameObject> playerPosition = new List<GameObject>();
    private List<Vector3> defaultPlayerPositons = new List<Vector3>();
    private List<Vector3> defaultEnemyPositions = new List<Vector3>();
    [SerializeField] private List<CharacterAnimationData> characterAnimations = new List<CharacterAnimationData>();    


    void inicializeTurnBasedGame()
    {
        onTurnbasedStart();
        getDefaultPositions();
        CreateTurnOrder();
    }

    void onTurnbasedStart()
    {
        currentEnemy = enemies[currentTurn];
    }

    void CreateTurnOrder()
    {
        turnOrder.Clear();
        int chance = Random.Range(0, 2);
        int enemyTurns = numberOfEnemies == 0 ? 1 : chance == 1 ? (numberOfEnemies == 2 ? Random.Range(2, 4) : Random.Range(3, 6)) : 0;
        for (int i = 0; i < enemyTurns; i++) turnOrder.Add(TurnType.Enemy);
    
        List<Character> activeCharacters = characters.Where(c => !c.isDead).ToList();
        activeCharacters = activeCharacters.OrderByDescending(c => c.speed).ToList();

        foreach (var character in activeCharacters)
        {
            turnOrder.Add((TurnType)character.id);
        }
        
        Debug.Log("Turn order: " + string.Join(", ", turnOrder));
    }

    void getDefaultPositions()
    {
        defaultEnemyPositions = enemyPosition.Select(pos => pos.transform.position).ToList();
        defaultPlayerPositons = playerPosition.Select(pos => pos.transform.position).ToList();
    }

    

    void Start()
    {
        characters = gameDataManager.currentGameData.characters;
        Debug.Log("Characters loaded: " + string.Join(", ", characters.Select(c => c.name)));   
        CreateTurnOrder();
    }

    Vector3 getAnimationPositions(int enemyPositionIndex, int playerPositionIndex, bool isPlayerAttacking)
    {
        if(isPlayerAttacking)
        {
            Vector3 enemyPos = defaultEnemyPositions[enemyPositionIndex];
            Vector3 playerPos = defaultPlayerPositons[playerPositionIndex];

            return (enemyPos + playerPos) / 2f;
        }
        else
        {
            Vector3 enemyPos = defaultEnemyPositions[enemyPositionIndex];
            Vector3 playerPos = defaultPlayerPositons[playerPositionIndex];
            
            return (enemyPos + playerPos) / 2f;
        }
    }

    void MovePlayerBackToPosition(int playerPositionIndex)
    {
        Vector3 targetPosition = defaultPlayerPositons[playerPositionIndex];
    }

    void PlayerAttackEnemy(int playerPositionIndex, int enemyPositionIndex)
    {
        // Move player to attack position

        Vector3 targetPosition = getAnimationPositions(enemyPositionIndex, playerPositionIndex, true);
        playerPosition[playerPositionIndex].transform.position = targetPosition;

        // Trigger attack animation here and apply damage to enemy


        // After animation, move player back

        MovePlayerBackToPosition(playerPositionIndex);
    }

    
}