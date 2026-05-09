using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.Universal;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct EnemeyInfo
{
    public int ID;
    public string name;
    public Sprite sprite;
    public Sprite TurnBasedIcon;
}

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

[System.Serializable]
public struct MusicEnemy
{
    public int ID;
    public AudioClip music;
}

public class TurnBasedLogic : MonoBehaviour
{
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
    private KeyCode keyJump = KeyBoardSetting.jump;
    private KeyCode keyDodge = KeyBoardSetting.dodge;
    private KeyCode keyParry = KeyBoardSetting.parry;

    GameData data = new GameData();
    
    [Header("Návratová scéna")]
    [Scene]
    [SerializeField] private string mainWorldScene;
    
    

    [SerializeField] private Database _databaseReference;
    [SerializeField] private SkillDatabase _skillDatabaseReference;
    [SerializeField] private PerksDatabase _perksDatabaseReference;
    [SerializeField] private GameObject starterUI;
    private static Database itemDatabase;
    private static SkillDatabase skillDatabase;
    private static PerksDatabase perksDatabase;

    [SerializeField] private TextMeshProUGUI nextAttackText;
    [SerializeField] private float baseCritChance = 0.05f;
    [SerializeField] private float critDamageMultiplier = 2f;
    [SerializeField] private Color critTextColor = new Color(1f, 0.4f, 0f);
    [SerializeField] private TextMeshProUGUI critIndicatorText;
    [SerializeField] private float critIndicatorDuration = 0.8f;
    [SerializeField] private AudioClip critSoundEffect;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private string parryAnimationTrigger = "Parry";
    [SerializeField] private string dodgeAnimationTrigger = "Dodge";
    [SerializeField] private Image flashImage;
    [SerializeField] private TextMeshProUGUI EnemyName;
    [SerializeField] private Image EnemyHealthBar;
    [SerializeField] private SpriteRenderer BackgroundPicture;
    [SerializeField] private GameObject chooseMenu;
    [SerializeField] private GameObject chooserThingsMenu;
    [SerializeField] private GameObject ChooseAttackUI;
    [SerializeField] private GameObject button;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject victoryPanel;

    [SerializeField] private TextMeshProUGUI parryDodgePromptText;

    [SerializeField] private GameObject infoUI;
    [SerializeField] private TextMeshProUGUI infoNameText;
    [SerializeField] private TextMeshProUGUI infoDescriptionText;
    

    public static List<int> whatEnemiesIsFighting = new List<int> { 1, 1, 2 };
    private List<Skills> currentCharacterSkills;
    private List<Character> characters;
    private Character currentCharacter;
    private List<Enemy> enemies;
    private List<Enemy> currentEnemy;

    private Dictionary<int, float> characterCritBonus = new Dictionary<int, float>();
    private Dictionary<int, int> characterDamageBonus = new Dictionary<int, int>();
    private Dictionary<int, int> characterArmorBonus = new Dictionary<int, int>();
    private int currentBackgroundPictureID = 1;
    private int currentArrow = 0;
    private bool isChoosingEnemy = false;
    private bool isPlayerChoosing = false;
    private bool isBattleOver = false;
    private bool isAnimating = false;
    private int currentTypeAttack;
    private int currentSkillID;
    private int currentItemID;

    private List<TurnType> turnOrder = new List<TurnType>();

    enum TurnType { Enemy, Enemy2, Enemy3, Player1, Player2, Player3, Player4, Player5 }

    private bool isWaitingForParryOrDodge = false;
    private dodgeType currentHitDodgeType;
    private bool lastHitWasDodged;
    private bool lastHitWasParried;
    private int totalHitsInAttack;
    private int dodgedHitsCount;
    [SerializeField] private List<EnemeyInfo> enemeyInfos = new List<EnemeyInfo>();
    [SerializeField] private List<GameObject> enemyPosition = new List<GameObject>();
    [SerializeField] private List<GameObject> playerPosition = new List<GameObject>();
    private List<Vector3> defaultPlayerPositions = new List<Vector3>();
    private List<Vector3> defaultEnemyPositions = new List<Vector3>();

    [SerializeField] private List<CharacterAnimationData> characterAnimations = new List<CharacterAnimationData>();
    [SerializeField] private List<EnemyAnimationData> enemyAnimations = new List<EnemyAnimationData>();

    [SerializeField] private List<Image> FaceHolders = new List<Image>();
    [SerializeField] private List<FacesSprite> Faces = new List<FacesSprite>();
    [SerializeField] private List<characterBars> characterBars = new List<characterBars>();
    [SerializeField] private List<BackgroundPicture> BackgroundPictures = new List<BackgroundPicture>();
    [SerializeField] private List<GameObject> arrowsCharacters = new List<GameObject>();
    [SerializeField] private List<GameObject> arrowsEnemies = new List<GameObject>();
    [SerializeField] private List<GameObject> enemyUIs = new List<GameObject>();
    [SerializeField] private List<GameObject> characterUIs = new List<GameObject>();
    [SerializeField] private List<GameObject> startUIEnemys = new List<GameObject>();
    [SerializeField] private List<TextMeshProUGUI> startUIEnemysText = new List<TextMeshProUGUI>();

    [SerializeField] private List<CameraInfo> camerasInfo = new List<CameraInfo>();
    private Camera currentActiveCamera;
    public float transitionDuration = 0.5f;
    public float zoomStartFOV = 80f;
    public float zoomEndFOV = 60f;
    private int defaultPPU;
    private Coroutine activeAnimation;
    private Dictionary<Camera, Vector3> originalPositions = new Dictionary<Camera, Vector3>();
    [SerializeField] private List<MusicEnemy> enemyMusics = new List<MusicEnemy>();
    [SerializeField] private AudioSource musicAudioSource;

    private float currentHitElapsed = 0f;
    private float currentHitParryWindow = 0f;

    [SerializeField] private TextMeshProUGUI StartGameText;
    [SerializeField] private TextMeshProUGUI StartHeaderText;
    [SerializeField] private TextMeshProUGUI ItemsText;
    [SerializeField] private TextMeshProUGUI SkillsText;
    [SerializeField] private TextMeshProUGUI AttacksText;
    [SerializeField] private TextMeshProUGUI VictoryText;
    [SerializeField] private TextMeshProUGUI GameOverText;
    [SerializeField] private TextMeshProUGUI ContinueText;
    [SerializeField] private TextMeshProUGUI RestartText;
    [SerializeField] private TextMeshProUGUI BackButtonText;

