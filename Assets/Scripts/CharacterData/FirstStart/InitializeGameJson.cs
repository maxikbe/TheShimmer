using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class SkillSaveData
{
    public int id;
    public int characterID;
    public int skillLevel;
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

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "Data.json");
        itemDatabase = _databaseReference;
        skillDatabase = _skillDatabaseReference;
        characterAnimationsStatic = characterAnimations;
        enemyAnimationsStatic = enemyAnimations;
        enemySpritesStatic = enemySprites;

        if (!File.Exists(savePath))
        {
            SaveInitialData();
        }
    }

    public static GameData SaveInitialData()
    {
        if (itemDatabase == null || skillDatabase == null)
        {
            return null;
        }

        GameData data = new GameData();

        data.characters.Add(new Character { id = 1, name = "Dr. Ventress", health = 150, maxHealth = 150, level = 1, speed = 5.0f, perkUpgradersNumber = 1, pickePerkID1 = 0, pickePerkID2 = 0, pickePerkID3 = 0, mana = 0, critChance = 10 });
        data.characters.Add(new Character { id = 2, name = "Lena", health = 80, maxHealth = 80, level = 1, speed = 4.5f, perkUpgradersNumber = 1, pickePerkID1 = 0, pickePerkID2 = 0, pickePerkID3 = 0, mana = 0, critChance = 20 });
        data.characters.Add(new Character { id = 3, name = "Cass Sheppard", health = 100, maxHealth = 100, level = 1, speed = 7.0f, perkUpgradersNumber = 1, pickePerkID1 = 0, pickePerkID2 = 0, pickePerkID3 = 0, mana = 0, critChance = 15 });
        data.characters.Add(new Character { id = 4, name = "Josie Radek", health = 90, maxHealth = 90, level = 1, speed = 8.0f, perkUpgradersNumber = 1, pickePerkID1 = 0, pickePerkID2 = 0, pickePerkID3 = 0, mana = 0, critChance = 25 });
        data.characters.Add(new Character { id = 5, name = "Anya Thorensen", health = 200, maxHealth = 200, level = 1, speed = 3.0f, perkUpgradersNumber = 1, pickePerkID1 = 0, pickePerkID2 = 0, pickePerkID3 = 0, mana = 0, critChance = 5 });

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
            newSaveSkill.characterID = skill.characterID;
            newSaveSkill.skillLevel = skill.skillLevel;

            data.Skills.Add(newSaveSkill);
        }

        data.player = new playerData
        {
            playerName = "Player",
            numberOfCoins = 100,
            numberOfMaterial = 5,
            numberOfGunUpgraders = 1
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
                maxHealth = 150,
                health = 150,
                attacks = new List<EnemyAttack> {
                    new EnemyAttack { 
                        id = 1, attackName = "Thunderous Word", totalAnimationDuration = 1.8f, 
                        hits = new List<Hit> { new Hit { timeOffset = 0.8f, damage = 25, dodgeTimePlayer = 0.4f, dodgeType = dodgeType.normal } }, 
                        weight = 60 
                    },
                    new EnemyAttack { 
                        id = 2, attackName = "Static Discharge", totalAnimationDuration = 1.2f, 
                        hits = new List<Hit> { new Hit { timeOffset = 0.4f, damage = 15, dodgeTimePlayer = 0.3f, dodgeType = dodgeType.jump } }, 
                        weight = 40 
                    }
                }
            }
        );

        data.enemies.Add(
            new Enemy
            {
                id = 2,
                name = "Screaming Bear",
                maxHealth = 400,
                health = 400,
                attacks = new List<EnemyAttack> {
                    new EnemyAttack { 
                        id = 1, attackName = "Desperate Shiver", totalAnimationDuration = 2.5f, 
                        hits = new List<Hit> { new Hit { timeOffset = 1.5f, damage = 50, dodgeTimePlayer = 0.5f, dodgeType = dodgeType.normal } }, 
                        weight = 70 
                    },
                    new EnemyAttack { 
                        id = 2, attackName = "Bone-Chilling Roar", totalAnimationDuration = 2.0f, 
                        hits = new List<Hit> { new Hit { timeOffset = 0.8f, damage = 20, dodgeTimePlayer = 0.6f, dodgeType = dodgeType.jump } }, 
                        weight = 30 
                    }
                }
            }
        );

        data.enemies.Add(
            new Enemy
            {
                id = 3,
                name = "The Crawler",
                maxHealth = 600,
                health = 600,
                attacks = new List<EnemyAttack> {
                    new EnemyAttack { 
                        id = 1, attackName = "Lighthouse Bloom", totalAnimationDuration = 3.5f, 
                        hits = new List<Hit> { new Hit { timeOffset = 2.5f, damage = 70, dodgeTimePlayer = 0.3f, dodgeType = dodgeType.normal } }, 
                        weight = 50 
                    },
                    new EnemyAttack { 
                        id = 2, attackName = "Topographical Anomaly", totalAnimationDuration = 2.2f, 
                        hits = new List<Hit> { new Hit { timeOffset = 1.0f, damage = 30, dodgeTimePlayer = 0.5f, dodgeType = dodgeType.jump } }, 
                        weight = 50 
                    }
                }
            }
        );

        data.enemies.Add(
            new Enemy
            {
                id = 4,
                name = "The Mimic",
                maxHealth = 200,
                health = 200,
                attacks = new List<EnemyAttack> {
                    new EnemyAttack { 
                        id = 1, attackName = "Perfect Reflection", totalAnimationDuration = 1.5f, 
                        hits = new List<Hit> { new Hit { timeOffset = 0.7f, damage = 40, dodgeTimePlayer = 0.2f, dodgeType = dodgeType.normal } }, 
                        weight = 100 
                    }
                }
            }
        );

        data.enemies.Add(
            new Enemy
            {
                id = 5,
                name = "Shimmer Alligator",
                maxHealth = 250,
                health = 250,
                attacks = new List<EnemyAttack> {
                    new EnemyAttack { 
                        id = 1, attackName = "Lunge", totalAnimationDuration = 1.3f, 
                        hits = new List<Hit> { new Hit { timeOffset = 0.4f, damage = 35, dodgeTimePlayer = 0.4f, dodgeType = dodgeType.normal } }, 
                        weight = 80 
                    },
                    new EnemyAttack { 
                        id = 2, attackName = "Tail Whip", totalAnimationDuration = 1.6f, 
                        hits = new List<Hit> { new Hit { timeOffset = 0.8f, damage = 25, dodgeTimePlayer = 0.5f, dodgeType = dodgeType.jump } }, 
                        weight = 20 
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

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);

        return data;
    }
}