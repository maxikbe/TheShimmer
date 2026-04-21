using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class gameDataManager : MonoBehaviour
{
    public static GameData currentGameData;
    private static string savePath;

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "Data.json");

        if (currentGameData == null)
        {
            LoadData();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void LoadData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            currentGameData = JsonUtility.FromJson<GameData>(json);
            Debug.Log("Data načtena z JSONu.");
        }
        else
        {
            Debug.Log("JSON nenalezen, čekám na inicializaci...");
        }
    }

    public static void SaveData()
    {
        if (currentGameData != null)
        {
            // pred zapisem questmanager aktualizuje data
            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.SaveQuestsToData();
            }

            string json = JsonUtility.ToJson(currentGameData, true);
            File.WriteAllText(savePath, json);
            Debug.Log("Data uložena do JSONu.");
        }
    }
}