using System;
using System.Collections.Generic;
using UnityEngine;

public class Merchant : MonoBehaviour
{
    public enum MerchantType { Weaponsmith, Pharmacist, Generalist }
    
    public MerchantType merchantType;
    
    [Header("Cenová politika")]
    public float buyModifier = 0.5f;
    public float sellModifier = 1.2f;

    [Header("Výchozí zásoby (Šablony z Unity)")]
    [SerializeField] private List<Item> startingStock = new List<Item>();
    
    [Header("Aktuální inventář (Živá data pro Save)")]
    public List<ItemSaveData> currentInventory = new List<ItemSaveData>();

    private void Start()
    {
        RestockInventory();
    }

    public void RestockInventory()
    {
        currentInventory.Clear();

        foreach (Item itemTemplate in startingStock)
        {
            ItemSaveData newItem = new ItemSaveData();
            newItem.id = itemTemplate.id;
            newItem.isOwned = false;
            newItem.level = itemTemplate.defaultLevel;
            newItem.amount = 1;
            
            
            currentInventory.Add(newItem);
        }
    }
}
