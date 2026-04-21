using System;
using System.Collections.Generic;
using UnityEngine;

public class Merchant : MonoBehaviour
{
    public enum MerchantType { Weaponsmith, Pharmacist, Generalist }
    
    public MerchantType merchantType;
    
    [Header("Identifikace (PRO ULOŽENÍ)")]
    public string merchantID; // unikatni ID
    
    [Header("Cenová politika")]
    public float buyModifier = 0.5f;    
    public float sellModifier = 1.2f;
    
    [Header("Smlouvání a Reputace")]
    [Tooltip("Kolik % pod/nad cenu snese (0.15 = 15%)")]
    public float haggleTolerance = 0.15f; 
    public int maxPatience = 3; // kolikrat ho muzu nsrat
    [HideInInspector] public int currentPatience;

    //  public bool playerHasHagglePerk = false; // potom perk funkce na zjisteni

    [Header("Podpultovky (Secret Stash)")]
    [Tooltip("Kolik reputace potřebuješ pro odemčení (0-100)")]
    public float repRequiredForSecret = 80f; 
    [SerializeField] private List<Item> secretStock = new List<Item>();
    // ---------------------------------

    [Header("Výchozí zásoby")]
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
        currentPatience = maxPatience; // pri restocku reset trpelivosti

        foreach (Item itemTemplate in startingStock)
        {
            ItemSaveData newItem = new ItemSaveData();
            newItem.id = itemTemplate.id;
            newItem.isOwned = false;
            newItem.level = itemTemplate.defaultLevel;
            newItem.amount = 1;
            
            currentInventory.Add(newItem);
        }

        // pokud je nactene a mame dost repu tak zobrazuju produkty
        if (gameDataManager.currentGameData != null)
        {
            MerchantReputation rep = gameDataManager.currentGameData.merchantReputations.Find(r => r.merchantID == merchantID);
            
            if (rep != null && rep.reputationValue >= repRequiredForSecret)
            {
                foreach (Item secretTemplate in secretStock)
                {
                    ItemSaveData newSecret = new ItemSaveData();
                    newSecret.id = secretTemplate.id;
                    newSecret.isOwned = false;
                    newSecret.level = secretTemplate.defaultLevel;
                    newSecret.amount = 1;
                    
                    currentInventory.Add(newSecret);
                }
            }
        }
    }
}