using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Character
{
    public int id;
    public string name;
    public int health;
    public int level;
    public float speed;
    public int HungerLevel;
    public int ThirstLevel;
    public int StaminaLevel;
    public int NervousnessLevel;
    public List<int> OwnedItemsInventoryItemsIDs = new List<int>();
    public List<int> UnOwnedItemsIDs = new List<int>();
    public List<int> usableItemIDs = new List<int>();
    public List<int> pickableTurnBaseItemIDs = new List<int>();
    public int pickedItemID;
    public List<int> unpickedItemIDs = new List<int>();
    public List<int> perksIDs = new List<int>();
    public List<int> unfoundPerksIDs = new List<int>();
    public List<int> usablePerksIDs = new List<int>();
}

