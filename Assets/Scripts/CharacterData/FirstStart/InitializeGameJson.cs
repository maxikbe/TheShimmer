using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class InitializeGameJson : MonoBehaviour
{
    [SerializeField] private Database _databaseReference; 

    private static string savePath;
    private static Database itemDatabase;

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "Data.json");
        itemDatabase = _databaseReference;

        if (!File.Exists(savePath))
        {
            Debug.Log("Soubor Data.json nenalezen. Provádím první inicializaci...");
            SaveInitialData();
        }
    }

    public static GameData SaveInitialData()
{
    if (itemDatabase == null)
    {
        Debug.LogError("Chyba: ItemDatabase není přiřazena!");
        return null;
    }

    GameData data = new GameData();

    data.characters.Add(new Character { id = 1, name = "Dr. Ventress", health = 150, level = 1, speed = 5.0f, perkUpgradersNumber = 1, pickePerkID1 = 0, pickePerkID2 = 0, pickePerkID3 = 0,});
    data.characters.Add(new Character { id = 2, name = "Lena", health = 80, level = 1, speed = 4.5f, perkUpgradersNumber = 1, pickePerkID1 = 0, pickePerkID2 = 0, pickePerkID3 = 0, });
    data.characters.Add(new Character { id = 3, name = "Cass Sheppard", health = 100, level = 1, speed = 7.0f, perkUpgradersNumber = 1, pickePerkID1 = 0, pickePerkID2 = 0, pickePerkID3 = 0, });
    data.characters.Add(new Character { id = 4, name = "Josie Radek", health = 90, level = 1, speed = 8.0f, perkUpgradersNumber = 1, pickePerkID1 = 0, pickePerkID2 = 0, pickePerkID3 = 0, });
    data.characters.Add(new Character { id = 5, name = "Anya Thorensen", health = 200, level = 1, speed = 3.0f, perkUpgradersNumber = 1, pickePerkID1 = 0, pickePerkID2 = 0, pickePerkID3 = 0, });

    List<Item> allItemsFromDB = itemDatabase.GetAllItems();

    foreach (var character in data.characters)
    {
        foreach (var item in allItemsFromDB)
        {
            if (item.allowedCharacterIDs == null || item.allowedCharacterIDs.Count == 0 || item.allowedCharacterIDs.Contains(character.id))
            {
                character.usableItemIDs.Add(item.id);
                character.UnOwnedItemsIDs.Add(item.id);    
                if(item.isTurnedBaseWeapon) character.pickableTurnBaseItemIDs.Add(item.id);
                if(item.isTurnedBaseWeapon && item.firstCharID == character.id )
                {
                    character.pickedItemID = item.id;
                    character.OwnedItemsInventoryItemsIDs.Add(item.id);
                    character.UnOwnedItemsIDs.Remove(item.id);
                } 
                if(item.isDefaultItem && !character.OwnedItemsInventoryItemsIDs.Contains(item.id))
                {
                    character.OwnedItemsInventoryItemsIDs.Add(item.id);
                    character.UnOwnedItemsIDs.Remove(item.id);
                }

            }
        }
    }

    foreach (Item item in allItemsFromDB)
    {
        ItemSaveData newSaveItem = new ItemSaveData();
        newSaveItem.id = item.id;
        newSaveItem.isOwned = item.isDefaultItem; 
        newSaveItem.level = item.defaultLevel;
        newSaveItem.amount = item.defaultAmount;

        if (item.allowedCharacterIDs == null || item.allowedCharacterIDs.Count == 0) foreach (var c in data.characters) newSaveItem.allowedCharacterIDs.Add(c.id);
        else newSaveItem.allowedCharacterIDs = new List<int>(item.allowedCharacterIDs);
        
        if(item.isTurnedBaseWeapon && item.firstCharID != -1 && !newSaveItem.allowedCharacterIDs.Contains(item.firstCharID)) newSaveItem.allowedCharacterIDs.Add(item.firstCharID);
        

        data.OwnedItems.Add(newSaveItem);
    }



   data.player = new playerData 
    {
        playerName = "Player",
        numberOfCoins = 100,
        numberOfMaterial = 5,
        numberOfGunUpgraders = 1
    };

    Perks[] allPerksFromResources = Resources.LoadAll<Perks>("PerksData");

    foreach (Perks perk in allPerksFromResources)
    {
        //data.player.unFoundPerks.Add(perk.id);
        data.player.foundPerks.Add(perk.id); //toto pak vyměnit za to nahoře, tohle je jen na debug
    }

    string json = JsonUtility.ToJson(data, true);
    File.WriteAllText(savePath, json);

    Debug.Log("JSON inicializován.");
    return data;
}
}