using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.UI;
using TMPro;
using NUnit.Framework;
using UnityEngine.Rendering.Universal;

//using System.Diagnostics;

[System.Serializable] 
public struct CameraInfo
{
    public int ID;
    public int IDofCamera;
    public Camera targetCamera;
    public float zoomMultiplier;
}

[System.Serializable] 
public struct FacesSprite
{
    public bool isEnemy;
    public int ID;
    public Sprite sprite;
}

[System.Serializable] 
public struct characterBars
{
    public int ID;
    public Image healthBar;
    public Image manaBar;
}

[System.Serializable]
public struct BackgroundPicture
{
    public int ID;
    public string name;
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
    [SerializeField] private Database _databaseReference;
    [SerializeField] private SkillDatabase _skillDatabaseReference;
    private static Database itemDatabase;
    private static SkillDatabase skillDatabase;
    private List<Skills> currentCharacterSkills;
    private List<Character> characters;
    private Character currentCharacter;
    private bool isPlayerTurn;
    private bool isPlayerChoosing = false;
    private int currentSkillID;
    private List<Enemy> enemies;
    private List<Enemy> currentEnemy;
    public List<int> whatEnemiesIsFighting = new List<int> { 1, 1, 1 };
    private int currentBackgroundPictureID = 1;
    private int currentArrow = 0;
    private bool isChoosingEnemy = false;
    private int EnemyID;
    private int currentTurn;
    private int currentTypeAttack;
    private List<TurnType> turnOrder = new List<TurnType>();

    private int maxVisibleTurns = 5;
    enum TurnType{ Enemy, Enemy2, Enemy3, Player1, Player2, Player3, Player4, Player5 }

    [SerializeField] private List<GameObject> enemyPosition = new List<GameObject>();
    [SerializeField] private  List<GameObject> playerPosition = new List<GameObject>();
    private List<Vector3> defaultPlayerPositons = new List<Vector3>();
    private List<Vector3> defaultEnemyPositions = new List<Vector3>();
    [SerializeField] private List<CharacterAnimationData> characterAnimations = new List<CharacterAnimationData>();    
    [SerializeField] private List<EnemyAnimationData> enemyAnimations = new List<EnemyAnimationData>();

    // UI
    [SerializeField] private List<Image> FaceHolders = new List<Image>();
    [SerializeField] private List<FacesSprite> Faces = new List<FacesSprite>();
    [SerializeField] private TextMeshProUGUI EnemyName;
    [SerializeField] private Image EnemyHealthBar;
    [SerializeField] private List<characterBars> characterBars = new List<characterBars>();
    [SerializeField] private List<BackgroundPicture> BackgroundPictures = new List<BackgroundPicture>();
    [SerializeField] private SpriteRenderer BackgroundPicture; 
    [SerializeField] private GameObject chooseMenu;
    [SerializeField] private GameObject chooserThingsMenu;
    [SerializeField] private GameObject ChooseAttackUI;
    [SerializeField] private GameObject button;
    [SerializeField] private List<GameObject> arrowsCharacters = new List<GameObject>();
    [SerializeField] private List<GameObject> arrowsEnemies = new List<GameObject>();
    [SerializeField] private List<GameObject> enemyUIs = new List<GameObject>();
    [SerializeField] private List<GameObject> characterUIs = new List<GameObject>();

    // DEFAULT
    void Start()
    {
        characters = gameDataManager.currentGameData.characters;
        enemies = gameDataManager.currentGameData.enemies;
        itemDatabase = _databaseReference;
        skillDatabase = _skillDatabaseReference;
        Debug.Log("Characters loaded: " + string.Join(", ", characters.Select(c => c.name)));   
        inicializeTurnBasedGame();
    }

    void Update()
    {
       // if (Input.GetKey(keySpecial)) PlayerAttackEnemy(0, 0);
        //if (Input.GetKey(keyNormal)) EnemyAttackPlayer(0, 0);
       // Debug.Log("Current Turn Order: " + string.Join(", ", turnOrder.Select(t => t.ToString())));
        if (Input.GetKeyDown(KeyCode.F)) nextTurn();
        if (Input.GetKeyDown(KeyCode.Z)) SetActiveCamera(8);
        if (Input.GetKeyDown(keyDown) && isChoosingEnemy && isPlayerChoosing)
        {
            currentArrow = (currentArrow + 1) % arrowsEnemies.Count;
            ShowArrow(arrowsEnemies, currentArrow);
        }
        if (Input.GetKeyDown(keyUp) && isChoosingEnemy && isPlayerChoosing)
        {
            currentArrow = (currentArrow - 1 + arrowsEnemies.Count) % arrowsEnemies.Count;
            ShowArrow(arrowsEnemies, currentArrow);
        }
        if(Input.GetKeyDown(keyAccept) && isChoosingEnemy && isPlayerChoosing) handlePlayerAttack();
        if (Input.GetKeyDown(keyBack) && isChoosingEnemy && isPlayerChoosing)
        {
            handleSelectionBack();
        }
    }

