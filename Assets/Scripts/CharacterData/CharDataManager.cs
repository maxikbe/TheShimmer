using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class CharDataManager : MonoBehaviour
{
    private string savePath;
    public GameData currentData;
    public CharpickerStatsHolder statsUI;

    private int currentCharacterID = 0;
    private int adder = 0;

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "CharData.json");
    }

    void Start()
    {
        GameData data;

        if (!File.Exists(savePath))
        {
            UnityEngine.Debug.Log("První spuštění: Vytvářím základní JSON data.");
            data = SaveInitialData(); 
        }
        else
        {
            UnityEngine.Debug.Log("Soubor nalezen, načítám postavy...");
            data = LoadData();
        }

        // TADY SE TO PROPOJUJE:
        if (data != null)
        {
            DistributeDataToHolders(data);
        }
    }

    private void DistributeDataToHolders(GameData data)
    {
        CharacterIdentity[] holders = FindObjectsOfType<CharacterIdentity>();

        for (int i = 0; i < holders.Length; i++)
        {
            if (i < data.characters.Count)
            {
                holders[i].Setup(data.characters[i]);
            }
            else
            {
                UnityEngine.Debug.LogWarning("Nebyla nalezena postava s ID " + i + "!");
            }
        }
    }

    public GameData SaveInitialData()
    {
        GameData data = new GameData();

        data.characters.Add(new Character { id = 1, name = "Hráč 1", health = 150, level = 1, speed = 5.0f });
        data.characters.Add(new Character { id = 2, name = "Hráč 2", health = 80, level = 1, speed = 4.5f });
        data.characters.Add(new Character { id = 3, name = "Hráč 3", health = 100, level = 1, speed = 7.0f });
        data.characters.Add(new Character { id = 4, name = "Hráč 4", health = 90, level = 1, speed = 8.0f });
        data.characters.Add(new Character { id = 5, name = "Hráč 5", health = 200, level = 1, speed = 3.0f });
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        
        UnityEngine.Debug.Log("Data uložena a připravena.");
        return data;
    }

    public GameData LoadData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            GameData data = JsonUtility.FromJson<GameData>(json);
            return data;
        }
        return null;
    }

     public void AddHealth(int id, int amount)
    {
        Debug.Log($"Přidávám {amount} zdraví postavě s ID {id}.");
        if (currentData == null || currentData.characters.Count == 0)
        {
            currentData = LoadData();
        }
        
        Character targetChar = currentData.characters.Find(c => c.id == id);

        if (targetChar != null)
        {
            int updatedHealth = targetChar.health + amount;
            UpdateCharacter(targetChar.id, targetChar.name, updatedHealth, targetChar.level);
            
            UnityEngine.Debug.Log($"Zdraví postavy {targetChar.name} (ID: {id}) zvýšeno o {amount}. Celkem: {targetChar.health}");
        }
        else
        {
            UnityEngine.Debug.LogError($"Nelze přidat zdraví. Postava s ID {id} neexistuje.");
        }
    }

    public void TestAddHealth()
    {
        AddHealth(2, 50);
    }

    //FindObjectOfType<CharDataManager>().AddHealth(2, 50);

    public void SaveCurrentData()
    {
        string json = JsonUtility.ToJson(currentData, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Data z editoru uložena do: " + savePath);
    }

    public void UpdateCharacter(int id, string newName, int newHealth, int newLevel)
    {
        // 1. Nejdříve zajistíme, že máme aktuální data v paměti (pokud nebyla načtena)
        if (currentData == null || currentData.characters.Count == 0)
        {
            currentData = LoadData();
        }

        // 2. Najdeme postavu podle ID
        Character targetChar = currentData.characters.Find(c => c.id == id);

        if (targetChar != null)
        {
            // 3. Změníme data
            targetChar.name = newName;
            targetChar.health = newHealth;
            targetChar.level = newLevel;

            // 4. Uložíme aktualizovaný seznam zpět do JSONu
            SaveCurrentData();
            UnityEngine.Debug.Log($"Postava ID {id} byla aktualizována a uložena.");
            
            // 5. Volitelné: Pokud chceš, aby se změna hned projevila ve hře
            DistributeDataToHolders(currentData);
            if (statsUI != null)
            {
                statsUI.UpdateStats(id); 
            }
        }
        else
        {
            UnityEngine.Debug.LogError($"Postava s ID {id} nebyla v JSONu nalezena!");
        }
    }

   
}