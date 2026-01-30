using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class CharDataManager : MonoBehaviour
{
    private string savePath;

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
        // Najde všechny objekty ve scéně, které mají skript CharacterIdentity
        CharacterIdentity[] holders = FindObjectsOfType<CharacterIdentity>();

        // Projdeme všechny holdery a přiřadíme jim data podle pořadí
        for (int i = 0; i < holders.Length; i++)
        {
            if (i < data.characters.Count)
            {
                holders[i].Setup(data.characters[i]);
            }
            else
            {
                UnityEngine.Debug.LogWarning("Máš víc Holderů ve scéně než postav v JSONu!");
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
}