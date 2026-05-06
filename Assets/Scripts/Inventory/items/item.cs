using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    // Vždy viditelné
    public int id = -1;
    public string itemName;
    public List<int> allowedCharacterIDs = new List<int>(); // Seznam ID postav
    public Sprite icon;
    public GameObject prefab;   
    public string description;
    public bool isResearched;
    public bool isUsable;
    public bool isTurnedBaseItem;


    //Upravy Dominika kvůli shop systému
    public bool canBeSold = true;
    public int basePrice;
    //konec úprav
    public int maxStack;
    public bool isDefaultItem = false;
    public int defaultAmount = 0;
    public int defaultLevel = 1;
    public ItemType itemType;

    // Zbraně
    public WeaponType weaponType;
    public bool isTurnedBaseWeapon;
    public int firstCharID;
    public bool isMagical; 
    public float Damage;
    public float FireRate;
    public float Range;
    public float ReloadTime;
    public int AmmoCapacity;
    public int AmmoID;
    public int AmmoAmount;
    public float Speed;
    public MagicalElement magicalElement;

    // Léčení a Konzumace
    public int HealAmount;
    public int consumeAmount;
    public int waterAmount;
    public int sleepAmount;
    
    // Brnění
    public int durability;
    public ArmorType armorType;
    public float Armor;
    public int weight;

    // TurnBasedItem

    public TurnBaseItemType turnBaseItemType;
    public int turnBaseItemEffectAmount;
    public int turnBaseItemDuration;
    
    //Samples
    public Rarity rarity;
    public List<MobType> originMobs = new List<MobType>();
    public List<PlantType> originPlants = new List<PlantType>();
    public float researchTimeMinutes = 5f;
    
    [Header("Alchymie a Crafting")]
    public bool canBeUsedInAlchemy; // NOVÉ: Má se to ukázat v alchymistickém inventáři?
    public bool isCrushable; 
    public Item crushedVersion; 
    public int requiredCrushes = 5;
    
    // staty pro alchymii, az kdyz isResearched == true
    public int potionHeal;
    public int potionAditionalHealth;
    public int potionBonusSpeed;
    public int potionBonusStamina;
    public int potionBonusFOV;
    public double potionBonushungerSpeed;
    public int potionBonusdamage;
    public bool hilightResources;
    
    public bool DropsFromMob(MobType mobToCheck)
    {
        return originMobs != null && originMobs.Contains(mobToCheck);
    }
    
    // Tuto funkci použije deník, aby zjistil, jestli z této kytky padá tento item
    public bool DropsFromPlant(PlantType plantToCheck)
    {
        return originPlants != null && originPlants.Contains(plantToCheck);
    }

}

public enum ItemType { Consumable, Healing, Armor, Resource, Weapon, Sample }
public enum Rarity {Common, Uncommon, Rare, Epic, Legendary}
public enum ArmorType { Head, Chest, Legs, Feet, Hands, Shield }
public enum WeaponType { Melee, Ranged, Magic }
public enum MagicalElement { Fire, Water, Earth, Air, Light, Dark, Alien, Star }
public enum TurnBaseItemType {Healing, Buff, Debuff, Mana, Weakening}
public enum MobType { None, Wolf, Bear, Companion, Merchant, Boss , Husband}
public enum PlantType { None, YellowTree, GlowingMushroom, StrangeWeed }