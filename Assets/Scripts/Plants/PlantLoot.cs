using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))] // Potřebuje trigger collider
[RequireComponent(typeof(PlantController))] // Zároveň to vyžaduje náš nový controller
public class PlantLoot : MonoBehaviour
{
    [Header("Co z kytky padne?")]
    public List<Item> lootItems;
    
    [Header("Nastavení sběru")]
    [Tooltip("Zničí se kytka po vylootování úplně? Pokud ne, zůstane na místě a nepůjde už znovu obrat.")]
    public bool destroyOnLoot = false;

    private PlantController plantController;
    private bool playerInRange = false;

    private void Start()
    {
        plantController = GetComponent<PlantController>();
    }

    private void Update()
    {
        // Hlídáme, jestli je hráč v zóně a zmáčknul E
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (plantController.isLooted)
            {
                Debug.Log("Tahle kytka už je vyždímaná jak tvůj mozek po noční šichtě v kódu!");
                return;
            }
            LootPlant();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Pokud už je kytka vybraná, vůbec UI trigger nezapínáme
            if (plantController.isLooted) return;

            playerInRange = true;
            // tady případně UI turn on na Ecko
            Debug.Log($"Zmáčkni 'E' pro utržení vzorku z {plantController.plantType}!");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void LootPlant()
    {
        // Projedeme celý list vzorků, co z toho má padnout
        foreach (Item staticItem in lootItems)
        {
            ItemSaveData newLootItem = new ItemSaveData();
            newLootItem.id = staticItem.id;
            newLootItem.isOwned = true; 
            newLootItem.level = staticItem.defaultLevel;
            newLootItem.amount = 1; 

            if (gameDataManager.currentGameData != null)
            {
                // Přidání itemu do inventáře úplně stejně jako u CorpseLoot
                gameDataManager.currentGameData.OwnedItems.Add(newLootItem);
                
                Debug.Log($"Lootnul jsi vzorek: {staticItem.itemName} (Uloženo do OwnedItems)");
            }
            else
            {
                Debug.LogError("Kokkotte, gameDataManager.currentGameData je null! Nenačetl se ti save file.");
            }
        }
    
        // Kytka byla vylootována
        plantController.isLooted = true;
    
        // Uložíme stav kytky do JSONu a pošleme info, jestli ji budeme ničit
        plantController.SaveMyState(destroyOnLoot);

        if (destroyOnLoot)
        {
            Destroy(gameObject); // Kytka dělá pápá
        }
        else
        {
            GetComponent<Collider2D>().enabled = false;
            playerInRange = false;
            Debug.Log("Vzorek odebrán, ale kytka tu zůstává na okrasu.");
        }
    }
}