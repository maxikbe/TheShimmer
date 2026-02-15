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
    public List<int> OwnedItemsInventoryItemsIDs = new List<int>();
    public List<int> usableItemIDs = new List<int>();
    public int pickedItemID;
    public List<int> unpickedItemIDs = new List<int>();
}

