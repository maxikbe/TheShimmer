using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class SimpleLoot : MonoBehaviour
{
    [Header("Co z toho vypadne?")]
    [SerializeField] private List<Item> lootItems;
    
    [Header("Nastavení lootu")]
    [Tooltip("Zničí se objekt po vylootování? (např. bedna zmizí)")]
    [SerializeField] private bool destroyOnLoot = false;

    private bool isLooted = false;
    private bool playerInRange = false;

    private void Update()
    {
        // Kontrola, jestli hráč stojí blízko, objekt není prázdný a zmáčkl Interact klávesu
        if (playerInRange && !isLooted && Input.GetKeyDown(KeyBoardSetting.Interact))
        {
            LootIt();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isLooted)
        {
            playerInRange = true;
            Debug.Log("Základní loot na dosah! Zmáčkni 'E' (nebo tvůj bind).");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void LootIt()
    {
        isLooted = true; // Pojistka proti double-lootu

        if (gameDataManager.currentGameData != null)
        {
            foreach (Item staticItem in lootItems)
            {
                // Vytvoříme instanci pro save data
                ItemSaveData newLootItem = new ItemSaveData();
                newLootItem.id = staticItem.id;
                newLootItem.isOwned = true; 
                newLootItem.level = staticItem.defaultLevel;
                newLootItem.amount = staticItem.defaultAmount > 0 ? staticItem.defaultAmount : 1; 

                // Šup s tím do kapes
                gameDataManager.currentGameData.OwnedItems.Add(newLootItem);
                
                // Zavoláme UI manažera, ať to ukáže v rohu obrazovky
                if (LootUIManager.Instance != null)
                {
                    LootUIManager.Instance.ShowLootNotification(staticItem);
                }
                else
                {
                    Debug.LogWarning("Kokkotte, chybí ti ve scéně LootUIManager!");
                }
            }
        }
        else
        {
            Debug.LogError("gameDataManager.currentGameData je null! Nenačetl se save.");
        }

        // Úklid po loocení
        if (destroyOnLoot)
        {
            Destroy(gameObject);
        }
        else
        {
            // Pokud bedna zůstává, jen vypneme collider, ať to hráče už neotravuje
            GetComponent<Collider2D>().enabled = false;
            playerInRange = false;
            Debug.Log("Vylootováno, schránka tu ale zůstává.");
        }
    }
}