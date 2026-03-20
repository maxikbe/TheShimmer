using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Reflection;

public class TurnBasedLogic : MonoBehaviour
{
    private GameData gameData; 
    private List<Character> characters = new List<Character>();
    private Character currentCharacter;
    private List<Enemy> enemies = new List<Enemy>();
    private Enemy currentEnemy;
    private int EnemyID;
    private int currentTurn;
    private List<type> turnOrder = new List<type>();
    enum type
    {
        Enemy,
        Player,
        Animation
    }
    
    void onTurnbasedStart()
    {
        currentEnemy = enemies[currentTurn];
    }
   
}