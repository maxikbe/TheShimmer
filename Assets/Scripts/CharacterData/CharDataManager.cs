using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class CharDataManager : MonoBehaviour
{
    private string savePath;
    public GameData currentData;
    public CharpickerStatsHolder statsUI;

    private int currentCharacterID = 0;
    private int adder = 0;

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "Data.json");
    }

    void Start()
    {
        GameData data;

        if (!File.Exists(savePath))
        {
            //UnityEngine.Debug.Log("První spuštění: Vytvářím základní JSON data.");
            data = InitializeGameJson.SaveInitialData(); 
        }
        else
        {
            //UnityEngine.Debug.Log("Soubor nalezen, načítám postavy...");
            data = LoadData();
        }

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
                //UnityEngine.Debug.LogWarning("Nebyla nalezena postava s ID " + i + "!");
            }
        }
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

     public void Adder(int id, int amount, int whatToAdd)
    {

        Debug.Log($"Přidávám {amount} {whatToAdd} postavě s ID {id}.");

        if (currentData == null || currentData.characters.Count == 0)
        {
            currentData = LoadData();
        }
        
        Character targetChar = currentData.characters.Find(c => c.id == id);

        if (targetChar != null)
        {
            switch(whatToAdd)
            {
                case 0:
                    targetChar.health += amount;
                    break;
                default:
                    Debug.LogWarning("Neznámý atribut k přidání.");
                    return;
            }

            UpdateCharacter(targetChar.id, targetChar.name, targetChar.health, targetChar.level);
            
            //UnityEngine.Debug.Log($"Zdraví postavy {targetChar.name} (ID: {id}) zvýšeno o {amount}. Celkem: {targetChar.health}");
        }
        else
        {
            //UnityEngine.Debug.LogError($"Nelze přidat zdraví. Postava s ID {id} neexistuje.");
        }
    }

    public void TestAddHealth()
    {
        Adder(2, 50, 0);
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
        if (currentData == null || currentData.characters.Count == 0)
        {
            currentData = LoadData();
        }

        Character targetChar = currentData.characters.Find(c => c.id == id);

        if (targetChar != null)
        {
            targetChar.name = newName;
            targetChar.health = newHealth;
            targetChar.level = newLevel;

            SaveCurrentData();
            //UnityEngine.Debug.Log($"Postava ID {id} byla aktualizována a uložena.");
            
            DistributeDataToHolders(currentData);
            if (statsUI != null)
            {
                statsUI.UpdateStats(id); 
            }
        }
        else
        {
            //UnityEngine.Debug.LogError($"Postava s ID {id} nebyla v JSONu nalezena!");
        }
    }

   
}