    public void LanguageOnStart()
    {
        StartGameText.text = gameDataManager.currentGameData.settings.currentLanguage == 0 ? "Start Game" : "Zapnout Hru";
        StartHeaderText.text = gameDataManager.currentGameData.settings.currentLanguage == 0 ? "Prepare for Battle!" : "Připravte se na boj!";
        ItemsText.text = gameDataManager.currentGameData.settings.currentLanguage == 0 ? "Items" : "Předměty";
        SkillsText.text = gameDataManager.currentGameData.settings.currentLanguage == 0 ? "Skills" : "Skily";        
        AttacksText.text = gameDataManager.currentGameData.settings.currentLanguage == 0 ? "Attacks" : "Útoky";
        VictoryText.text = gameDataManager.currentGameData.settings.currentLanguage == 0 ? "Congratulation You Win!" : "Gratuluji! Vyhrál jsi!";
        GameOverText.text = gameDataManager.currentGameData.settings.currentLanguage == 0 ? "Expedition Died" : "Expedice skončila";
        ContinueText.text = gameDataManager.currentGameData.settings.currentLanguage == 0 ? "Continue" : "Pokračovat";
        RestartText.text = gameDataManager.currentGameData.settings.currentLanguage == 0 ? "Restart" : "Restartovat";
        BackButtonText.text = gameDataManager.currentGameData.settings.currentLanguage == 0 ? "Back" : "Zpět";
    }

    void ApplyPerksToCharacters()
    {
        characterCritBonus.Clear();
        characterDamageBonus.Clear();
        characterArmorBonus.Clear();

        if (gameDataManager.currentGameData == null) return;

        Perks[] allPerks = Resources.LoadAll<Perks>("PerksData");

        foreach (Character ch in characters)
        {
            List<int> equippedIDs = new List<int>();
            if (ch.pickePerkID1 != 0) equippedIDs.Add(ch.pickePerkID1);
            if (ch.pickePerkID2 != 0) equippedIDs.Add(ch.pickePerkID2);
            if (ch.pickePerkID3 != 0) equippedIDs.Add(ch.pickePerkID3);

            foreach (int perkID in equippedIDs)
            {
                Perks perk = System.Array.Find(allPerks, p => p.id == perkID);
                if (perk == null) continue;
                ApplySinglePerk(ch, perk);
            }

            Debug.Log($"[Perks] {ch.name}: equipped perk IDs [{string.Join(", ", equippedIDs)}]");
        }
    }

    void ApplySinglePerk(Character ch, Perks perk)
    {
        switch (perk.perkType)
        {
            case perkType.healthAdder:
                ch.maxHealth += perk.addingAmount * perk.levelOfPerk;
                ch.health += perk.addingAmount * perk.levelOfPerk;
                break;

            case perkType.damageAdder:
                if (!characterDamageBonus.ContainsKey(ch.id)) characterDamageBonus[ch.id] = 0;
                characterDamageBonus[ch.id] += perk.addingAmount * perk.levelOfPerk;
                break;

            case perkType.critAdder:
                if (!characterCritBonus.ContainsKey(ch.id)) characterCritBonus[ch.id] = 0f;
                characterCritBonus[ch.id] += (perk.addingAmount * perk.levelOfPerk) / 100f;
                break;

            case perkType.speedAdder:
                ch.speed += perk.addingAmount * perk.levelOfPerk;
                break;

            case perkType.armorAdder:
                if (!characterArmorBonus.ContainsKey(ch.id)) characterArmorBonus[ch.id] = 0;
                characterArmorBonus[ch.id] += perk.addingAmount * perk.levelOfPerk;
                break;
        }
    }

    float GetCritChance(Character ch)
    {
        float bonus = characterCritBonus.ContainsKey(ch.id) ? characterCritBonus[ch.id] : 0f;
        return Mathf.Clamp01(baseCritChance + bonus);
    }

    bool RollCrit(Character ch)
    {
        return Random.value < GetCritChance(ch);
    }

    int ApplyCrit(int damage)
    {
        return Mathf.RoundToInt(damage * critDamageMultiplier);
    }

    IEnumerator ShowCritIndicator()
    {
        if (critIndicatorText != null)
        {
            critIndicatorText.text = gameDataManager.currentGameData.settings.currentLanguage == 0 ? "CRITICAL!" : "KRYTÍCÍ!";
            critIndicatorText.color = critTextColor;
            critIndicatorText.gameObject.SetActive(true);

            if (audioSource != null && critSoundEffect != null)
                audioSource.PlayOneShot(critSoundEffect);

            yield return new WaitForSeconds(critIndicatorDuration);
            critIndicatorText.gameObject.SetActive(false);
        }
    }

    int GetDamageBonus(Character ch)
    {
        return characterDamageBonus.ContainsKey(ch.id) ? characterDamageBonus[ch.id] : 0;
    }

    int ReduceByArmor(Character ch, int incomingDamage)
    {
        int armor = characterArmorBonus.ContainsKey(ch.id) ? characterArmorBonus[ch.id] : 0;
        return Mathf.Max(1, incomingDamage - armor);
    }
    public void StartTurnBasedGame()
    {
        if (starterUI != null) starterUI.SetActive(false);
        StartCoroutine(StartGameCoroutine());
    }

    IEnumerator StartGameCoroutine()
    {
        isAnimating = true;

        if (flashImage != null)
        {
            flashImage.gameObject.SetActive(true);
            flashImage.color = new Color(1f, 1f, 1f, 1f);
            float elapsed = 0f;
            while (elapsed < 0.5f)
            {
                elapsed += Time.deltaTime;
                flashImage.color = new Color(1f, 1f, 1f, 1f - (elapsed / 0.5f));
                yield return null;
            }
            flashImage.gameObject.SetActive(false);
            yield return new WaitForSeconds(3.5f);
        }
        else
        {
            yield return new WaitForSeconds(4f);
        }

        isAnimating = false;
        PlayCurrentTurn();
    }

    void Start()
    {
        characters = gameDataManager.currentGameData.characters;
        enemies = gameDataManager.currentGameData.enemies;
        itemDatabase = _databaseReference;
        skillDatabase = _skillDatabaseReference;
        perksDatabase = _perksDatabaseReference;

        Debug.Log("Characters loaded: " + string.Join(", ", characters.Select(c => c.name)));

        ApplyPerksToCharacters();

        if (gameDataManager.currentGameData.activeCombatEnemyIDs.Count > 0)
        {
            whatEnemiesIsFighting = new List<int>(gameDataManager.currentGameData.activeCombatEnemyIDs);
        }

        LanguageOnStart();

        inicializeTurnBasedGame();
        HideInfo();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) nextTurn();
        if (isBattleOver) return;

        if (isWaitingForParryOrDodge)
        {
            if (Input.GetKeyDown(keyParry))
            {
                if (currentHitDodgeType != dodgeType.jump)
                {
                    if (currentHitElapsed <= currentHitParryWindow)
                    {
                        lastHitWasParried = true;
                        lastHitWasDodged = false;
                        ShowParryDodgeText(gameDataManager.currentGameData.settings.currentLanguage == 0 ? "PARRY!" : "KRYT!");
                    }
                    else
                    {
                        ShowParryDodgeText(gameDataManager.currentGameData.settings.currentLanguage == 0 ? "TOO LATE!" : "POZDĚ!");
                    }
                    isWaitingForParryOrDodge = false;
                }
            }
            else if (currentHitDodgeType == dodgeType.normal && Input.GetKeyDown(keyDodge))
            {
                lastHitWasDodged = true;
                lastHitWasParried = false;
                isWaitingForParryOrDodge = false;
                ShowParryDodgeText(gameDataManager.currentGameData.settings.currentLanguage == 0 ? "DODGE!" : "UHNUTÍ!");
            }
            else if (currentHitDodgeType == dodgeType.jump && Input.GetKeyDown(keyJump))
            {
                lastHitWasDodged = true;
                lastHitWasParried = false;
                isWaitingForParryOrDodge = false;
                ShowParryDodgeText(gameDataManager.currentGameData.settings.currentLanguage == 0 ? "JUMP DODGE!" : "SKOK!");
            }
            return;
        }