    // CAMERAS
    [SerializeField] private List<CameraInfo> camerasInfo = new List<CameraInfo>();
    private Camera currentActiveCamera;
    public float transitionDuration = 0.5f; 
    public float zoomStartFOV = 80f;        
    public float zoomEndFOV = 60f;
    private int defaultPPU;
    private Coroutine activeAnimation;
    private Dictionary<Camera, Vector3> originalPositions = new Dictionary<Camera, Vector3>();
    void SetActiveCamera(int cameraID)
    {
        var target = camerasInfo.FirstOrDefault(c => c.IDofCamera == cameraID);
        if (target.targetCamera == null)
        {
            Debug.LogWarning($"Kamera s ID {cameraID} nenalezena!");
            return;
        }

        if (activeAnimation != null) StopCoroutine(activeAnimation);
        activeAnimation = StartCoroutine(AnimateToCamera(target.targetCamera, target.zoomMultiplier));
    }

    System.Collections.IEnumerator AnimateToCamera(Camera targetCam, float zoomMultiplier)
    {
        Camera mainCam = camerasInfo.FirstOrDefault(c => c.IDofCamera == 0).targetCamera;
        var ppCam = mainCam.GetComponent<UnityEngine.Rendering.Universal.PixelPerfectCamera>();

        Vector3 startPos = mainCam.transform.position;
        Quaternion startRot = mainCam.transform.rotation;
        int startPPU = ppCam.assetsPPU;

        Vector3 endPos = targetCam.transform.position;      
        Quaternion endRot = targetCam.transform.rotation;    
        int endPPU = zoomMultiplier > 0 ? Mathf.RoundToInt(defaultPPU * zoomMultiplier)  : defaultPPU;

        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / transitionDuration);

            mainCam.transform.position = Vector3.Lerp(startPos, endPos, t);
            mainCam.transform.rotation = Quaternion.Lerp(startRot, endRot, t);
            ppCam.assetsPPU = Mathf.RoundToInt(Mathf.Lerp(startPPU, endPPU, t));

