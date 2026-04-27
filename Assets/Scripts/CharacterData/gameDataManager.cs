using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

public class gameDataManager : MonoBehaviour
{
    public static GameData currentGameData;
    private static string savePath;
    private static string userDefaultName = "Data";

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

        InvokeRepeating(nameof(AutoSaveWrapper), GameSettings.autoSaveTime, GameSettings.autoSaveTime);
    }

    private void AutoSaveWrapper()
    {
        SaveData(null, true);
    }

    public void UserSave(string customName)
    {
        SaveData(customName, false);
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

    public static void SaveData(string fileName = null, bool autoSave = false)
    {
        if (currentGameData != null)
        {
            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.SaveQuestsToData();
            }

            string json = JsonUtility.ToJson(currentGameData, true);
            
            string finalName;
            if (string.IsNullOrEmpty(fileName))
            {
                if (autoSave)
                {
                    string date = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
                    finalName = $"AutoSave[{date}]_{userDefaultName}.json";
                } 
                else
                {
                    string date = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
                    finalName = $"{userDefaultName}_{date}.json";
                }
            }
            else
            {
                finalName = $"{fileName}.json";
            }

            savePath = Path.Combine(Application.persistentDataPath, finalName);
            File.WriteAllText(savePath, json);
            Debug.Log("Data uložena do: " + savePath);
        }
    }
}