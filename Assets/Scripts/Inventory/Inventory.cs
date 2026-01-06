using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
   public Item[] allItems; 

    void Start()
    {
        Debug.Log("Načítání všech položek inventáře...");
        allItems = Resources.LoadAll<Item>("AllItems");

        foreach (var item in allItems)
        {
            Debug.Log("Načteno: " + item.name);
        }
    }
}