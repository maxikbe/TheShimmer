using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public InventoryUI inventoryUI; 

    [SerializeField] public List<Item> items = new List<Item>();
    
    public int maxWeight = 20;

    public void AddItem(Item newItem)
    {
        if(GetCurrentWeight() + newItem.weight <= maxWeight) 
        {
            items.Add(newItem);
            if (inventoryUI != null)
            {
                inventoryUI.UpdateInventoryUI();
            }
            Debug.Log("Předmět " + newItem.itemName + " přidán.Aktuální váha: " + GetCurrentWeight() + "/" + maxWeight);
        } 
        else
        {
            Debug.Log("Inventory is full! (Weight limit reached: " + maxWeight + ")");
        }
    }

    public void RemoveItem(Item itemToRemove)
    {
        if (items.Contains(itemToRemove))
        {
            items.Remove(itemToRemove);

            if (inventoryUI != null)
            {
                inventoryUI.UpdateInventoryUI();
            }
        }
        else
        {
            Debug.Log("Item not found in inventory!");
        }
    }

    public void UseItem(Item itemToUse)
    {
        if (items.Contains(itemToUse))
        {
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
        foreach (Item itm in items)
        {
            currentWeight += itm.weight;
        }
        return currentWeight;
    }
}