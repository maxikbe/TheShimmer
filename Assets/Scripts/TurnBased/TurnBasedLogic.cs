using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.UI;
//using System.Diagnostics;



[System.Serializable] 
public struct CameraInfo
{
    public int ID;
    public int IDofCamera;
    public Camera targetCamera;
}

[System.Serializable] 
public struct FacesSprite
{
    public bool isEnemy;
    public int ID;
    public Sprite sprite;
}
public class TurnBasedLogic : MonoBehaviour
{
    //turn Based ShortCuts
    private KeyCode keySpecial = KeyBoardSetting.chooseSpecialSpell;
    private KeyCode keyNormal = KeyBoardSetting.chooseNormalSpell;
    private KeyCode keyItem = KeyBoardSetting.chooseItem;
    private KeyCode keyAccept = KeyBoardSetting.doAccept;
    private KeyCode keyBack = KeyBoardSetting.doBack;
    private KeyCode keyUp = KeyBoardSetting.swapUp;
    private KeyCode keyDown = KeyBoardSetting.swapDown;
    private KeyCode keyLeft = KeyBoardSetting.swapLeft;
    private KeyCode keyRight = KeyBoardSetting.swapRight;
    private KeyCode keyAliveUp = KeyBoardSetting.swapAliveUp;
    private KeyCode keyAliveDown = KeyBoardSetting.swapAliveDown;

    // VARIABLES
    GameData data = new GameData(); 
    private List<Character> characters;
    private Character currentCharacter;
    private List<Enemy> enemies;
    private List<Enemy> currentEnemy;
    public List<int> whatEnemiesIsFighting = new List<int> { 1, 1, 1 };
    private int numberOfEnemies = 1;
    private int EnemyID;
    private int currentTurn;
    private List<TurnType> turnOrder = new List<TurnType>();
    [SerializeField] private List<Image> FaceHolders = new List<Image>();
    [SerializeField] private List<FacesSprite> Faces = new List<FacesSprite>();
    private int maxVisibleTurns = 5;
    enum TurnType{ Enemy, Enemy2, Enemy3, Player1, Player2, Player3, Player4, Player5 }

    [SerializeField] private List<GameObject> enemyPosition = new List<GameObject>();
    [SerializeField] private  List<GameObject> playerPosition = new List<GameObject>();
    private List<Vector3> defaultPlayerPositons = new List<Vector3>();
    private List<Vector3> defaultEnemyPositions = new List<Vector3>();
    [SerializeField] private List<CharacterAnimationData> characterAnimations = new List<CharacterAnimationData>();    
    [SerializeField] private List<EnemyAnimationData> enemyAnimations = new List<EnemyAnimationData>();
  

    // DEFAULT
    void Start()
    {
        characters = gameDataManager.currentGameData.characters;
        enemies = gameDataManager.currentGameData.enemies;
        Debug.Log("Characters loaded: " + string.Join(", ", characters.Select(c => c.name)));   
        inicializeTurnBasedGame();
    }

    void Update()
    {
       // if (Input.GetKey(keySpecial)) PlayerAttackEnemy(0, 0);
        //if (Input.GetKey(keyNormal)) EnemyAttackPlayer(0, 0);
       // Debug.Log("Current Turn Order: " + string.Join(", ", turnOrder.Select(t => t.ToString())));
    }

    // CAMERAS
    [SerializeField] private List<CameraInfo> camerasInfo = new List<CameraInfo>();
    private Camera currentActiveCamera;

    void SetActiveCamera(int cameraID)
    {
        foreach (var camInfo in camerasInfo) camInfo.targetCamera.enabled = false;

        var target = camerasInfo.FirstOrDefault(c => c.IDofCamera == cameraID);
        if (target.targetCamera != null)
        {
            target.targetCamera.enabled = true;
            currentActiveCamera = target.targetCamera;
        }
    }

    void SwitchToOverviewCamera()
    {
        SetActiveCamera(0);
    }

    void SwitchToPlayerCamera(int playerID)
    {
        SetActiveCamera(playerID);
    }

    void SwitchToEnemyCamera(int enemyID)
    {
        SetActiveCamera(enemyID);
    }

    // CODE

    void inicializeTurnBasedGame()
    {
        onTurnbasedStart();
        getDefaultPositions();
        CreateTurnOrder();
    }

    void onTurnbasedStart()
    {
        Debug.Log("Enemies: " + string.Join(", ", enemies.Select(e => e.name)));
        Debug.Log("Enemies IDs: " + string.Join(", ", enemies.Select(e => e.id)));
        Debug.Log("Enemies what is fighting: " + string.Join(", ", whatEnemiesIsFighting));
        currentEnemy = whatEnemiesIsFighting
            .Select(id => enemies.FirstOrDefault(e => e.id == id))
            .Where(e => e != null)
            .ToList();
        Debug.Log("Current enemy: " + string.Join(", ", currentEnemy.Select(e => e.name)));
    }
    void CreateTurnOrder()
    {
        turnOrder.Clear();
        
        List<TurnType> enemyPool = new List<TurnType>();
        for (int i = 0; i < currentEnemy.Count; i++)
        {
            if (i == 0) enemyPool.Add(TurnType.Enemy);
            else if (i == 1) enemyPool.Add(TurnType.Enemy2);
            else if (i == 2) enemyPool.Add(TurnType.Enemy3);
        }

        List<TurnType> playerPool = characters
            .Where(c => !c.isDead)
            .Select(c => {
                if (c.id == 1) return TurnType.Player1;
                if (c.id == 2) return TurnType.Player2;
                if (c.id == 3) return TurnType.Player3;
                if (c.id == 4) return TurnType.Player4;
                return TurnType.Player5;
            })
            .ToList();

        if (enemyPool.Count == 0 && playerPool.Count == 0) return;

        int enemyIndex = 0;
        int playerIndex = 0;

        while (turnOrder.Count < 500)
        {
            if (enemyPool.Count > 0)
            {
                turnOrder.Add(enemyPool[enemyIndex]);
                enemyIndex = (enemyIndex + 1) % enemyPool.Count;
            }

            if (turnOrder.Count >= 500) break;

            if (playerPool.Count > 0)
            {
                int playersToAdd = Random.Range(2, 5); 
                for (int i = 0; i < playersToAdd; i++)
                {
                    if (turnOrder.Count >= 500) break;
                    turnOrder.Add(playerPool[playerIndex]);
                    playerIndex = (playerIndex + 1) % playerPool.Count;
                }
            }
        }
        UpdateFaces();
    }