        if (isAnimating) return;

        if (isPlayerChoosing && isChoosingEnemy)
        {
            if (Input.GetKeyDown(keyDown)) CycleEnemyArrow(1);
            if (Input.GetKeyDown(keyUp)) CycleEnemyArrow(-1);
            if (Input.GetKeyDown(keyAccept)) StartCoroutine(HandlePlayerAttackCoroutine());
            if (Input.GetKeyDown(keyBack)) handleSelectionBack();
        }
        else if (isPlayerChoosing && !isChoosingEnemy)
        {
            if (Input.GetKeyDown(keyDown)) CycleCharacterArrow(1);
            if (Input.GetKeyDown(keyUp)) CycleCharacterArrow(-1);
            if (Input.GetKeyDown(keyAccept)) StartCoroutine(HandlePlayerSelfTargetCoroutine());
            if (Input.GetKeyDown(keyBack)) handleSelectionBack();
        }
    }

    void CycleEnemyArrow(int dir)
    {
        List<int> alive = currentEnemy
            .Select((e, i) => new { e, i })
            .Where(x => !x.e.isDead)
            .Select(x => x.i)
            .ToList();

        if (alive.Count == 0) return;

        int pos = alive.IndexOf(currentArrow);
        if (pos < 0) pos = 0;
        pos = (pos + dir + alive.Count) % alive.Count;
        currentArrow = alive[pos];
        ShowArrowEnemySafe(currentArrow);
    }

    void CycleCharacterArrow(int dir)
    {
        List<int> alive = characters
            .Select((c, i) => new { c, i })
            .Where(x => !x.c.isDead)
            .Select(x => x.i)
            .ToList();

        if (alive.Count == 0) return;

        int pos = alive.IndexOf(currentArrow);
        if (pos < 0) pos = 0;
        pos = (pos + dir + alive.Count) % alive.Count;
        currentArrow = alive[pos];
        ShowArrow(arrowsCharacters, currentArrow);
    }

    void inicializeTurnBasedGame()
    {
        onTurnbasedStart();
        getDefaultPositions();
        createTurnOrder();
        updateEnemyHealthBar();
        updateCharacterBars();
        RefreshAllUIs();

        foreach (var info in camerasInfo)
            if (info.targetCamera != null)
                originalPositions[info.targetCamera] = info.targetCamera.transform.position;

        var ppCam = camerasInfo.FirstOrDefault(c => c.IDofCamera == 0)
                               .targetCamera
                               .GetComponent<PixelPerfectCamera>();
        defaultPPU = ppCam.assetsPPU;

        SetActiveCamera(0);
        EnemyName.text = currentEnemy.Count > 0 ? currentEnemy[0].name : "";
        BackgroundPicture.sprite = BackgroundPictures
            .FirstOrDefault(b => b.ID == currentBackgroundPictureID).sprite;

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (parryDodgePromptText != null) parryDodgePromptText.gameObject.SetActive(false);
        if (critIndicatorText != null) critIndicatorText.gameObject.SetActive(false);

        for (int i = 0; i < startUIEnemys.Count; i++)
        {
            var enemyData = enemeyInfos.FirstOrDefault(e => e.ID == whatEnemiesIsFighting[i]);
            startUIEnemys[i].GetComponent<UnityEngine.UI.Image>().sprite = enemyData.TurnBasedIcon;
        }

        for (int i = 0; i < startUIEnemysText.Count; i++)
        {
            var enemyData = enemeyInfos.FirstOrDefault(e => e.ID == whatEnemiesIsFighting[i]);
            startUIEnemysText[i].text = enemyData.name;
        }
        for (int i = 0; i < currentEnemy.Count; i++)
        {
            if (i >= enemyUIs.Count || enemyUIs[i] == null) continue;
            var enemyData = enemeyInfos.FirstOrDefault(e => e.ID == currentEnemy[i].id);
            if (enemyData.sprite == null) continue;
            var sr = enemyUIs[i].GetComponent<SpriteRenderer>();
            if (sr != null) sr.sprite = enemyData.sprite;
        }

        if (musicAudioSource != null && enemyMusics.Count > 0 && currentEnemy.Count > 0)
        {
            int middleIndex = currentEnemy.Count / 2;
            int middleEnemyID = currentEnemy[middleIndex].id;
            MusicEnemy matchedMusic = enemyMusics.FirstOrDefault(m => m.ID == middleEnemyID);

            if (matchedMusic.music != null)
            {
                float volume = 1f;
                volume = gameDataManager.currentGameData.settings.FinalMusicVolume;
                musicAudioSource.clip = matchedMusic.music;
                musicAudioSource.volume = volume;
                musicAudioSource.loop = true;
                musicAudioSource.Play();
            }
        }
    }

    void onTurnbasedStart()
    {
        currentEnemy = whatEnemiesIsFighting
            .Select(id =>
            {
                Enemy original = enemies.FirstOrDefault(en => en.id == id);
                if (original == null) return null;
                return new Enemy
                {
                    id = original.id,
                    name = original.name,
                    health = original.health,
                    maxHealth = original.maxHealth,
                    isDead = original.isDead,
                    sprite = original.sprite,
                    attacks = original.attacks.Select(a => new EnemyAttack
                    {
                        id = a.id,
                        attackName = a.attackName,
                        totalAnimationDuration = a.totalAnimationDuration,
                        weight = a.weight,
                        numberOfCharHits = a.numberOfCharHits,
                        hits = a.hits.Select(h => new Hit
                        {
                            timeOffset = h.timeOffset,
                            damage = h.damage,
                            parryTimePlayer = h.parryTimePlayer,
                            dodgeTimePlayer = h.dodgeTimePlayer,
                            dodgeType = h.dodgeType
                        }).ToList(),
                        animations = a.animations
                    }).ToList()
                };
            })
            .Where(e => e != null)
            .ToList();

        Debug.Log("Current enemy: " + string.Join(", ", currentEnemy.Select(e => e.name)));
        foreach (var e in currentEnemy)
        {
            Debug.Log($"Enemy: {e.name}, attacks: {e.attacks.Count}");
            foreach (var a in e.attacks)
                Debug.Log($"  Attack: {a.attackName}, hits: {a.hits.Count}, numberOfCharHits: {a.numberOfCharHits}");
        }
        
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
            .Select(c =>
            {
                if (c.id == 1) return TurnType.Player1;
                if (c.id == 2) return TurnType.Player2;
                if (c.id == 3) return TurnType.Player3;
                if (c.id == 4) return TurnType.Player4;
                return TurnType.Player5;
            }).ToList();

        if (enemyPool.Count == 0 && playerPool.Count == 0) return;

        int enemyIndex = 0;
        int playerIndex = 0;

        while (turnOrder.Count < 500)
        {
            if (enemyPool.Count > 0)
            {
                int enemyTurnsPerRound = 2;
                for (int e = 0; e < enemyTurnsPerRound; e++)
                {
                    if (turnOrder.Count >= 500) break;
                    turnOrder.Add(enemyPool[enemyIndex]);
                    enemyIndex = (enemyIndex + 1) % enemyPool.Count;
                }
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

    bool IsPlayerTurn(TurnType t) =>
        t == TurnType.Player1 || t == TurnType.Player2 || t == TurnType.Player3 ||
        t == TurnType.Player4 || t == TurnType.Player5;

    Character GetCharacterForTurn(TurnType t) =>
        characters.FirstOrDefault(c =>
            (t == TurnType.Player1 && c.id == 1) ||
            (t == TurnType.Player2 && c.id == 2) ||
            (t == TurnType.Player3 && c.id == 3) ||
            (t == TurnType.Player4 && c.id == 4) ||
            (t == TurnType.Player5 && c.id == 5));

    int GetEnemyIndex(TurnType t)
    {
        if (t == TurnType.Enemy) return 0;
        if (t == TurnType.Enemy2) return 1;
        if (t == TurnType.Enemy3) return 2;
        return -1;
    }

    int GetFirstAliveEnemyIndex()
    {
        for (int i = 0; i < currentEnemy.Count; i++)
            if (!currentEnemy[i].isDead) return i;
        return 0;
    }

    public void nextTurn()
    {
        if (isBattleOver || isAnimating) return;
        if (turnOrder.Count == 0) return;

        if (chooseMenu.activeSelf) chooseMenu.SetActive(false);
        isPlayerChoosing = false;
        isChoosingEnemy = false;
        HideArrow();
        handleSkillItemClosing();

        turnOrder.RemoveAt(0);
        if (turnOrder.Count == 0) createTurnOrder();

        PlayCurrentTurn();
    }

    void PlayCurrentTurn()
    {
        if (turnOrder.Count == 0) return;

        UpdateFaces();
        TurnType next = turnOrder[0];

        if (IsPlayerTurn(next))
        {
            currentCharacter = GetCharacterForTurn(next);
            if (currentCharacter == null || currentCharacter.isDead) { nextTurn(); return; }

            Debug.Log("Hráčův tah: " + currentCharacter.name);
            SwitchToPlayerCamera(currentCharacter.id);
            StartCoroutine(ShowChooseMenuDelayed(0.35f));
        }
        else
        {
            int enemyIndex = GetEnemyIndex(next);
            if (enemyIndex < 0 || enemyIndex >= currentEnemy.Count || currentEnemy[enemyIndex].isDead)
            {
                nextTurn();
                return;
            }
            StartCoroutine(EnemyTurnCoroutine(enemyIndex));
        }
    }

    IEnumerator ShowChooseMenuDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        chooseMenu.SetActive(true);
    }

    IEnumerator EnemyTurnCoroutine(int enemyIndex)
    {
        isAnimating = true;

    Enemy enemy = currentEnemy[enemyIndex];
    Debug.Log("Nepřítelův tah: " + enemy.name);

    EnemyAttack chosenAttack = ChooseEnemyAttack(enemy);
    if (chosenAttack == null) { isAnimating = false; nextTurn(); yield break; }

    List<Character> alivePlayers = characters.Where(c => !c.isDead).ToList();
    if (alivePlayers.Count == 0) { isAnimating = false; TriggerGameOver(); yield break; }

    int hitCount = Mathf.Min(chosenAttack.numberOfCharHits, alivePlayers.Count);
    List<Character> targets = new List<Character>();
    List<int> playerIndices = new List<int>();

    List<Character> pool = new List<Character>(alivePlayers);
    for (int t = 0; t < hitCount; t++)
    {
        int pick = Random.Range(0, pool.Count);
        targets.Add(pool[pick]);
        playerIndices.Add(characters.IndexOf(pool[pick]));
        pool.RemoveAt(pick);
    }

    if (playerIndices.Count == 0) { isAnimating = false; nextTurn(); yield break; }
    int primaryPlayerIndex = playerIndices[0];

    if (enemyIndex >= enemyPosition.Count || primaryPlayerIndex >= playerPosition.Count)
    {
        Debug.LogError($"Position out of range: enemyIndex={enemyIndex}, primaryPlayerIndex={primaryPlayerIndex}");
        isAnimating = false; nextTurn(); yield break;
    }

        
        if (chosenAttack == null) { isAnimating = false; nextTurn(); yield break; }

        Debug.Log($"{enemy.name} útočí '{chosenAttack.attackName}' na {string.Join(", ", targets.Select(t => t.name))}");

        if (nextAttackText != null)
        {
            nextAttackText.text = gameDataManager.currentGameData.settings.currentLanguage == 0 ? "Enemy is Attacking: " + chosenAttack.attackName : "Nepřítel útočí: " + chosenAttack.attackName;
            nextAttackText.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(2f);
        if (nextAttackText != null) nextAttackText.gameObject.SetActive(false);

        yield return StartCoroutine(SwitchToCameraAndWait(8));
        yield return StartCoroutine(MoveObject(
            enemyPosition[enemyIndex],
            defaultEnemyPositions[enemyIndex],
            getAnimationPositions(enemyIndex, primaryPlayerIndex, false),
            0.25f));

        List<Hit> sortedHits = chosenAttack.hits.OrderBy(h => h.timeOffset).ToList();
        totalHitsInAttack = sortedHits.Count;
        dodgedHitsCount = 0;
        float lastOffset = 0f;

        foreach (Hit hit in sortedHits)
        {
            float waitTime = hit.timeOffset - lastOffset;
            if (waitTime > 0f) yield return new WaitForSeconds(waitTime);
            lastOffset = hit.timeOffset;

            lastHitWasDodged = false;
            lastHitWasParried = false;
            currentHitDodgeType = hit.dodgeType;
            isWaitingForParryOrDodge = true;

            string prompt = hit.dodgeType == dodgeType.jump
                ? (gameDataManager.currentGameData.settings.currentLanguage == 0 ? $"[{keyJump}] JUMP" : $"[{keyJump}] SKOK")
                : (gameDataManager.currentGameData.settings.currentLanguage == 0 ? $"[{keyDodge}] DODGE  |  [{keyParry}] PARRY" : $"[{keyDodge}] UHNI  |  [{keyParry}] KRYJ");
            ShowParryDodgeText(prompt);

            float parryWindow = hit.parryTimePlayer;
            float dodgeWindow = hit.dodgeTimePlayer;
            currentHitElapsed = 0f;
            currentHitParryWindow = parryWindow;

            while (isWaitingForParryOrDodge && currentHitElapsed < dodgeWindow)
            {
                currentHitElapsed += Time.deltaTime;
                yield return null;
            }

            isWaitingForParryOrDodge = false;
            HideParryDodgeText();
    
            if (lastHitWasParried)
            {
                if (playerAnimator != null) playerAnimator.SetTrigger(parryAnimationTrigger);
                int reflectDmg = Mathf.Max(1, hit.damage / 2);
                enemy.health -= reflectDmg;
                if (enemy.health < 0) enemy.health = 0;
                updateEnemyHealthBar();
                Debug.Log($"PARRY! vrátil {reflectDmg} damage na {enemy.name}");
                ShowParryDodgeText(gameDataManager.currentGameData.settings.currentLanguage == 0 ? $"PARRY! +{reflectDmg}" : $"VYKRYTO! +{reflectDmg}");
                yield return new WaitForSeconds(0.5f);
                HideParryDodgeText();
            }
            else if (lastHitWasDodged)
            {
                if (playerAnimator != null) playerAnimator.SetTrigger(dodgeAnimationTrigger);
                dodgedHitsCount++;

                if (hit.dodgeType == dodgeType.jump)
                {
                    int reflectDmg = Mathf.Max(1, hit.damage / 2);
                    enemy.health -= reflectDmg;
                    if (enemy.health < 0) enemy.health = 0;
                    updateEnemyHealthBar();
                    Debug.Log($"JUMP DODGE! vrátil {reflectDmg} damage na {enemy.name}");
                    ShowParryDodgeText(gameDataManager.currentGameData.settings.currentLanguage == 0 ? $"JUMP! +{reflectDmg}" : $"SKOK! +{reflectDmg}");
                    yield return new WaitForSeconds(0.5f);
                    HideParryDodgeText();
                }
                else
                {
                    ShowParryDodgeText(gameDataManager.currentGameData.settings.currentLanguage == 0 ? "PERFECT DODGE!" : "PERFEKTNÍ ÚHYB!");
                    yield return new WaitForSeconds(0.5f);
                    HideParryDodgeText();
                }
            }
            else
            {
                foreach (Character target in targets)
                {
                    int finalDmg = ReduceByArmor(target, hit.damage);
                    target.health -= finalDmg;
                    if (target.health < 0) target.health = 0;
                    Debug.Log($"HIT! {target.name} -{finalDmg} HP → {target.health}/{target.maxHealth}");
                }
                updateCharacterBars();
            }

            yield return new WaitForSeconds(0.1f);
        }

        if (enemy.health <= 0)
        {
            enemy.isDead = true;
            UpdateTurnOrder(enemy.id, true);
            HandleEnemyDeath(enemyIndex);
            Debug.Log(enemy.name + " zemřel od counter-attacku!");
        }

        foreach (Character target in targets)
        {
            if (target.health <= 0)
            {
                int playerIndex = characters.IndexOf(target);
                UpdateTurnOrder(target.id, false);
                HandlePlayerDeath(playerIndex);
                Debug.Log(target.name + " zemřel!");
            }
        }

        yield return new WaitForSeconds(0.2f);
        yield return StartCoroutine(MoveObject(
            enemyPosition[enemyIndex],
            enemyPosition[enemyIndex].transform.position,
            defaultEnemyPositions[enemyIndex],
            0.2f));

        isAnimating = false;
        CheckBattleEnd();
        if (!isBattleOver) nextTurn();
    }

    EnemyAttack ChooseEnemyAttack(Enemy enemy)
    {
        if (enemy.attacks == null || enemy.attacks.Count == 0) return null;
        float totalWeight = enemy.attacks.Sum(a => a.weight);
        if (totalWeight <= 0f) return enemy.attacks[0];

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;
        foreach (EnemyAttack attack in enemy.attacks)
        {
            cumulative += attack.weight;
            if (roll < cumulative) return attack;
        }
        return enemy.attacks[0];
    }

    void HandleEnemyDeath(int enemyIndex)
    {
        if (enemyIndex < enemyUIs.Count && enemyUIs[enemyIndex] != null) enemyUIs[enemyIndex].SetActive(false);
        if (enemyIndex < arrowsEnemies.Count && arrowsEnemies[enemyIndex] != null) arrowsEnemies[enemyIndex].SetActive(false);
        if (enemyIndex < enemyPosition.Count && enemyPosition[enemyIndex] != null) enemyPosition[enemyIndex].SetActive(false);

        if (isChoosingEnemy && currentArrow == enemyIndex)
        {
            List<int> alive = currentEnemy
                .Select((e, i) => new { e, i })
                .Where(x => !x.e.isDead)
                .Select(x => x.i)
                .ToList();

            if (alive.Count > 0) { currentArrow = alive[0]; ShowArrowEnemySafe(currentArrow); }
            else HideArrow();
        }
    }

    void HandlePlayerDeath(int playerIndex)
    {
        if (playerIndex < characterUIs.Count && characterUIs[playerIndex] != null) characterUIs[playerIndex].SetActive(false);
        if (playerIndex < arrowsCharacters.Count && arrowsCharacters[playerIndex] != null) arrowsCharacters[playerIndex].SetActive(false);
        if (playerIndex < playerPosition.Count && playerPosition[playerIndex] != null) playerPosition[playerIndex].SetActive(false);
    }

    void ShowArrowEnemySafe(int index)
    {
        for (int i = 0; i < arrowsEnemies.Count; i++)
        {
            bool show = i == index && i < currentEnemy.Count && !currentEnemy[i].isDead;
            arrowsEnemies[i].SetActive(show);
        }
    }

    void RefreshAllUIs()
    {
        for (int i = 0; i < currentEnemy.Count; i++)
        {
            bool alive = !currentEnemy[i].isDead;
            if (i < enemyUIs.Count && enemyUIs[i] != null) enemyUIs[i].SetActive(alive);
            if (i < enemyPosition.Count && enemyPosition[i] != null) enemyPosition[i].SetActive(alive);
        }
        for (int i = 0; i < characters.Count; i++)
        {
            bool alive = !characters[i].isDead;
            if (i < characterUIs.Count && characterUIs[i] != null) characterUIs[i].SetActive(alive);
            if (i < playerPosition.Count && playerPosition[i] != null) playerPosition[i].SetActive(alive);
        }
    }

    public void basicAttack()
    {
        if (isAnimating) return;
        currentTypeAttack = 0;

        List<int> aliveEnemies = currentEnemy
            .Select((e, i) => new { e, i })
            .Where(x => !x.e.isDead)
            .Select(x => x.i)
            .ToList();

        if (aliveEnemies.Count == 1)
        {
            currentArrow = aliveEnemies[0];
            handleSkillItemClosing();
            if (chooseMenu.activeSelf) chooseMenu.SetActive(false);
            StartCoroutine(HandlePlayerAttackCoroutine());
            return;
        }

        isPlayerChoosing = true;
        isChoosingEnemy = true;
        currentArrow = GetFirstAliveEnemyIndex();
        handleSkillItemClosing();
        if (chooseMenu.activeSelf) chooseMenu.SetActive(false);
        SetActiveCamera(8);
        ShowArrowEnemySafe(currentArrow);
    }

    IEnumerator HandlePlayerAttackCoroutine()
    {
        if (isAnimating) yield break;
        isAnimating = true;

        HideArrow();
        isPlayerChoosing = false;
        isChoosingEnemy = false;

        int targetEnemyIndex = currentArrow;

        switch (currentTypeAttack)
        {
            case 0:
            {
                Enemy targetEnemy = currentEnemy[targetEnemyIndex];
                int playerIndex = characters.IndexOf(currentCharacter);

                yield return StartCoroutine(SwitchToCameraAndWait(8));
                yield return StartCoroutine(MoveObject(
                    playerPosition[playerIndex],
                    defaultPlayerPositions[playerIndex],
                    getAnimationPositions(targetEnemyIndex, playerIndex, true),
                    0.25f));

                yield return new WaitForSeconds(0.1f);

                int damage = currentCharacter.attack;
                if (itemDatabase != null)
                {
                    Item weapon = itemDatabase.GetItemByID(currentCharacter.pickedItemID);
                    if (weapon != null) damage = (int)weapon.Damage;
                }

                damage += GetDamageBonus(currentCharacter);

                bool isCrit = RollCrit(currentCharacter);
                if (isCrit)
                {
                    damage = ApplyCrit(damage);
                    yield return StartCoroutine(ShowCritIndicator());
                }

                targetEnemy.health -= damage;
                if (targetEnemy.health < 0) targetEnemy.health = 0;

                int charIndex = characters.IndexOf(currentCharacter);
                characters[charIndex].mana = (int)Mathf.Min(characters[charIndex].mana + 5, 10);

                updateEnemyHealthBar();
                updateCharacterBars();
                Debug.Log($"{currentCharacter.name} udeřil {targetEnemy.name} za {damage}{(isCrit ? " (CRIT!)" : "")}. HP: {targetEnemy.health}");

                yield return new WaitForSeconds(0.15f);
                yield return StartCoroutine(MoveObject(
                    playerPosition[playerIndex],
                    playerPosition[playerIndex].transform.position,
                    defaultPlayerPositions[playerIndex],
                    0.2f));

                if (targetEnemy.health <= 0)
                {
                    targetEnemy.isDead = true;
                    UpdateTurnOrder(targetEnemy.id, true);
                    HandleEnemyDeath(targetEnemyIndex);
                    Debug.Log(targetEnemy.name + " poražen!");
                }
                break;
            }

            case 1:
            {
                Skills currentSkill = skillDatabase.GetSkillByID(currentSkillID);
                Enemy targetEnemy = currentEnemy[targetEnemyIndex];
                int charIndex = characters.IndexOf(currentCharacter);
                int playerIndex = charIndex;

                characters[charIndex].mana -= currentSkill.manaCost;
                if (characters[charIndex].mana < 0) characters[charIndex].mana = 0;
                updateCharacterBars();

                yield return StartCoroutine(SwitchToCameraAndWait(8));
                yield return StartCoroutine(MoveObject(
                    playerPosition[playerIndex],
                    defaultPlayerPositions[playerIndex],
                    getAnimationPositions(targetEnemyIndex, playerIndex, true),
                    0.25f));

                yield return new WaitForSeconds(0.1f);

                int skillDamage = currentSkill.amount + GetDamageBonus(currentCharacter);

                bool isCrit = RollCrit(currentCharacter);
                if (isCrit)
                {
                    skillDamage = ApplyCrit(skillDamage);
                    yield return StartCoroutine(ShowCritIndicator());
                }

                targetEnemy.health -= skillDamage;
                if (targetEnemy.health < 0) targetEnemy.health = 0;
                updateEnemyHealthBar();
                Debug.Log($"{currentCharacter.name} použil {currentSkill.name} na {targetEnemy.name} za {skillDamage}{(isCrit ? " (CRIT!)" : "")}");

                yield return new WaitForSeconds(0.15f);
                yield return StartCoroutine(MoveObject(
                    playerPosition[playerIndex],
                    playerPosition[playerIndex].transform.position,
                    defaultPlayerPositions[playerIndex],
                    0.2f));

                if (targetEnemy.health <= 0)
                {
                    targetEnemy.isDead = true;
                    UpdateTurnOrder(targetEnemy.id, true);
                    HandleEnemyDeath(targetEnemyIndex);
                    Debug.Log(targetEnemy.name + " poražen!");
                }
                break;
            }

            case 2:
            {
                Item currentItem = itemDatabase.GetItemByID(currentItemID);
                Enemy targetEnemy = currentEnemy[targetEnemyIndex];

                SwitchToPlayerCamera(currentCharacter.id);
                yield return new WaitForSeconds(0.3f);

                if (currentItem != null)
                {
                    if (currentItem.turnBaseItemType == TurnBaseItemType.Debuff ||
                        currentItem.turnBaseItemType == TurnBaseItemType.Weakening)
                    {
                        int itemDamage = currentItem.turnBaseItemEffectAmount + GetDamageBonus(currentCharacter);

                        bool isCrit = RollCrit(currentCharacter);
                        if (isCrit)
                        {
                            itemDamage = ApplyCrit(itemDamage);
                            yield return StartCoroutine(ShowCritIndicator());
                        }

                        targetEnemy.health -= itemDamage;
                        if (targetEnemy.health < 0) targetEnemy.health = 0;
                        Debug.Log($"{currentCharacter.name} použil {currentItem.itemName} na {targetEnemy.name} za {itemDamage}{(isCrit ? " (CRIT!)" : "")}");
                    }
                }

                updateEnemyHealthBar();
                yield return new WaitForSeconds(0.2f);

                if (targetEnemy.health <= 0)
                {
                    targetEnemy.isDead = true;
                    UpdateTurnOrder(targetEnemy.id, true);
                    HandleEnemyDeath(targetEnemyIndex);
                    Debug.Log(targetEnemy.name + " poražen!");
                }
                break;
            }
        }

        isAnimating = false;
        CheckBattleEnd();
        if (!isBattleOver) nextTurn();
    }

    IEnumerator HandlePlayerSelfTargetCoroutine()
    {
        if (isAnimating) yield break;
        isAnimating = true;

        HideArrow();
        isPlayerChoosing = false;
        isChoosingEnemy = false;

        int targetIndex = currentArrow;
        Character targetCharacter = characters[targetIndex];
        int charIndex = characters.IndexOf(currentCharacter);

        SwitchToPlayerCamera(currentCharacter.id);
        yield return new WaitForSeconds(0.3f);

        if (currentTypeAttack == 1)
        {
            Skills currentSkill = skillDatabase.GetSkillByID(currentSkillID);
            characters[charIndex].mana -= currentSkill.manaCost;
            if (characters[charIndex].mana < 0) characters[charIndex].mana = 0;

            switch (currentSkill.type)
            {
                case skillType.Heal:
                    targetCharacter.health = (int)Mathf.Min(targetCharacter.health + currentSkill.amount, targetCharacter.maxHealth);
                    Debug.Log($"{currentCharacter.name} vyléčil {targetCharacter.name} o {currentSkill.amount} HP");
                    break;
                case skillType.Mana:
                    targetCharacter.mana = (int)Mathf.Min(targetCharacter.mana + currentSkill.amount, 10);
                    Debug.Log($"{currentCharacter.name} obnovil {targetCharacter.name} {currentSkill.amount} many");
                    break;
                case skillType.Buff:
                    Debug.Log($"{currentCharacter.name} použil buff na {targetCharacter.name}: {currentSkill.name}");
                    break;
            }
        }
        else if (currentTypeAttack == 2)
        {
            Item currentItem = itemDatabase.GetItemByID(currentItemID);
            if (currentItem != null)
            {
                switch (currentItem.turnBaseItemType)
                {
                    case TurnBaseItemType.Healing:
                        targetCharacter.health = (int)Mathf.Min(targetCharacter.health + currentItem.turnBaseItemEffectAmount, targetCharacter.maxHealth);
                        Debug.Log($"{currentCharacter.name} použil {currentItem.itemName} a vyléčil {targetCharacter.name} o {currentItem.turnBaseItemEffectAmount} HP");
                        break;
                    case TurnBaseItemType.Mana:
                        targetCharacter.mana = (int)Mathf.Min(targetCharacter.mana + currentItem.turnBaseItemEffectAmount, 10);
                        Debug.Log($"{currentCharacter.name} použil {currentItem.itemName} a obnovil {targetCharacter.name} {currentItem.turnBaseItemEffectAmount} manu");
                        break;
                    case TurnBaseItemType.Buff:
                        Debug.Log($"{currentCharacter.name} použil {currentItem.itemName} jako buff na {targetCharacter.name}");
                        break;
                }
            }
        }

        updateCharacterBars();
        yield return new WaitForSeconds(0.2f);

        isAnimating = false;
        CheckBattleEnd();
        if (!isBattleOver) nextTurn();
    }

    void handleSelection(bool isChoosingEnemyInput, int currentTypeAttackInput)
    {
        if (isAnimating) return;

        if (currentTypeAttackInput == 1)
        {
            Skills currentSkill = skillDatabase.GetSkillByID(currentSkillID);
            if (currentSkill.manaCost > currentCharacter.mana)
            {
                Debug.Log("Nedostatek many!");
                return;
            }
        }

        HideInfo();

        if (isChoosingEnemyInput)
        {
            List<int> alive = currentEnemy.Select((e, i) => new { e, i }).Where(x => !x.e.isDead).Select(x => x.i).ToList();
            if (alive.Count == 1)
            {
                currentTypeAttack = currentTypeAttackInput;
                isChoosingEnemy = true;
                currentArrow = alive[0];
                handleSkillItemClosing();
                if (chooseMenu.activeSelf) chooseMenu.SetActive(false);
                StartCoroutine(HandlePlayerAttackCoroutine());
                return;
            }
        }

        isPlayerChoosing = true;
        currentTypeAttack = currentTypeAttackInput;
        handleSkillItemClosing();
        if (chooseMenu.activeSelf) chooseMenu.SetActive(false);
        SetActiveCamera(8);

        if (isChoosingEnemyInput)
        {
            isChoosingEnemy = true;
            currentArrow = GetFirstAliveEnemyIndex();
            ShowArrowEnemySafe(currentArrow);
        }
        else
        {
            isChoosingEnemy = false;
            currentArrow = Mathf.Max(0, characters.FindIndex(c => !c.isDead));
            ShowArrow(arrowsCharacters, currentArrow);
        }
    }

    void handleSelectionBack()
    {
        isPlayerChoosing = false;
        isChoosingEnemy = false;
        HideArrow();
        HideInfo();
        SwitchToPlayerCamera(currentCharacter.id);
        if (!chooseMenu.activeSelf) chooseMenu.SetActive(true);
        handleSkillOpening();
    }

    public void handleSkillOpening()
    {
        handleSkillItemClosing();
        chooserThingsMenu.SetActive(true);
        infoUI.SetActive(true);

        if (currentCharacter == null || skillDatabase == null) return;

        currentCharacterSkills = skillDatabase.GetAllSkills()
            .Where(s => s.characterID == currentCharacter.id)
            .ToList();

        foreach (var skill in currentCharacterSkills)
        {
            Skills capturedSkill = skill;
            GameObject skillButton = Instantiate(button, chooserThingsMenu.transform);
            skillButton.GetComponentInChildren<TextMeshProUGUI>().text =
            gameDataManager.currentGameData.settings.currentLanguage == 0
                ? $"{capturedSkill.name} ({capturedSkill.manaCost} MP)"
                : $"{capturedSkill.name} ({capturedSkill.manaCost} MP)"; 

            Button btn = skillButton.GetComponent<Button>();
            btn.interactable = capturedSkill.manaCost <= currentCharacter.mana;
            btn.onClick.AddListener(() =>
            {
                currentSkillID = capturedSkill.id;
                handleSelection(capturedSkill.type == skillType.Damage, 1);
            });

            EventTrigger trigger = skillButton.AddComponent<EventTrigger>();
            EventTrigger.Entry enter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            enter.callback.AddListener((d) => { ShowInfo(capturedSkill.name, 
                gameDataManager.currentGameData.settings.currentLanguage == 0 
                    ? "Type: " + capturedSkill.type.ToString() + "\nMana: " + capturedSkill.manaCost
                    : "Typ: " + capturedSkill.type.ToString() + "\nMana: " + capturedSkill.manaCost); });
            trigger.triggers.Add(enter);
            EventTrigger.Entry exit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            exit.callback.AddListener((d) => { HideInfo(); });
            trigger.triggers.Add(exit);
        }
    }

    public void handleItemOpening()
    {
        handleSkillItemClosing();
        chooserThingsMenu.SetActive(true);
        infoUI.SetActive(true);

        if (itemDatabase == null) return;

        List<Item> usableItems = itemDatabase.GetAllItems().Where(i => i.isTurnedBaseItem).ToList();

        foreach (var item in usableItems)
        {
            Item capturedItem = item;
            GameObject itemButton = Instantiate(button, chooserThingsMenu.transform);
            itemButton.GetComponentInChildren<TextMeshProUGUI>().text = capturedItem.itemName;

            Button btn = itemButton.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                currentItemID = capturedItem.id;
                bool targetsEnemy = (capturedItem.turnBaseItemType == TurnBaseItemType.Debuff ||
                                     capturedItem.turnBaseItemType == TurnBaseItemType.Weakening);
                handleSelection(targetsEnemy, 2);
            });

            EventTrigger trigger = itemButton.AddComponent<EventTrigger>();
            EventTrigger.Entry enter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            enter.callback.AddListener((d) => { ShowInfo(capturedItem.itemName, capturedItem.description); });
            trigger.triggers.Add(enter);
            EventTrigger.Entry exit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            exit.callback.AddListener((d) => { HideInfo(); });
            trigger.triggers.Add(exit);
        }
    }

    public void handleSkillItemClosing()
    {
        foreach (Transform child in chooserThingsMenu.transform)
            Destroy(child.gameObject);
        chooserThingsMenu.SetActive(false);
        infoUI.SetActive(false);
        HideInfo();
    }

    void ShowInfo(string nameStr, string descStr)
    {
        if (infoNameText != null) infoNameText.text = nameStr;
        if (infoDescriptionText != null) infoDescriptionText.text = descStr;
    }

    void HideInfo()
    {
        if (infoNameText != null) infoNameText.text = "";
        if (infoDescriptionText != null) infoDescriptionText.text = "";
    }

    void ShowParryDodgeText(string msg)
    {
        if (parryDodgePromptText == null) return;
        parryDodgePromptText.text = msg;
        parryDodgePromptText.gameObject.SetActive(true);
    }

    void HideParryDodgeText()
    {
        if (parryDodgePromptText == null) return;
        parryDodgePromptText.gameObject.SetActive(false);
    }

    void getDefaultPositions()
    {
        defaultEnemyPositions = enemyPosition.Select(p => p.transform.position).ToList();
        defaultPlayerPositions = playerPosition.Select(p => p.transform.position).ToList();
    }

    Vector3 getAnimationPositions(int enemyPositionIndex, int playerPositionIndex, bool isPlayerAttacking)
    {
        Vector3 enemyPos = defaultEnemyPositions[enemyPositionIndex];
        Vector3 playerPos = defaultPlayerPositions[playerPositionIndex];
        return isPlayerAttacking
            ? Vector3.Lerp(playerPos, enemyPos, 0.65f)
            : Vector3.Lerp(enemyPos, playerPos, 0.65f);
    }

    IEnumerator SwitchToCameraAndWait(int cameraID)
    {
        SetActiveCamera(cameraID);
        yield return new WaitForSeconds(transitionDuration);
    }

    IEnumerator MoveObject(GameObject obj, Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            obj.transform.position = Vector3.Lerp(from, to, Mathf.SmoothStep(0f, 1f, elapsed / duration));
            yield return null;
        }
        obj.transform.position = to;
    }

    void CheckBattleEnd()
    {
        if (currentEnemy.All(e => e.isDead)) { TriggerVictory(); return; }
        if (characters.All(c => c.isDead)) { TriggerGameOver(); }
    }

    void TriggerVictory()
    {
        if (isBattleOver) return;
        isBattleOver = true;
        StopAllCoroutines();
    
        Debug.Log("=== VÍTĚZSTVÍ! ===");
        if (victoryPanel != null) victoryPanel.SetActive(true);

        // Tvůj kód pro uložení stavu NPC...
        foreach (string uniqueID in gameDataManager.currentGameData.activeCombatNPCIDs)
        {
            var state = gameDataManager.currentGameData.savedWorldNPCs.Find(n => n.uniqueID == uniqueID);
            if (state != null)
            {
                state.isDead = true;
                state.isInCombat = false;
            }
        }
    
        gameDataManager.currentGameData.activeCombatNPCIDs.Clear();
        gameDataManager.currentGameData.activeCombatEnemyIDs.Clear();
        gameDataManager.SaveData();
        
    }

    public void ContinueGame()
    {
        if (!string.IsNullOrEmpty(mainWorldScene))
        {
            SceneManager.LoadScene(mainWorldScene);
        }
        else
        {
            Debug.LogError("zapomněl jsi v Inspectoru vybrat scénu, kam se máš vrátit!");
        }
    }


    void TriggerGameOver()
    {
        if (isBattleOver) return;
        isBattleOver = true;
        StopAllCoroutines();
        Debug.Log("=== GAME OVER ===");
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        foreach (string uniqueID in gameDataManager.currentGameData.activeCombatNPCIDs)
        {
            var state = gameDataManager.currentGameData.savedWorldNPCs.Find(n => n.uniqueID == uniqueID);
            if (state != null) state.isInCombat = false;
        }
        gameDataManager.currentGameData.activeCombatNPCIDs.Clear();
        gameDataManager.currentGameData.activeCombatEnemyIDs.Clear();
        gameDataManager.SaveData();
    }

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

    IEnumerator AnimateToCamera(Camera targetCam, float zoomMultiplier)
    {
        Camera mainCam = camerasInfo.FirstOrDefault(c => c.IDofCamera == 0).targetCamera;
        var ppCam = mainCam.GetComponent<PixelPerfectCamera>();

        Vector3 startPos = mainCam.transform.position;
        Quaternion startRot = mainCam.transform.rotation;
        int startPPU = ppCam.assetsPPU;
        Vector3 endPos = targetCam.transform.position;
        Quaternion endRot = targetCam.transform.rotation;
        int endPPU = zoomMultiplier > 0
            ? Mathf.RoundToInt(defaultPPU * zoomMultiplier)
            : defaultPPU;

        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / transitionDuration);
            mainCam.transform.position = Vector3.Lerp(startPos, endPos, t);
            mainCam.transform.rotation = Quaternion.Lerp(startRot, endRot, t);
            ppCam.assetsPPU = Mathf.RoundToInt(Mathf.Lerp(startPPU, endPPU, t));
            yield return null;
        }

        mainCam.transform.position = endPos;
        mainCam.transform.rotation = endRot;
        ppCam.assetsPPU = endPPU;
    }

    void SwitchToOverviewCamera() => SetActiveCamera(0);
    void SwitchToPlayerCamera(int playerID) => SetActiveCamera(playerID);
    void SwitchToEnemyCamera(int enemyCamID) => SetActiveCamera(enemyCamID);

    void UpdateFaces()
    {
        for (int i = 0; i < FaceHolders.Count; i++)
        {
            if (i >= turnOrder.Count) { FaceHolders[i].gameObject.SetActive(false); continue; }

            FaceHolders[i].gameObject.SetActive(true);
            TurnType turn = turnOrder[i];
            bool lookingEnemy = !IsPlayerTurn(turn);
            int targetID = 0;

            if (lookingEnemy)
            {
                int idx = GetEnemyIndex(turn);
                if (idx >= 0 && idx < currentEnemy.Count) targetID = currentEnemy[idx].id;
            }
            else
            {
                if (turn == TurnType.Player1) targetID = 1;
                else if (turn == TurnType.Player2) targetID = 2;
                else if (turn == TurnType.Player3) targetID = 3;
                else if (turn == TurnType.Player4) targetID = 4;
                else if (turn == TurnType.Player5) targetID = 5;
            }

            FacesSprite found = Faces.FirstOrDefault(f => f.isEnemy == lookingEnemy && f.ID == targetID);
            if (found.sprite != null) FaceHolders[i].sprite = found.sprite;
        }
    }

    void updateEnemyHealthBar()
    {
        int maxHealth = 0, currentHealth = 0;
        foreach (var e in currentEnemy)
        {
            maxHealth += e.maxHealth;
            currentHealth += Mathf.Max(e.health, 0);
        }
        if (maxHealth > 0) EnemyHealthBar.fillAmount = currentHealth / (float)maxHealth;
    }

    void updateCharacterBars()
    {
        for (int i = 0; i < characterBars.Count && i < characters.Count; i++)
        {
            var bars = characterBars[i];
            var ch = characters[i];
            if (bars.healthBar != null) bars.healthBar.fillAmount = Mathf.Clamp01(ch.health / (float)ch.maxHealth);
            if (bars.manaBar != null) bars.manaBar.fillAmount = Mathf.Clamp01(ch.mana / 10f);
        }
    }

    private void ChooseMenu() => chooseMenu.SetActive(!chooseMenu.activeSelf);

    private void ShowArrow(List<GameObject> arrows, int index)
    {
        for (int i = 0; i < arrows.Count; i++)
            arrows[i].SetActive(i == index);
    }

    private void HideArrow()
    {
        arrowsEnemies.ForEach(a => a.SetActive(false));
        arrowsCharacters.ForEach(a => a.SetActive(false));
    }

    public void openChooseAttackUI() => ChooseAttackUI.SetActive(true);
    public void closeChooseAttackUI() => ChooseAttackUI.SetActive(false);
}