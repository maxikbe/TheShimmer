using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))] // potrebuje triggercolider
public class CorpseLoot : MonoBehaviour
{
    [Header("Identifikace pro Bestiář")]
    public MobType mobOrigin = MobType.None; // TADY SI V INSPEKTORU VYBEREŠ, CO TO JE ZA ZVÍŘE

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
        if (gameDataManager.currentGameData != null)
        {
            // 1. ZÁPIS DO BESTIÁŘE (Pokud to není MobType.None a ještě to nemáme)
            if (mobOrigin != MobType.None && !gameDataManager.currentGameData.unlockedBestiary.Contains(mobOrigin))
            {
                gameDataManager.currentGameData.unlockedBestiary.Add(mobOrigin);
                Debug.Log($"[Deník] Nový záznam v Bestiáři: {mobOrigin}");
            }

            // 2. SEBRÁNÍ ITEMŮ
            foreach (Item staticItem in lootItems)
            {
                ItemSaveData newLootItem = new ItemSaveData();
                newLootItem.id = staticItem.id;
                newLootItem.isOwned = true; 
                newLootItem.level = staticItem.defaultLevel;
                newLootItem.amount = 1; 

                gameDataManager.currentGameData.OwnedItems.Add(newLootItem);
                Debug.Log($"Lootnul jsi: {staticItem.itemName} (Uloženo do OwnedItems)");
            }
        }
        else
        {
            Debug.LogError("Kokkotte, gameDataManager.currentGameData je null! Nenačetl se ti save file.");
        }

        Destroy(gameObject);
    }
}