            yield return null;
        }

        mainCam.transform.position = endPos;
        mainCam.transform.rotation = endRot;
        ppCam.assetsPPU = endPPU;
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
        createTurnOrder();
        updateEnemyHealthBar();
        updateCharacterBars();
        foreach (var info in camerasInfo)
        {
            if (info.targetCamera != null)
            {
                originalPositions[info.targetCamera] = info.targetCamera.transform.position;
            }
        }
        var ppCam = camerasInfo.FirstOrDefault(c => c.IDofCamera == 0).targetCamera
            .GetComponent<UnityEngine.Rendering.Universal.PixelPerfectCamera>();
        defaultPPU = ppCam.assetsPPU;
        SetActiveCamera(0);
        EnemyName.text = currentEnemy[0].name;
        BackgroundPicture.sprite = BackgroundPictures[currentBackgroundPictureID].sprite;
    }

    void onTurnbasedStart()
    {
        Debug.Log("Enemies: " + string.Join(", ", enemies.Select(e => e.name)));
        Debug.Log("Enemies IDs: " + string.Join(", ", enemies.Select(e => e.id)));
        Debug.Log("Enemies what is fighting: " + string.Join(", ", whatEnemiesIsFighting));
        currentEnemy = whatEnemiesIsFighting
            .Select((id, index) => {
                Enemy original = enemies.FirstOrDefault(en => en.id == id);
                if (original == null) return null;
                return new Enemy {
                    id = original.id,
                    name = original.name,
                    health = original.health,
                    maxHealth = original.maxHealth,
                    isDead = original.isDead,
                    sprite = original.sprite,
                    attacks = original.attacks.Select(a => new EnemyAttack {
                        id = a.id,
                        attackName = a.attackName,
                        totalAnimationDuration = a.totalAnimationDuration,
                        weight = a.weight,
                        hits = a.hits.Select(h => new Hit {
                            timeOffset = h.timeOffset,
                            damage = h.damage
                        }).ToList(),
                        animations = a.animations 
                    }).ToList()
                };
            })
            .Where(e => e != null)
            .ToList();
        Debug.Log("Current enemy: " + string.Join(", ", currentEnemy.Select(e => e.name)));
    }
    void createTurnOrder()
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
        if (turnOrder[0] != TurnType.Player1 && turnOrder[0] != TurnType.Player2 && turnOrder[0] != TurnType.Player3 && turnOrder[0] != TurnType.Player4 && turnOrder[0] != TurnType.Player5) { isPlayerTurn = false; return;}
        else isPlayerTurn = true;
        currentCharacter = characters.FirstOrDefault(c => 
            (turnOrder[0] == TurnType.Player1 && c.id == 1) ||
            (turnOrder[0] == TurnType.Player2 && c.id == 2) ||
            (turnOrder[0] == TurnType.Player3 && c.id == 3) ||
            (turnOrder[0] == TurnType.Player4 && c.id == 4) ||
            (turnOrder[0] == TurnType.Player5 && c.id == 5) 
        );
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

    public void nextTurn()
    {
        if (turnOrder.Count == 0) return;

        TurnType currentTurn = turnOrder[0];
        turnOrder.RemoveAt(0);
        UpdateFaces();
        Debug.Log("Current Turn: " + currentTurn);
        if (turnOrder[0] != TurnType.Player1 && turnOrder[0] != TurnType.Player2 && turnOrder[0] != TurnType.Player3 && turnOrder[0] != TurnType.Player4 && turnOrder[0] != TurnType.Player5) return;
        currentCharacter = characters.FirstOrDefault(c => 
            (turnOrder[0] == TurnType.Player1 && c.id == 1) ||
            (turnOrder[0] == TurnType.Player2 && c.id == 2) ||
            (turnOrder[0] == TurnType.Player3 && c.id == 3) ||
            (turnOrder[0] == TurnType.Player4 && c.id == 4) ||
            (turnOrder[0] == TurnType.Player5 && c.id == 5) 
        );
        SwitchToPlayerCamera(currentCharacter.id);
        ChooseMenu();
        Debug.Log("Current Character: " + currentCharacter.name);
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

    void handlePlayerAttack()
    {
        HideArrow();
        Debug.Log("Current Arrow: " + currentArrow);
        switch (currentTypeAttack)
        {
            case 0:
            {
                var targetEnemy = currentEnemy[currentArrow];
                targetEnemy.health -= currentCharacter.attack;
                characters[currentCharacter.id].mana += 5;
                if (characters[currentCharacter.id].mana > 10) characters[currentCharacter.id].mana = 10;
                Debug.Log("CHAR MANA " + characters[currentCharacter.id].mana );
                if (targetEnemy.health <= 0) { targetEnemy.isDead = true; UpdateTurnOrder(targetEnemy.id, true); }
                updateEnemyHealthBar();
                updateCharacterBars();
                break;
            }
            case 1:
            {
                Skills currentSkill = skillDatabase.GetSkillByID(currentSkillID);
                switch (currentSkill.type)
                {
                    case skillType.Damage:
                    {
                        var targetEnemy = currentEnemy[currentArrow];
                        Debug.Log($"{targetEnemy.name} health: {targetEnemy.health}");
                        targetEnemy.health -= currentSkill.amount;
                        Debug.Log($"{targetEnemy.name} health: {targetEnemy.health}\nTotal enemies: {currentEnemy.Count}");

                        if (targetEnemy.health <= 0)
                        {
                            targetEnemy.isDead = true;
                            UpdateTurnOrder(targetEnemy.id, true);
                        }
                        updateEnemyHealthBar();
                        break;
                    }
                    case skillType.Heal:
                        currentCharacter.health += currentSkill.amount;
                        updateCharacterBars();
                        break;

                    case skillType.Mana:
                        currentCharacter.mana += currentSkill.amount;
                        updateCharacterBars();
                        break;

                    case skillType.Buff:
                        Debug.Log("Buff");
                        break;

                    default:
                        Debug.Log("Unknown skill type");
                        break;
                }
                break;
            }
            default:
                break;
        }
        isPlayerChoosing = false;
        isChoosingEnemy = false;
        HideArrow();
        nextTurn();
    }
    public void basicAttack()
    {
        Debug.Log("Basic attack");
        isPlayerChoosing = true;
        isChoosingEnemy = true;
        arrowsEnemies[currentArrow].SetActive(true);         
        currentTypeAttack = 0;
        handleSkillItemClosing();
        ChooseMenu();
        SetActiveCamera(8);
    }

    
    void handleSelection(bool isChoosingEnemyInput, int currentTypeAttackInput)
    {
        Skills currentSkill = skillDatabase.GetSkillByID(currentSkillID);
        if (currentSkill.manaCost > currentCharacter.mana)
        {
            Debug.Log("Not enough mana!");
            return;
        }
        isPlayerChoosing = true;
        currentArrow = 0;
        currentTypeAttack = currentTypeAttackInput;
        handleSkillItemClosing();
        ChooseMenu();
        SetActiveCamera(8);
        if (isChoosingEnemyInput)
        {
            isChoosingEnemy = true;
            arrowsEnemies[currentArrow].SetActive(true);         
        }
        else
        {
            isChoosingEnemy = false;
            arrowsCharacters[0].SetActive(true);
            Debug.Log("Choosing character...");
        }
    }

    void handleSelectionBack()
    {
        isPlayerChoosing = false;
        isChoosingEnemy = false;
        arrowsEnemies.ForEach(a => a.SetActive(false));
        arrowsCharacters.ForEach(a => a.SetActive(false));
        SwitchToPlayerCamera(currentCharacter.id);
        ChooseMenu();
        handleSkillOpening();
        HideArrow();
    }


    // UI FUNCTIONS

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
    void updateEnemyHealthBar()
    {
        int maxHealth = 0;
        int currentHealth = 0;
        for (int i = 0; i < currentEnemy.Count; i++)
        {
            maxHealth += currentEnemy[i].maxHealth;
            currentHealth += currentEnemy[i].health;
            Debug.Log("Current health: " + currentEnemy[i].health);
        }
        EnemyHealthBar.fillAmount = currentHealth / (float)maxHealth;
    }
    
    void updateCharacterBars()
    {
        int i = 0;
        foreach (characterBars bars in characterBars)
        {
        
            bars.healthBar.fillAmount = characters[i].health / (float)characters[i].maxHealth;
            bars.manaBar.fillAmount = characters[i].mana / 10;
            Debug.Log("CHAR MANA " + characters[i].mana);
            i++;
        }
    }
    private void ChooseMenu()
    {
        if(chooseMenu.activeSelf) chooseMenu.SetActive(false);
        else chooseMenu.SetActive(true);
    }

    public void handleSkillOpening()
    {
        handleSkillItemClosing();
        chooserThingsMenu.SetActive(true);
        if (currentCharacter != null && skillDatabase != null)
        {
            currentCharacterSkills = skillDatabase.GetAllSkills()
                .Where(s => s.characterID == currentCharacter.id)
                .ToList();
            
            Debug.Log("Current character skills: " + string.Join(", ", currentCharacterSkills.Select(s => s.name)));

            foreach (var skill in currentCharacterSkills)
            {
                GameObject skillButton = Instantiate(button, chooserThingsMenu.transform);
                skillButton.GetComponentInChildren<TextMeshProUGUI>().text = skill.name;
                Button skillButtonBtn = skillButton.GetComponent<Button>();
                skillButtonBtn.onClick.AddListener(() =>{
                    if (skill.type == skillType.Damage) handleSelection(true, 1);   
                    else handleSelection(false, 1);
                    currentSkillID = skill.id;
                });
            }
        }

    }

    public void handleSkillItemClosing()
    {
        foreach (Transform child in chooserThingsMenu.transform)
        {
            Destroy(child.gameObject); 
        }
    }

    private void ShowArrow(List<GameObject> arrows, int index)
    {
        foreach (var arrow in arrows)
            arrow.SetActive(false);
        
        arrows[index].SetActive(true);
    }

    private void HideArrow()
    {
        arrowsEnemies.ForEach(a => a.SetActive(false));
        arrowsCharacters.ForEach(a => a.SetActive(false));
    }

    public void handleItemOpening()
    {
        chooserThingsMenu.SetActive(true);   
    }
    
    public void openChooseAttackUI()
    {
        ChooseAttackUI.SetActive(true);
    }

    public void closeChooseAttackUI()
    {
        ChooseAttackUI.SetActive(false);
    }
}