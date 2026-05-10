using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class SkillSaveData
{
    public int id;
    public bool isResearched;
}

public class InitializeGameJson : MonoBehaviour
{
    [SerializeField] private Database _databaseReference;
    [SerializeField] private SkillDatabase _skillDatabaseReference;
    [SerializeField] public List<CharacterAnimationData> characterAnimations = new List<CharacterAnimationData>();
    [SerializeField] private List<EnemyAnimationData> enemyAnimations;
    [SerializeField] public List<EnemySprite> enemySprites = new List<EnemySprite>();

    public struct EnemySprite
    {
        public int id;
        public Sprite sprite;
    }

    private static string savePath;
    private static List<CharacterAnimationData> characterAnimationsStatic;
    private static List<EnemyAnimationData> enemyAnimationsStatic;
    private static List<EnemySprite> enemySpritesStatic;

    private static Database itemDatabase;
    private static SkillDatabase skillDatabase;

    public static string fileName;

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "Data.json");
        itemDatabase = _databaseReference;
        skillDatabase = _skillDatabaseReference;
        characterAnimationsStatic = characterAnimations;
        enemyAnimationsStatic = enemyAnimations;
        enemySpritesStatic = enemySprites;
    }

    public static void CreateSave(string FileName)
    {
        fileName = FileName;
        savePath = Path.Combine(Application.persistentDataPath, FileName);
        SaveInitialData();
        
    }
    public static GameData SaveInitialData()
    {
        if (itemDatabase == null || skillDatabase == null)
        {
            Debug.LogError("CRITICAL: Databáze jsou null! Snažíš se savovat dřív, než projel Awake() v InitializeGameJson!");
            return null;
        }

        GameData data = new GameData();
        data.settings = new SettingsSaver();
        
        var s = data.settings;

        // GameSettings
        s.autoSave = GameSettings.autoSave;
        s.autoSaveTime = GameSettings.autoSaveTime;
        s.currentDifficulty = GameSettings.currentDifficulty;
        s.needToEat = GameSettings.needToEat;
        s.needToDrink = GameSettings.needToDrink;
        s.needToSleep = GameSettings.needToSleep;
        s.staminaEnabled = GameSettings.staminaEnabled;
        s.inventoryKapacityEnabled = GameSettings.inventoryKapacityEnabled;
        s.inventoryKapacity = GameSettings.inventoryKapacity;
        s.masterVolume = GameSettings.masterVolume;
        s.musicVolume = GameSettings.musicVolume;
        s.sfxVolume = GameSettings.sfxVolume;
        s.ambientVolume = GameSettings.ambientVolume;
        s.ambientVolumeEnabled = GameSettings.ambientVolumeEnabled;
        s.sfxVolumeEnabled = GameSettings.sfxVolumeEnabled;
        s.musicVolumeEnabled = GameSettings.musicVolumeEnabled;
        s.currentLanguage = GameSettings.currentLanguage;
        s.fpsShown = GameSettings.fpsShown;
        s.pingShown = GameSettings.pingShown;
        s.FinalSpeechVolume = GameSettings.FinalSpeechVolume;
        s.FinalMusicVolume = GameSettings.FinalMusicVolume;
        s.FinalSfxVolume = GameSettings.FinalSfxVolume;
        s.FinalAmbientVolume = GameSettings.FinalAmbientVolume;

        // KeyBoardSetting
        s.keyUp = KeyBoardSetting.keyUp;
        s.keyDown = KeyBoardSetting.keyDown;
        s.keyLeft = KeyBoardSetting.keyLeft;
        s.keyRight = KeyBoardSetting.keyRight;
        s.keyRun = KeyBoardSetting.keyRun;

        s.Pause = KeyBoardSetting.Pause;
        s.MenuRight = KeyBoardSetting.MenuRight;
        s.MenuLeft = KeyBoardSetting.MenuLeft;
        s.Cancel = KeyBoardSetting.Cancel;
        s.TBinventory = KeyBoardSetting.TBinventory;
        s.NormalInventory = KeyBoardSetting.NormalInventory;
        s.Journal = KeyBoardSetting.Journal;
        s.Codex = KeyBoardSetting.Codex;
        s.Interact = KeyBoardSetting.Interact;
        s.Craft = KeyBoardSetting.Craft;
        s.Tent = KeyBoardSetting.Tent;
        s.Pack = KeyBoardSetting.Pack;
        s.Map = KeyBoardSetting.Map;
        s.InspectItem = KeyBoardSetting.InspectItem;
        s.LightenUp = KeyBoardSetting.LightenUp;

        s.chooseSpecialSpell = KeyBoardSetting.chooseSpecialSpell;
        s.chooseNormalSpell = KeyBoardSetting.chooseNormalSpell;
        s.chooseItem = KeyBoardSetting.chooseItem;
        s.doAccept = KeyBoardSetting.doAccept;
        s.doBack = KeyBoardSetting.doBack;
        s.swapUp = KeyBoardSetting.swapUp;
        s.swapDown = KeyBoardSetting.swapDown;
        s.swapLeft = KeyBoardSetting.swapLeft;
        s.swapRight = KeyBoardSetting.swapRight;
        s.swapAliveUp = KeyBoardSetting.swapAliveUp;
        s.swapAliveDown = KeyBoardSetting.swapAliveDown;
        s.jump = KeyBoardSetting.jump;
        s.dodge = KeyBoardSetting.dodge;
        s.parry = KeyBoardSetting.parry;

        data.characters.Add(new Character { id = 1, name = "Dr. Ventress", health = 150, maxHealth = 150, level = 1, speed = 5.0f, perkUpgradersNumber = 1, pickePerkID1 = 0, pickePerkID2 = 0, pickePerkID3 = 0, mana = 0, critChance = 10, currentEXP = 0 });
        data.characters.Add(new Character { id = 2, name = "Lena", health = 80, maxHealth = 80, level = 1, speed = 4.5f, perkUpgradersNumber = 1, pickePerkID1 = 0, pickePerkID2 = 0, pickePerkID3 = 0, mana = 0, critChance = 20, currentEXP = 0 });
        data.characters.Add(new Character { id = 3, name = "Cass Sheppard", health = 100, maxHealth = 100, level = 1, speed = 7.0f, perkUpgradersNumber = 1, pickePerkID1 = 0, pickePerkID2 = 0, pickePerkID3 = 0, mana = 0, critChance = 15, currentEXP = 0 });
        data.characters.Add(new Character { id = 4, name = "Josie Radek", health = 90, maxHealth = 90, level = 1, speed = 8.0f, perkUpgradersNumber = 1, pickePerkID1 = 0, pickePerkID2 = 0, pickePerkID3 = 0, mana = 0, critChance = 25, currentEXP = 0 });
        data.characters.Add(new Character { id = 5, name = "Anya Thorensen", health = 200, maxHealth = 200, level = 1, speed = 3.0f, perkUpgradersNumber = 1, pickePerkID1 = 0, pickePerkID2 = 0, pickePerkID3 = 0, mana = 0, critChance = 5, currentEXP = 0 });

        List<Item> allItemsFromDB = itemDatabase.GetAllItems();
        List<Skills> allSkillsFromDB = skillDatabase.GetAllSkills();

        foreach (var character in data.characters)
        {
            foreach (var item in allItemsFromDB)
            {
                if (item.allowedCharacterIDs == null || item.allowedCharacterIDs.Count == 0 || item.allowedCharacterIDs.Contains(character.id))
                {
                    character.usableItemIDs.Add(item.id);
                    character.UnOwnedItemsIDs.Add(item.id);
                    if (item.isTurnedBaseWeapon) character.pickableTurnBaseItemIDs.Add(item.id);
                    if (item.isTurnedBaseWeapon && item.firstCharID == character.id)
                    {
                        character.pickedItemID = item.id;
                        character.OwnedItemsInventoryItemsIDs.Add(item.id);
                        character.UnOwnedItemsIDs.Remove(item.id);
                    }
                    if (item.isDefaultItem && !character.OwnedItemsInventoryItemsIDs.Contains(item.id))
                    {
                        character.OwnedItemsInventoryItemsIDs.Add(item.id);
                        character.UnOwnedItemsIDs.Remove(item.id);
                    }
                }
            }
            
        }

        foreach (Item item in allItemsFromDB)
        {
            ItemSaveData newSaveItem = new ItemSaveData();
            newSaveItem.id = item.id;
            newSaveItem.isOwned = item.isDefaultItem;
            newSaveItem.level = item.defaultLevel;
            newSaveItem.amount = item.defaultAmount;

            if (item.allowedCharacterIDs == null || item.allowedCharacterIDs.Count == 0) foreach (var c in data.characters) newSaveItem.allowedCharacterIDs.Add(c.id);
            else newSaveItem.allowedCharacterIDs = new List<int>(item.allowedCharacterIDs);

            if (item.isTurnedBaseWeapon && item.firstCharID != -1 && !newSaveItem.allowedCharacterIDs.Contains(item.firstCharID)) newSaveItem.allowedCharacterIDs.Add(item.firstCharID);

            data.OwnedItems.Add(newSaveItem);
        }

        if (data.Skills == null) data.Skills = new List<SkillSaveData>(); 
        
        foreach (Skills skill in allSkillsFromDB)
        {
            SkillSaveData newSaveSkill = new SkillSaveData();
            newSaveSkill.id = skill.id;
            newSaveSkill.isResearched = skill.isDefault; 
            data.Skills.Add(newSaveSkill);
        }

        data.player = new playerData
        {
            playerName = "Player",
            numberOfCoins = 100,
            numberOfMaterial = 5,
            numberOfGunUpgraders = 1,
            playerPos = new Vector2(0,0),
            time = 0.5f,
            dayNumber = 1,
            currentScene = "MainStoryMap",
            isTentPlaced = false,
            tentPos = new Vector2(0,0),
            thirstLevel = 100f,
            maxThirstLevel = 100f,
            hungerLevel = 100f,
            maxHungerLevel = 100f,
            staminaLevel = 100f,
            maxStaminaLevel = 100f,
            sleepLevel = 100f,
            maxSleepLevel = 100f,
            campFires = new List<CampFire>()
        };

        Perks[] allPerksFromResources = Resources.LoadAll<Perks>("PerksData");

        foreach (Perks perk in allPerksFromResources)
        {
            data.player.foundPerks.Add(perk.id);
        }

        data.enemies.Add(
            new Enemy
            {
                id = 1,
                name = "Speaker",
                maxHealth = 2000,
                health = 2000,
                attacks = new List<EnemyAttack> {

                    // Útok 1 – pomalý švih pláštěm, jeden silný zásah
                    new EnemyAttack {
                        id = 1, attackName = "Shroud Slash",
                        totalAnimationDuration = 2.0f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.9f, damage = 42, parryTimePlayer = 0.30f, dodgeTimePlayer = 0.55f, dodgeType = dodgeType.normal }
                        },
                        weight = 70,
                        numberOfCharHits = 1
                    },

                    // Útok 2 – dvě vlny temné energie po sobě
                    new EnemyAttack {
                        id = 2, attackName = "Void Pulse",
                        totalAnimationDuration = 2.4f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.5f, damage = 44, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.45f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 1.3f, damage = 48, parryTimePlayer = 0.28f, dodgeTimePlayer = 0.50f, dodgeType = dodgeType.normal }
                        },
                        weight = 55,
                        numberOfCharHits = 2
                    },

                    // Útok 3 – série nízkých vln nutící hráče skákat
                    new EnemyAttack {
                        id = 3, attackName = "Root Surge",
                        totalAnimationDuration = 3.0f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.4f, damage = 30, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.45f, dodgeType = dodgeType.jump },
                            new Hit { timeOffset = 1.0f, damage = 30, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.45f, dodgeType = dodgeType.jump },
                            new Hit { timeOffset = 1.6f, damage = 30, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.45f, dodgeType = dodgeType.jump },
                            new Hit { timeOffset = 2.3f, damage = 38, parryTimePlayer = 0.30f, dodgeTimePlayer = 0.55f, dodgeType = dodgeType.jump }
                        },
                        weight = 40,
                        numberOfCharHits = 4
                    },

                    // Útok 4 – rychlý výpad mečem skrytým v plášti
                    new EnemyAttack {
                        id = 4, attackName = "Blade Whisper",
                        totalAnimationDuration = 1.5f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.3f, damage = 42, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.40f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 0.8f, damage = 40, parryTimePlayer = 0.28f, dodgeTimePlayer = 0.48f, dodgeType = dodgeType.normal }
                        },
                        weight = 50,
                        numberOfCharHits = 1
                    },

                    // Útok 5 – pomalý ale devastující zásah s dlouhým telegrafem
                    new EnemyAttack {
                        id = 5, attackName = "Hollow Verdict",
                        totalAnimationDuration = 3.5f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 2.2f, damage = 100, parryTimePlayer = 0.35f, dodgeTimePlayer = 0.65f, dodgeType = dodgeType.normal }
                        },
                        weight = 25,
                        numberOfCharHits = 1
                    },

                    // Útok 6 – chaotický mix: skok + normální, mixuje dodge typy
                    new EnemyAttack {
                        id = 6, attackName = "Unraveling",
                        totalAnimationDuration = 3.2f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.5f, damage = 42, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.45f, dodgeType = dodgeType.jump   },
                            new Hit { timeOffset = 1.1f, damage = 44, parryTimePlayer = 0.27f, dodgeTimePlayer = 0.47f, dodgeType = dodgeType.normal  },
                            new Hit { timeOffset = 1.8f, damage = 42, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.45f, dodgeType = dodgeType.jump   },
                            new Hit { timeOffset = 2.5f, damage = 48, parryTimePlayer = 0.30f, dodgeTimePlayer = 0.55f, dodgeType = dodgeType.normal  }
                        },
                        weight = 35,
                        numberOfCharHits = 3
                    }
                }
            }
        );

        data.enemies.Add(
            new Enemy
            {
                id = 2,
                name = "Bear",
                maxHealth = 420,
                health = 420,
                attacks = new List<EnemyAttack> {

                    // Útok 1 – rychlý kousnutí
                    new EnemyAttack {
                        id = 1, attackName = "Bite",
                        totalAnimationDuration = 1.4f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.6f, damage = 28, parryTimePlayer = 0.28f, dodgeTimePlayer = 0.50f, dodgeType = dodgeType.normal }
                        },
                        weight = 75,
                        numberOfCharHits = 1
                    },

                    // Útok 2 – dvě tlapky za sebou (levá, pravá)
                    new EnemyAttack {
                        id = 2, attackName = "Double Swipe",
                        totalAnimationDuration = 2.0f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.5f, damage = 18, parryTimePlayer = 0.26f, dodgeTimePlayer = 0.46f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 1.1f, damage = 22, parryTimePlayer = 0.28f, dodgeTimePlayer = 0.50f, dodgeType = dodgeType.normal }
                        },
                        weight = 65,
                        numberOfCharHits = 2
                    },

                    // Útok 3 – medvěd se vzpřímí a těžce dopadne oběma tlapama
                    new EnemyAttack {
                        id = 3, attackName = "Bear Slam",
                        totalAnimationDuration = 2.8f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 1.8f, damage = 48, parryTimePlayer = 0.35f, dodgeTimePlayer = 0.65f, dodgeType = dodgeType.normal }
                        },
                        weight = 35,
                        numberOfCharHits = 1
                    },

                    // Útok 4 – nízký útok, strhnutí hráče k zemi
                    new EnemyAttack {
                        id = 4, attackName = "Ground Rake",
                        totalAnimationDuration = 2.2f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.4f, damage = 14, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.45f, dodgeType = dodgeType.jump },
                            new Hit { timeOffset = 0.9f, damage = 14, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.45f, dodgeType = dodgeType.jump },
                            new Hit { timeOffset = 1.4f, damage = 18, parryTimePlayer = 0.28f, dodgeTimePlayer = 0.50f, dodgeType = dodgeType.jump }
                        },
                        weight = 45,
                        numberOfCharHits = 3
                    },

                    // Útok 5 – medvěd se rozběhne a taranuje hráče
                    new EnemyAttack {
                        id = 5, attackName = "Charging Rush",
                        totalAnimationDuration = 2.5f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.6f, damage = 16, parryTimePlayer = 0.26f, dodgeTimePlayer = 0.46f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 1.1f, damage = 16, parryTimePlayer = 0.26f, dodgeTimePlayer = 0.46f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 1.7f, damage = 24, parryTimePlayer = 0.30f, dodgeTimePlayer = 0.55f, dodgeType = dodgeType.normal }
                        },
                        weight = 50,
                        numberOfCharHits = 2
                    },

                    // Útok 6 – zuřivá série tlapek když má málo HP (chaos)
                    new EnemyAttack {
                        id = 6, attackName = "Frenzy Swipe",
                        totalAnimationDuration = 3.0f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.3f, damage = 12, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.45f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 0.7f, damage = 12, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.45f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 1.1f, damage = 12, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.45f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 1.5f, damage = 12, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.45f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 2.0f, damage = 20, parryTimePlayer = 0.28f, dodgeTimePlayer = 0.52f, dodgeType = dodgeType.normal }
                        },
                        weight = 30,
                        numberOfCharHits = 4
                    }
                }
            }
        );

        data.enemies.Add(
            new Enemy
            {
                id = 3,
                name = "The Shimmering Bear",
                maxHealth = 980,
                health = 980,
                attacks = new List<EnemyAttack> {

                    // Útok 1 – bleskově rychké kousnutí s dvojitým snapem čelistí
                    new EnemyAttack {
                        id = 1, attackName = "Skull Snap",
                        totalAnimationDuration = 1.6f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.4f, damage = 28, parryTimePlayer = 0.27f, dodgeTimePlayer = 0.48f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 0.9f, damage = 35, parryTimePlayer = 0.28f, dodgeTimePlayer = 0.50f, dodgeType = dodgeType.normal }
                        },
                        weight = 70,
                        numberOfCharHits = 1
                    },

                    // Útok 2 – medvěd se vzpřímí a řve, pak dopadne oběma tlapami
                    new EnemyAttack {
                        id = 2, attackName = "Death Roar Slam",
                        totalAnimationDuration = 3.2f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 2.2f, damage = 65, parryTimePlayer = 0.38f, dodgeTimePlayer = 0.70f, dodgeType = dodgeType.normal }
                        },
                        weight = 30,
                        numberOfCharHits = 1
                    },

                    // Útok 3 – série drtivých tlapek střídavě
                    new EnemyAttack {
                        id = 3, attackName = "Bone Crush Combo",
                        totalAnimationDuration = 2.8f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.4f, damage = 20, parryTimePlayer = 0.26f, dodgeTimePlayer = 0.46f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 0.9f, damage = 20, parryTimePlayer = 0.26f, dodgeTimePlayer = 0.46f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 1.4f, damage = 20, parryTimePlayer = 0.26f, dodgeTimePlayer = 0.46f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 2.0f, damage = 38, parryTimePlayer = 0.32f, dodgeTimePlayer = 0.58f, dodgeType = dodgeType.normal }
                        },
                        weight = 55,
                        numberOfCharHits = 3
                    },

                    // Útok 4 – nízký smeták tlapou, nutí skákat
                    new EnemyAttack {
                        id = 4, attackName = "Ground Cleave",
                        totalAnimationDuration = 2.0f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.5f, damage = 22, parryTimePlayer = 0.27f, dodgeTimePlayer = 0.48f, dodgeType = dodgeType.jump },
                            new Hit { timeOffset = 1.1f, damage = 28, parryTimePlayer = 0.29f, dodgeTimePlayer = 0.52f, dodgeType = dodgeType.jump }
                        },
                        weight = 50,
                        numberOfCharHits = 2
                    },

                    // Útok 5 – divoký sprint přes celou arenu
                    new EnemyAttack {
                        id = 5, attackName = "Gore Charge",
                        totalAnimationDuration = 2.6f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.5f, damage = 18, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.46f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 1.0f, damage = 18, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.46f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 1.6f, damage = 30, parryTimePlayer = 0.30f, dodgeTimePlayer = 0.55f, dodgeType = dodgeType.normal }
                        },
                        weight = 50,
                        numberOfCharHits = 2
                    },

                    // Útok 6 – mixovaná vlna nízkých + vysokých úderů
                    new EnemyAttack {
                        id = 6, attackName = "Rampage",
                        totalAnimationDuration = 3.6f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.4f, damage = 16, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.45f, dodgeType = dodgeType.jump   },
                            new Hit { timeOffset = 0.9f, damage = 18, parryTimePlayer = 0.26f, dodgeTimePlayer = 0.47f, dodgeType = dodgeType.normal  },
                            new Hit { timeOffset = 1.4f, damage = 16, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.45f, dodgeType = dodgeType.jump   },
                            new Hit { timeOffset = 1.9f, damage = 18, parryTimePlayer = 0.26f, dodgeTimePlayer = 0.47f, dodgeType = dodgeType.normal  },
                            new Hit { timeOffset = 2.6f, damage = 42, parryTimePlayer = 0.33f, dodgeTimePlayer = 0.60f, dodgeType = dodgeType.normal  }
                        },
                        weight = 35,
                        numberOfCharHits = 4
                    },

                    // Útok 7 – finisher, medvěd zachytí hráče čelistmi (neparryvatelný feel)
                    new EnemyAttack {
                        id = 7, attackName = "Hollow Maw",
                        totalAnimationDuration = 4.0f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 1.0f, damage = 25, parryTimePlayer = 0.26f, dodgeTimePlayer = 0.48f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 2.0f, damage = 25, parryTimePlayer = 0.26f, dodgeTimePlayer = 0.48f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 3.2f, damage = 55, parryTimePlayer = 0.36f, dodgeTimePlayer = 0.68f, dodgeType = dodgeType.normal }
                        },
                        weight = 20,
                        numberOfCharHits = 3
                    }
                }
            }
        );

        data.enemies.Add(
            new Enemy
            {
                // Předpokládám, že ID 9 následuje po Mimic Pantherovi
                id = 4, 
                name = "Ocular Weaver",
                // Trochu méně životů než panter, spoléhá spíše na agilitu a triky
                maxHealth = 190, 
                health = 190,
                attacks = new List<EnemyAttack> {

                    // Útok 1 – Základní, rychlé seknutí předními prsty-nohami.
                    new EnemyAttack {
                        id = 1, attackName = "Digit Swipe",
                        totalAnimationDuration = 1.1f,
                        hits = new List<Hit> {
                            // Rychlý hit, standardní okna
                            new Hit { timeOffset = 0.35f, damage = 18, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.45f, dodgeType = dodgeType.normal }
                        },
                        weight = 80, // Častý útok
                        numberOfCharHits = 1
                    },

                    // Útok 2 – Výpad kusadly, delší nápřah, větší poškození.
                    new EnemyAttack {
                        id = 2, attackName = "Venomous Bite",
                        totalAnimationDuration = 1.5f,
                        hits = new List<Hit> {
                            // Pomalý nápřah (0.6s), snadnější na reakci
                            new Hit { timeOffset = 0.6f, damage = 28, parryTimePlayer = 0.30f, dodgeTimePlayer = 0.55f, dodgeType = dodgeType.normal }
                        },
                        weight = 60,
                        numberOfCharHits = 1
                    },

                    // Útok 3 – Vystřelení lepivého vlákna z "oka", nutno přeskočit.
                    new EnemyAttack {
                        id = 3, attackName = "Ocular Web Shot",
                        totalAnimationDuration = 1.8f,
                        hits = new List<Hit> {
                            // Útok letí nízko, nutno jump dodge
                            new Hit { timeOffset = 0.7f, damage = 15, parryTimePlayer = 0.28f, dodgeTimePlayer = 0.50f, dodgeType = dodgeType.jump }
                        },
                        weight = 50,
                        numberOfCharHits = 1
                    },

                    // Útok 4 – Kombo: Rychlé po sobě jdoucí údery prsty zakončené dupnutím.
                    new EnemyAttack {
                        id = 4, attackName = "Frenzy of Fingers",
                        totalAnimationDuration = 2.5f,
                        hits = new List<Hit> {
                            // První tři hity jsou velmi rychlé, těžké na parry po sobě
                            new Hit { timeOffset = 0.3f, damage = 8, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.40f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 0.5f, damage = 8, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.40f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 0.7f, damage = 8, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.40f, dodgeType = dodgeType.normal },
                            // Finální těžký úder (dupnutí všemi nohami)
                            new Hit { timeOffset = 1.3f, damage = 22, parryTimePlayer = 0.32f, dodgeTimePlayer = 0.55f, dodgeType = dodgeType.normal }
                        },
                        weight = 40,
                        numberOfCharHits = 3 // Hráč musí trefit 3 parry
                    },

                    // Útok 5 – Vizuální trik: Velké oko zabliká, Weaver zmizí a objeví se nad hráčem.
                    new EnemyAttack {
                        id = 5, attackName = "Abyssal Drop",
                        totalAnimationDuration = 3.0f,
                        hits = new List<Hit> {
                            // Dlouhá příprava (teleportace, pád), velmi těžká rána
                            new Hit { timeOffset = 2.0f, damage = 45, parryTimePlayer = 0.38f, dodgeTimePlayer = 0.65f, dodgeType = dodgeType.normal }
                        },
                        weight = 25, // Vzácnější, silný útok
                        numberOfCharHits = 1
                    }
                }
            }
        );

        data.enemies.Add(
            new Enemy
            {
                id = 5,
                name = "Coral Dragon",
                maxHealth = 280,
                health = 280,
                attacks = new List<EnemyAttack> {

                    // Útok 1 – rychlý výpad drápy
                    new EnemyAttack {
                        id = 1, attackName = "Claw Strike",
                        totalAnimationDuration = 1.5f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.5f, damage = 20, parryTimePlayer = 0.27f, dodgeTimePlayer = 0.48f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 1.0f, damage = 24, parryTimePlayer = 0.28f, dodgeTimePlayer = 0.50f, dodgeType = dodgeType.normal }
                        },
                        weight = 70,
                        numberOfCharHits = 1
                    },

                    // Útok 2 – výstřel jedovatých korálových trhlin po zemi
                    new EnemyAttack {
                        id = 2, attackName = "Reef Burst",
                        totalAnimationDuration = 2.4f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.5f, damage = 14, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.45f, dodgeType = dodgeType.jump },
                            new Hit { timeOffset = 1.0f, damage = 14, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.45f, dodgeType = dodgeType.jump },
                            new Hit { timeOffset = 1.6f, damage = 14, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.45f, dodgeType = dodgeType.jump }
                        },
                        weight = 55,
                        numberOfCharHits = 3
                    },

                    // Útok 3 – drak se otočí a šlehne ocasem
                    new EnemyAttack {
                        id = 3, attackName = "Tide Whip",
                        totalAnimationDuration = 2.0f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 1.2f, damage = 38, parryTimePlayer = 0.32f, dodgeTimePlayer = 0.58f, dodgeType = dodgeType.normal }
                        },
                        weight = 45,
                        numberOfCharHits = 1
                    },

                    // Útok 4 – výdech ledového/korálového paprsku
                    new EnemyAttack {
                        id = 4, attackName = "Brine Breath",
                        totalAnimationDuration = 3.0f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.8f, damage = 12, parryTimePlayer = 0.26f, dodgeTimePlayer = 0.46f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 1.3f, damage = 12, parryTimePlayer = 0.26f, dodgeTimePlayer = 0.46f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 1.8f, damage = 12, parryTimePlayer = 0.26f, dodgeTimePlayer = 0.46f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 2.4f, damage = 18, parryTimePlayer = 0.29f, dodgeTimePlayer = 0.52f, dodgeType = dodgeType.normal }
                        },
                        weight = 40,
                        numberOfCharHits = 3
                    },

                    // Útok 5 – mix: skok + normální, drak trhá prostor
                    new EnemyAttack {
                        id = 5, attackName = "Abyssal Flurry",
                        totalAnimationDuration = 3.2f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.4f, damage = 14, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.45f, dodgeType = dodgeType.jump   },
                            new Hit { timeOffset = 0.9f, damage = 16, parryTimePlayer = 0.26f, dodgeTimePlayer = 0.47f, dodgeType = dodgeType.normal  },
                            new Hit { timeOffset = 1.5f, damage = 14, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.45f, dodgeType = dodgeType.jump   },
                            new Hit { timeOffset = 2.2f, damage = 22, parryTimePlayer = 0.30f, dodgeTimePlayer = 0.54f, dodgeType = dodgeType.normal  }
                        },
                        weight = 35,
                        numberOfCharHits = 3
                    },

                    // Útok 6 – drak se postaví a těžce dopadne celým tělem
                    new EnemyAttack {
                        id = 6, attackName = "Crushing Depth",
                        totalAnimationDuration = 3.8f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 2.6f, damage = 55, parryTimePlayer = 0.37f, dodgeTimePlayer = 0.68f, dodgeType = dodgeType.normal }
                        },
                        weight = 22,
                        numberOfCharHits = 1
                    }
                }
            }
        );

        data.enemies.Add(
            new Enemy
            {
                id = 6,
                name = "Crocodile",
                maxHealth = 320,
                health = 320,
                attacks = new List<EnemyAttack> {

                    // Útok 1 – bleskové kousnutí čelistmi
                    new EnemyAttack {
                        id = 1, attackName = "Snap",
                        totalAnimationDuration = 1.3f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.5f, damage = 30, parryTimePlayer = 0.28f, dodgeTimePlayer = 0.50f, dodgeType = dodgeType.normal }
                        },
                        weight = 75,
                        numberOfCharHits = 1
                    },

                    // Útok 2 – krok vpřed a dvě rychlá kousnutí
                    new EnemyAttack {
                        id = 2, attackName = "Lunge Bite",
                        totalAnimationDuration = 1.8f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.4f, damage = 18, parryTimePlayer = 0.26f, dodgeTimePlayer = 0.46f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 0.9f, damage = 26, parryTimePlayer = 0.28f, dodgeTimePlayer = 0.50f, dodgeType = dodgeType.normal }
                        },
                        weight = 60,
                        numberOfCharHits = 1
                    },

                    // Útok 3 – ocas smeče po zemi zleva doprava
                    new EnemyAttack {
                        id = 3, attackName = "Tail Sweep",
                        totalAnimationDuration = 2.2f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 1.4f, damage = 36, parryTimePlayer = 0.32f, dodgeTimePlayer = 0.58f, dodgeType = dodgeType.jump }
                        },
                        weight = 50,
                        numberOfCharHits = 1
                    },

                    // Útok 4 – krokodýl se otočí a bije ocasem dvakrát
                    new EnemyAttack {
                        id = 4, attackName = "Death Roll Swipe",
                        totalAnimationDuration = 2.8f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.7f, damage = 16, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.46f, dodgeType = dodgeType.jump   },
                            new Hit { timeOffset = 1.4f, damage = 16, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.46f, dodgeType = dodgeType.jump   },
                            new Hit { timeOffset = 2.1f, damage = 28, parryTimePlayer = 0.30f, dodgeTimePlayer = 0.54f, dodgeType = dodgeType.normal  }
                        },
                        weight = 42,
                        numberOfCharHits = 2
                    },

                    // Útok 5 – sprint a těžký náraz celým tělem
                    new EnemyAttack {
                        id = 5, attackName = "Armored Charge",
                        totalAnimationDuration = 2.4f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.5f, damage = 15, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.45f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 1.0f, damage = 15, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.45f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 1.7f, damage = 28, parryTimePlayer = 0.30f, dodgeTimePlayer = 0.54f, dodgeType = dodgeType.normal }
                        },
                        weight = 45,
                        numberOfCharHits = 2
                    },

                    // Útok 6 – krokodýl se přihrbe a vyskočí s rozevřenými čelistmi
                    new EnemyAttack {
                        id = 6, attackName = "Death Lunge",
                        totalAnimationDuration = 3.4f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 2.4f, damage = 62, parryTimePlayer = 0.36f, dodgeTimePlayer = 0.66f, dodgeType = dodgeType.normal }
                        },
                        weight = 20,
                        numberOfCharHits = 1
                    }
                }
            }
        );

        data.enemies.Add(
            new Enemy
            {
                id = 7,
                name = "Earth Shaker",
                maxHealth = 360,
                health = 360,
                attacks = new List<EnemyAttack> {

                    // Útok 1 – těžká větev shora dolů
                    new EnemyAttack {
                        id = 1, attackName = "Branch Slam",
                        totalAnimationDuration = 2.0f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 1.2f, damage = 35, parryTimePlayer = 0.32f, dodgeTimePlayer = 0.58f, dodgeType = dodgeType.normal }
                        },
                        weight = 70,
                        numberOfCharHits = 1
                    },

                    // Útok 2 – dvě pomalé rány větvemi střídavě
                    new EnemyAttack {
                        id = 2, attackName = "Timber Strike",
                        totalAnimationDuration = 2.8f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.8f, damage = 22, parryTimePlayer = 0.30f, dodgeTimePlayer = 0.54f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 1.8f, damage = 28, parryTimePlayer = 0.32f, dodgeTimePlayer = 0.56f, dodgeType = dodgeType.normal }
                        },
                        weight = 60,
                        numberOfCharHits = 2
                    },

                    // Útok 3 – kořeny vystřelí ze země v řadě
                    new EnemyAttack {
                        id = 3, attackName = "Root Eruption",
                        totalAnimationDuration = 3.0f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.6f, damage = 14, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.46f, dodgeType = dodgeType.jump },
                            new Hit { timeOffset = 1.1f, damage = 14, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.46f, dodgeType = dodgeType.jump },
                            new Hit { timeOffset = 1.6f, damage = 14, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.46f, dodgeType = dodgeType.jump },
                            new Hit { timeOffset = 2.2f, damage = 20, parryTimePlayer = 0.28f, dodgeTimePlayer = 0.52f, dodgeType = dodgeType.jump }
                        },
                        weight = 50,
                        numberOfCharHits = 4
                    },

                    // Útok 4 – třese zemí, vlna otřesu jde po zemi
                    new EnemyAttack {
                        id = 4, attackName = "Seismic Pound",
                        totalAnimationDuration = 2.6f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.5f, damage = 16, parryTimePlayer = 0.26f, dodgeTimePlayer = 0.47f, dodgeType = dodgeType.jump },
                            new Hit { timeOffset = 1.1f, damage = 16, parryTimePlayer = 0.26f, dodgeTimePlayer = 0.47f, dodgeType = dodgeType.jump },
                            new Hit { timeOffset = 1.8f, damage = 24, parryTimePlayer = 0.29f, dodgeTimePlayer = 0.53f, dodgeType = dodgeType.normal }
                        },
                        weight = 45,
                        numberOfCharHits = 2
                    },

                    // Útok 5 – výbuch pylu/spór, mix směrů
                    new EnemyAttack {
                        id = 5, attackName = "Spore Burst",
                        totalAnimationDuration = 2.4f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.5f, damage = 14, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.45f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 1.0f, damage = 14, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.45f, dodgeType = dodgeType.jump   },
                            new Hit { timeOffset = 1.6f, damage = 14, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.45f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 2.0f, damage = 20, parryTimePlayer = 0.28f, dodgeTimePlayer = 0.50f, dodgeType = dodgeType.jump   }
                        },
                        weight = 38,
                        numberOfCharHits = 3
                    },

                    // Útok 6 – celý strom se zhroutí vpřed na hráče
                    new EnemyAttack {
                        id = 6, attackName = "Ancient Fall",
                        totalAnimationDuration = 4.0f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 1.2f, damage = 20, parryTimePlayer = 0.27f, dodgeTimePlayer = 0.50f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 2.2f, damage = 20, parryTimePlayer = 0.27f, dodgeTimePlayer = 0.50f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 3.2f, damage = 55, parryTimePlayer = 0.37f, dodgeTimePlayer = 0.68f, dodgeType = dodgeType.normal }
                        },
                        weight = 20,
                        numberOfCharHits = 2
                    }
                }
            }
        );

        data.enemies.Add(
            new Enemy
            {
                id = 8,
                name = "Glass Heron",
                maxHealth = 150, // Nižší HP kvůli skleněnému tělu
                health = 150,
                attacks = new List<EnemyAttack> {

                    // Útok 1 – Bleskové klovnutí zobákem
                    new EnemyAttack {
                        id = 1, attackName = "Crystalline Stab",
                        totalAnimationDuration = 1.0f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.4f, damage = 20, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.45f, dodgeType = dodgeType.normal }
                        },
                        weight = 85,
                        numberOfCharHits = 1
                    },

                    // Útok 2 – Rozmáchne křídly a vystřelí skleněné střepy (nutno skočit)
                    new EnemyAttack {
                        id = 2, attackName = "Shards of Refraction",
                        totalAnimationDuration = 1.8f,
                        hits = new List<Hit> {
                            // Střepy létají nízko u země
                            new Hit { timeOffset = 0.8f, damage = 15, parryTimePlayer = 0.27f, dodgeTimePlayer = 0.50f, dodgeType = dodgeType.jump }
                        },
                        weight = 65,
                        numberOfCharHits = 1
                    },

                    // Útok 3 – Trojité sekavé kombo křídly
                    new EnemyAttack {
                        id = 3, attackName = "Prismatic Flurry",
                        totalAnimationDuration = 2.2f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.4f, damage = 10, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.42f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 0.8f, damage = 10, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.42f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 1.3f, damage = 15, parryTimePlayer = 0.28f, dodgeTimePlayer = 0.48f, dodgeType = dodgeType.normal }
                        },
                        weight = 50,
                        numberOfCharHits = 2
                    },

                    // Útok 4 – Heron se vznese a prudce narazí do země (plošný útok)
                    new EnemyAttack {
                        id = 4, attackName = "Shatter Dive",
                        totalAnimationDuration = 2.6f,
                        hits = new List<Hit> {
                            // Dlouhý nápřah ve vzduchu, ničivý dopad
                            new Hit { timeOffset = 1.8f, damage = 40, parryTimePlayer = 0.35f, dodgeTimePlayer = 0.60f, dodgeType = dodgeType.normal }
                        },
                        weight = 30,
                        numberOfCharHits = 1
                    },

                    // Útok 5 – Hypnotický záblesk z oka následovaný rychlým výpadem
                    new EnemyAttack {
                        id = 5, attackName = "Ocular Glimmer",
                        totalAnimationDuration = 2.0f,
                        hits = new List<Hit> {
                            // První hit je jen "oslepení" (malý damage), druhý je hlavní rána
                            new Hit { timeOffset = 0.5f, damage = 5, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.40f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 1.2f, damage = 30, parryTimePlayer = 0.30f, dodgeTimePlayer = 0.55f, dodgeType = dodgeType.normal }
                        },
                        weight = 40,
                        numberOfCharHits = 1
                    }
                }
            }
        );



        data.enemies.Add(
            new Enemy
            {
                id = 9,
                name = "Mimic Panther",
                maxHealth = 240,
                health = 240,
                attacks = new List<EnemyAttack> {

                    // Útok 1 – bleskový výpad drápem, těžko čitelný
                    new EnemyAttack {
                        id = 1, attackName = "Phantom Swipe",
                        totalAnimationDuration = 1.2f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.4f, damage = 22, parryTimePlayer = 0.26f, dodgeTimePlayer = 0.46f, dodgeType = dodgeType.normal }
                        },
                        weight = 75,
                        numberOfCharHits = 1
                    },

                    // Útok 2 – skok na hráče z místa
                    new EnemyAttack {
                        id = 2, attackName = "Pounce",
                        totalAnimationDuration = 1.6f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.5f, damage = 18, parryTimePlayer = 0.26f, dodgeTimePlayer = 0.46f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 0.9f, damage = 24, parryTimePlayer = 0.27f, dodgeTimePlayer = 0.48f, dodgeType = dodgeType.normal }
                        },
                        weight = 65,
                        numberOfCharHits = 1
                    },

                    // Útok 3 – série rychlých škrábnutí oběma drápama
                    new EnemyAttack {
                        id = 3, attackName = "Feral Flurry",
                        totalAnimationDuration = 2.2f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.3f, damage = 12, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.45f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 0.6f, damage = 12, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.45f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 0.9f, damage = 12, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.45f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 1.4f, damage = 20, parryTimePlayer = 0.28f, dodgeTimePlayer = 0.50f, dodgeType = dodgeType.normal }
                        },
                        weight = 55,
                        numberOfCharHits = 3
                    },

                    // Útok 4 – přihrbe se a proklouzne pod hráčem, bije ocasem
                    new EnemyAttack {
                        id = 4, attackName = "Shadow Slide",
                        totalAnimationDuration = 2.0f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.6f, damage = 16, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.46f, dodgeType = dodgeType.jump },
                            new Hit { timeOffset = 1.3f, damage = 26, parryTimePlayer = 0.29f, dodgeTimePlayer = 0.52f, dodgeType = dodgeType.normal }
                        },
                        weight = 48,
                        numberOfCharHits = 2
                    },

                    // Útok 5 – napodobí pohyb hráče a zaútočí ze slepého úhlu
                    new EnemyAttack {
                        id = 5, attackName = "Mimicry Lunge",
                        totalAnimationDuration = 2.8f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.4f, damage = 14, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.45f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 1.0f, damage = 14, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.45f, dodgeType = dodgeType.jump   },
                            new Hit { timeOffset = 1.6f, damage = 14, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.45f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 2.2f, damage = 22, parryTimePlayer = 0.29f, dodgeTimePlayer = 0.52f, dodgeType = dodgeType.normal }
                        },
                        weight = 38,
                        numberOfCharHits = 3
                    },

                    // Útok 6 – panther se schoulí a vyskočí s obrovskou silou
                    new EnemyAttack {
                        id = 6, attackName = "Apex Predator",
                        totalAnimationDuration = 3.4f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 2.4f, damage = 58, parryTimePlayer = 0.35f, dodgeTimePlayer = 0.64f, dodgeType = dodgeType.normal }
                        },
                        weight = 18,
                        numberOfCharHits = 1
                    }
                }
            }
        );

        data.enemies.Add(
            new Enemy
            {
                id = 10,
                name = "Marrow Butterfly",
                maxHealth = 320, // Tankovější boss díky krunýři
                health = 320,
                attacks = new List<EnemyAttack> {

                    // Útok 1 – Dupnutí předníma rukama najednou
                    new EnemyAttack {
                        id = 1, attackName = "Ancestral Slam",
                        totalAnimationDuration = 1.4f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.6f, damage = 25, parryTimePlayer = 0.30f, dodgeTimePlayer = 0.55f, dodgeType = dodgeType.normal }
                        },
                        weight = 70,
                        numberOfCharHits = 1
                    },

                    // Útok 2 – Rozmáchne se čelistmi se šperky, které vytvoří řetězový efekt
                    new EnemyAttack {
                        id = 2, attackName = "Jeweled Mandibles",
                        totalAnimationDuration = 1.8f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.7f, damage = 18, parryTimePlayer = 0.28f, dodgeTimePlayer = 0.48f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 1.1f, damage = 12, parryTimePlayer = 0.26f, dodgeTimePlayer = 0.45f, dodgeType = dodgeType.normal }
                        },
                        weight = 60,
                        numberOfCharHits = 2
                    },

                    // Útok 3 – "Vlna" rukama zepředu dozadu, nutno přeskočit (nízký útok)
                    new EnemyAttack {
                        id = 3, attackName = "Crawling Ripples",
                        totalAnimationDuration = 2.4f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.5f, damage = 15, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.45f, dodgeType = dodgeType.jump },
                            new Hit { timeOffset = 1.0f, damage = 15, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.45f, dodgeType = dodgeType.jump }
                        },
                        weight = 45,
                        numberOfCharHits = 2
                    },

                    // Útok 4 – Rychlá série úderů mnoha rukama (vypadá to jako chaotické bubnování)
                    new EnemyAttack {
                        id = 4, attackName = "Thrumming Marrow",
                        totalAnimationDuration = 3.0f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.4f, damage = 6, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.40f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 0.6f, damage = 6, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.40f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 0.8f, damage = 6, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.40f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 1.0f, damage = 6, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.40f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 1.8f, damage = 24, parryTimePlayer = 0.32f, dodgeTimePlayer = 0.58f, dodgeType = dodgeType.normal }
                        },
                        weight = 35,
                        numberOfCharHits = 4
                    },

                    // Útok 5 – Z očí na krunýři vystřelí elektrické výboje (modré žilkování na obrázku)
                    new EnemyAttack {
                        id = 5, attackName = "Neural Discharge",
                        totalAnimationDuration = 2.2f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 1.2f, damage = 35, parryTimePlayer = 0.30f, dodgeTimePlayer = 0.60f, dodgeType = dodgeType.normal }
                        },
                        weight = 40,
                        numberOfCharHits = 1
                    },

                    // Útok 6 – Vzpne se a celou vahou dopadne na hráče
                    new EnemyAttack {
                        id = 6, attackName = "Larval Crush",
                        totalAnimationDuration = 3.5f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 2.4f, damage = 55, parryTimePlayer = 0.40f, dodgeTimePlayer = 0.70f, dodgeType = dodgeType.normal }
                        },
                        weight = 20,
                        numberOfCharHits = 1
                    }
                }
            }
        );

        data.enemies.Add(
            new Enemy
            {
                id = 11,
                name = "Mycelium Howler",
                maxHealth = 260,
                health = 260,
                attacks = new List<EnemyAttack> {

                    // Útok 1 – Prudké kousnutí s nápřahem
                    new EnemyAttack {
                        id = 1, attackName = "Fungal Bite",
                        totalAnimationDuration = 1.3f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.5f, damage = 24, parryTimePlayer = 0.28f, dodgeTimePlayer = 0.50f, dodgeType = dodgeType.normal }
                        },
                        weight = 75,
                        numberOfCharHits = 1
                    },

                    // Útok 2 – Rozmáchne se tlapou a vypustí oblak spor (plošný útok)
                    new EnemyAttack {
                        id = 2, attackName = "Spore Swipe",
                        totalAnimationDuration = 1.6f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.6f, damage = 18, parryTimePlayer = 0.26f, dodgeTimePlayer = 0.48f, dodgeType = dodgeType.normal }
                        },
                        weight = 65,
                        numberOfCharHits = 1
                    },

                    // Útok 3 – Výskok a dopad, který vyvolá rázovou vlnu hub (nutno skočit)
                    new EnemyAttack {
                        id = 3, attackName = "Mycelial Burst",
                        totalAnimationDuration = 2.0f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 1.1f, damage = 22, parryTimePlayer = 0.30f, dodgeTimePlayer = 0.55f, dodgeType = dodgeType.jump }
                        },
                        weight = 50,
                        numberOfCharHits = 1
                    },

                    // Útok 4 – Rychlá kombinace tří kousnutí za pohybu
                    new EnemyAttack {
                        id = 4, attackName = "Howler's Hunger",
                        totalAnimationDuration = 2.4f,
                        hits = new List<Hit> {
                            new Hit { timeOffset = 0.4f, damage = 12, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.42f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 0.9f, damage = 12, parryTimePlayer = 0.25f, dodgeTimePlayer = 0.42f, dodgeType = dodgeType.normal },
                            new Hit { timeOffset = 1.5f, damage = 18, parryTimePlayer = 0.28f, dodgeTimePlayer = 0.48f, dodgeType = dodgeType.normal }
                        },
                        weight = 55,
                        numberOfCharHits = 2
                    },

                    // Útok 5 – Vlk se zastaví, oko se rozzáří a vyvolá ochromující vytí
                    new EnemyAttack {
                        id = 5, attackName = "Resonant Howl",
                        totalAnimationDuration = 3.2f,
                        hits = new List<Hit> {
                            // Pozvolný náběh zvuku, silné poškození
                            new Hit { timeOffset = 2.1f, damage = 35, parryTimePlayer = 0.35f, dodgeTimePlayer = 0.62f, dodgeType = dodgeType.normal }
                        },
                        weight = 30,
                        numberOfCharHits = 1
                    }
                }
            }
        );



        foreach (var enemySprite in enemySpritesStatic)
        {
            Enemy targetEnemy = data.enemies.Find(e => e.id == enemySprite.id);

            if (targetEnemy != null)
            {
                targetEnemy.sprite = enemySprite.sprite;
            }
        }

        data.characterAnimations = characterAnimationsStatic;
        data.enemyAnimations = enemyAnimationsStatic;
        gameDataManager.currentGameData = data;
        gameDataManager.userDefaultName = fileName;

        string json = JsonUtility.ToJson(data, true);
        Debug.Log("Vytvoření nového souboru s názvem: " + savePath);
        File.WriteAllText(savePath, json);

        return data;
    }
}