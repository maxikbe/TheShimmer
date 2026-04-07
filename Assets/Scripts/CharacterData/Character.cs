using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Character
{
    public int id;
    public string name;
    public int health;
    public int maxHealth;
    public int level;
    public int mana = 0;
    public float speed; 
    public float ExperiencePoints;
    
    public int attack;
    public int defense;

    public int perkUpgradersNumber;
    public int hungerLevel;
    public int thirstLevel;
    public int staminaLevel;
    public int nervousnessLevel;

    public int pickePerkID1;
    public int pickePerkID2;
    public int pickePerkID3;

    public int pickedItemID;

    public List<int> usableItemIDs = new List<int>();
    public List<int> UnOwnedItemsIDs = new List<int>();
    public List<int> pickableTurnBaseItemIDs = new List<int>();
    public List<int> OwnedItemsInventoryItemsIDs = new List<int>();
    
    public List<int> ownedItemsIDs = new List<int>();
    public List<int> equippedUpgradesIDs = new List<int>();
    public List<int> activeBuffsIDs = new List<int>();

    public bool isDead => health <= 0;

    public void TakeDamage(int damage)
    {
        int finalDamage = Mathf.Max(1, damage - defense);
        health -= finalDamage;
        if (health < 0) health = 0;
    }

    public void ConsumeItem(int itemID)
    {
        if (ownedItemsIDs.Contains(itemID))
        {
            ownedItemsIDs.Remove(itemID);
        }
    }
}