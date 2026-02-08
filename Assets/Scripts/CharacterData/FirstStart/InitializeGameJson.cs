using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class InitializeGameJson : MonoBehaviour
{
    [SerializeField] private Database _databaseReference; // Sem v Unity přetáhni svůj Database asset

    private static string savePath;
    private static Database itemDatabase;

    void Awake()
    {
        // 1. Nastavíme cestu
        savePath = Path.Combine(Application.persistentDataPath, "Data.json");
        
        // 2. Propojíme statickou referenci s tou z inspektoru
        itemDatabase = _databaseReference;

        // 3. Kontrola existence souboru
        if (!File.Exists(savePath))
        {
            Debug.Log("Soubor Data.json nenalezen. Provádím první inicializaci...");
            SaveInitialData();
        }
        else
        {
            Debug.Log("Data.json již existuje. Inicializace není potřeba.");
        }
    }

    public static GameData SaveInitialData()
    {
        if (itemDatabase == null)
        {
            Debug.LogError("Chyba: ItemDatabase není přiřazena v InitializeGameJson!");
            return null;
        }

        GameData data = new GameData();

        // Přidání postav
        data.characters.Add(new Character { id = 1, name = "Dr. Ventress", health = 150, level = 1, speed = 5.0f });
        data.characters.Add(new Character { id = 2, name = "Lena", health = 80, level = 1, speed = 4.5f });
        data.characters.Add(new Character { id = 3, name = "Cass Sheppard", health = 100, level = 1, speed = 7.0f });
        data.characters.Add(new Character { id = 4, name = "Josie Radek", health = 90, level = 1, speed = 8.0f });
        data.characters.Add(new Character { id = 5, name = "Anya Thorensen", health = 200, level = 1, speed = 3.0f });

        // Přidání itemů z Database
        List<Item> allItemsFromDB = itemDatabase.GetAllItems();
        foreach (Item item in allItemsFromDB)
        {
            ItemSaveData newSaveItem = new ItemSaveData();
            newSaveItem.id = item.id;
            newSaveItem.isOwned = item.isDefaultItem; 
            newSaveItem.level = item.defaultLevel;
            newSaveItem.amount = item.defaultAmount;

            data.OwnedItems.Add(newSaveItem);
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);

        Debug.Log("JSON úspěšně vytvořen na cestě: " + savePath);
        return data;
    }
}