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

    // 1. Definice postav
    data.characters.Add(new Character { id = 1, name = "Dr. Ventress", health = 150, level = 1, speed = 5.0f });
    data.characters.Add(new Character { id = 2, name = "Lena", health = 80, level = 1, speed = 4.5f });
    data.characters.Add(new Character { id = 3, name = "Cass Sheppard", health = 100, level = 1, speed = 7.0f });
    data.characters.Add(new Character { id = 4, name = "Josie Radek", health = 90, level = 1, speed = 8.0f });
    data.characters.Add(new Character { id = 5, name = "Anya Thorensen", health = 200, level = 1, speed = 3.0f });

    List<Item> allItemsFromDB = itemDatabase.GetAllItems();

    foreach (var character in data.characters)
    {
        foreach (var item in allItemsFromDB)
        {
            if (item.allowedCharacterIDs == null || item.allowedCharacterIDs.Count == 0 || item.allowedCharacterIDs.Contains(character.id))
            {
                character.usableItemIDs.Add(item.id);
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

        if (item.allowedCharacterIDs == null || item.allowedCharacterIDs.Count == 0)
        {
            foreach (var c in data.characters) newSaveItem.allowedCharacterIDs.Add(c.id);
        }
        else
        {
            newSaveItem.allowedCharacterIDs = new List<int>(item.allowedCharacterIDs);
        }

        data.OwnedItems.Add(newSaveItem);
    }

    string json = JsonUtility.ToJson(data, true);
    File.WriteAllText(savePath, json);

    Debug.Log("JSON inicializován: Postavy nyní vědí, které předměty mohou používat.");
    return data;
}
}