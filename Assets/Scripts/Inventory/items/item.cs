using UnityEngine;

[CreateAssetMenu(fileName = "item", menuName = "Scriptable Objects/item")]
public class item : ScriptableObject
{
    public int id;
    public string itemName;
    public Sprite icon;
    public GameObject prefab;   
    public string description;
    public int maxStack;
    public ItemType itemType;
    public bool isDefaultItem;
    public int AmmoID;
    public int AmmoAmount;
    public int HealAmount;
    public float Damage;
    public float Armor;
    public float Speed;
    public float Range;
    public float FireRate;
    public float ReloadTime;
    public int AmmoCapacity;
    public bool isTwoHanded;
    public int durability;
    public int weight;
}

public enum ItemType 
{
    Consumable, 
    armor, 
    Resource,
    gun
}