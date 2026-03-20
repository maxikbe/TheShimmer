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
    private int EnemyID;
    private int currentTurn;
    private List<TurnType> turnOrder = new List<TurnType>();
    private int maxVisibleTurns = 5;
    enum TurnType
    {
        Enemy,
        Player1,
        Player2,
        Player3,    
        Player4,
        Player5,
        Animation
    }
    
    void onTurnbasedStart()
    {
        currentEnemy = enemies[currentTurn];
    }

    void CreateTurnOrder()
    {
        turnOrder.Clear();
        int enemyTurns = Random.Range(1, 4);
        for (int i = 0; i < enemyTurns; i++) turnOrder.Add(TurnType.Enemy);
    
        List<Character> activeCharacters = characters.Where(c => !c.isDead).ToList();
        activeCharacters = activeCharacters.OrderByDescending(c => c.speed).ToList();

        foreach (var character in activeCharacters)
        {
            turnOrder.Add((TurnType)character.id);
        }
        
        Debug.Log("Turn order: " + string.Join(", ", turnOrder));
    }

    void inicializeGame()
    {
        
    }
    void Start()
    {
        characters = gameDataManager.currentGameData.characters;
        Debug.Log("Characters loaded: " + string.Join(", ", characters.Select(c => c.name)));   
        CreateTurnOrder();
    }
}