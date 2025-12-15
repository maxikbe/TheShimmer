using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<item> items = new List<item>();

    public int maxWeight = 20;
    public void AddItem(item newItem)
    {
        if(GetCurrentWeight() < maxWeight)
        {
            items.Add(newItem);
        } else
        {
            Debug.Log("Inventory is full!");
        }
    }

    public void RemoveItem(item itemToRemove)
    {
        if (items.Contains(itemToRemove))
        {
            items.Remove(itemToRemove);
        }
        else
        {
            Debug.Log("Item not found in inventory!");
        }
    }

    public void UseItem(item itemToUse)
    {
        if (items.Contains(itemToUse))
        {
            // logika toho co se stane kdyz item pouzijes
            Debug.Log("Used item: " + itemToUse.itemName);
            RemoveItem(itemToUse);
        }
        else
        {
            Debug.Log("Item not found in inventory!");
        }
    }

    public int GetCurrentWeight()
    {
        int currentWeight = 0;
        foreach (item itm in items)
        {
            currentWeight += itm.weight;
        }
        return currentWeight;
    }
}