    void UpdateTurnOrder(int idWhoDied, bool isEnemy)
    {
        TurnType turnToRemove = TurnType.Player1; 

        if (isEnemy)
        {
            if (idWhoDied == 1) turnToRemove = TurnType.Enemy;
            else if (idWhoDied == 2) turnToRemove = TurnType.Enemy2;
            else if (idWhoDied == 3) turnToRemove = TurnType.Enemy3;
        }
        else
        {
            if (idWhoDied == 1) turnToRemove = TurnType.Player1;
            else if (idWhoDied == 2) turnToRemove = TurnType.Player2;
            else if (idWhoDied == 3) turnToRemove = TurnType.Player3;
            else if (idWhoDied == 4) turnToRemove = TurnType.Player4;
            else if (idWhoDied == 5) turnToRemove = TurnType.Player5;
        }

        turnOrder.RemoveAll(t => t == turnToRemove);
        UpdateFaces();
    }

    void UpdateFaces()
    {
        for (int i = 0; i < FaceHolders.Count; i++)
        {
            if (i >= turnOrder.Count)
            {
                FaceHolders[i].gameObject.SetActive(false);
                continue;
            }

            FaceHolders[i].gameObject.SetActive(true);
            TurnType turn = turnOrder[i];
            
            bool lookingForEnemy = false;
            int targetID = 0;
            if (turn == TurnType.Enemy || turn == TurnType.Enemy2 || turn == TurnType.Enemy3)
            {
                lookingForEnemy = true;
                int enemyIdx = (int)turn; 
                if (enemyIdx < currentEnemy.Count) targetID = currentEnemy[enemyIdx].id;
            }
            else 
            {
                lookingForEnemy = false;
                if (turn == TurnType.Player1) targetID = 1;
                else if (turn == TurnType.Player2) targetID = 2;
                else if (turn == TurnType.Player3) targetID = 3;
                else if (turn == TurnType.Player4) targetID = 4;
                else if (turn == TurnType.Player5) targetID = 5;
            }

            FacesSprite foundFace = Faces.FirstOrDefault(f => f.isEnemy == lookingForEnemy && f.ID == targetID);

            if (foundFace.sprite != null)
            {
                FaceHolders[i].sprite = foundFace.sprite;
            }
        }
    }

    void getDefaultPositions()
    {
        defaultEnemyPositions = enemyPosition.Select(pos => pos.transform.position).ToList();
        defaultPlayerPositons = playerPosition.Select(pos => pos.transform.position).ToList();
    }

 

    Vector3 getAnimationPositions(int enemyPositionIndex, int playerPositionIndex, bool isPlayerAttacking)
    {
        if(isPlayerAttacking)
        {
            Vector3 enemyPos = defaultEnemyPositions[enemyPositionIndex];
            Vector3 playerPos = defaultPlayerPositons[playerPositionIndex];

            return Vector3.Lerp(playerPos, enemyPos, 0.65f);
        }
        else
        {
            Vector3 enemyPos = defaultEnemyPositions[enemyPositionIndex];
            Vector3 playerPos = defaultPlayerPositons[playerPositionIndex];
            
            return Vector3.Lerp(enemyPos, playerPos, 0.65f);
        }
    }

    void MovePlayerBackToPosition(int playerPositionIndex)
    {
        Vector3 targetPosition = defaultPlayerPositons[playerPositionIndex];
    }

    void MoveEnemyBackToPosition(int enemyPositionIndex)
    {
        Vector3 targetPosition = defaultEnemyPositions[enemyPositionIndex];
    }

    void PlayerAttackEnemy(int playerPositionIndex, int enemyPositionIndex)
    {
        // Move player to attack position

        Vector3 targetPosition = getAnimationPositions(enemyPositionIndex, playerPositionIndex, true);
        playerPosition[playerPositionIndex].transform.position = targetPosition;

        // Trigger attack animation here and apply damage to enemy


        // After animation, move player back

        //MovePlayerBackToPosition(playerPositionIndex);
    }

    void EnemyAttackPlayer(int enemyPositionIndex, int playerPositionIndex)
    {
        // Move enemy to attack position

        Vector3 targetPosition = getAnimationPositions(enemyPositionIndex, playerPositionIndex, false);
        enemyPosition[enemyPositionIndex].transform.position = targetPosition;

        // Trigger attack animation here and apply damage to player

        // After animation, move enemy back

        MoveEnemyBackToPosition(enemyPositionIndex);
    }

    void HandlePlayerAttack()
    {
        
    }

    
}