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
    public int maxStack;
    public bool isDefaultItem = false;
    public int defaultAmount = 0;
    public int defaultLevel = 1;
    public ItemType itemType;

    // Zbraně
    public WeaponType weaponType;
    public bool isTurnedBaseWeapon;
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
    
    // Brnění
    public int durability;
    public ArmorType armorType;
    public float Armor;
    public int weight;
}

public enum ItemType { Consumable, Healing, Armor, Resource, Weapon }
public enum ArmorType { Head, Chest, Legs, Feet, Hands, Shield }
public enum WeaponType { Melee, Ranged, Magic }
public enum MagicalElement { Fire, Water, Earth, Air, Light, Dark, Alien, Star }