using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))] // potrebuje triggercolider
public class CorpseLoot : MonoBehaviour
{
    [Header("Co z něj padne?")]
    public List<Item> lootItems;
    
    private bool playerInRange = false;

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            LootCorpse();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            // tady kdyztak UI turn on na ecko
            Debug.Log("Zmačkni 'E' pro lootování!");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void LootCorpse()
    {
        // projede se všechno
        foreach (Item staticItem in lootItems)
        {
            ItemSaveData newLootItem = new ItemSaveData();
            newLootItem.id = staticItem.id;
            newLootItem.isOwned = true; 
            newLootItem.level = staticItem.defaultLevel;
            newLootItem.amount = 1; 

            if (gameDataManager.currentGameData != null)
            {
                gameDataManager.currentGameData.OwnedItems.Add(newLootItem);
                
                Debug.Log($"Lootnul jsi: {staticItem.itemName} (Uloženo do OwnedItems)");
            }
            else
            {
                Debug.LogError("Kokkotte, gameDataManager.currentGameData je null! Nenačetl se ti save file.");
            }
        }

        //gameDataManager.SaveData();

        Destroy(gameObject);
    }
}