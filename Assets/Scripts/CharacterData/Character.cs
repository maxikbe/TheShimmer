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
    public float speed; 
    public float ExperiencePoints;
    
    public int attack;
    public int defense;

    public int perkUpgradersNumber;
    public int hungerLevel;
    public int thirstLevel;
    public int staminaLevel;
    public int nervousnessLevel;

    public int pickedPerkID1;
    public int pickedPerkID2;
    public int pickedPerkID3;

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