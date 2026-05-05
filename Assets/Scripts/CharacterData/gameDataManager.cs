using UnityEngine;
using System.IO;
using System;

public class gameDataManager : MonoBehaviour
{
    public static GameData currentGameData;
    private static string savePath;
    public static string userDefaultName = "Data";

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

    public static void LoadData()
    {
        string path = Path.Combine(Application.persistentDataPath, userDefaultName + ".json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            currentGameData = JsonUtility.FromJson<GameData>(json);
            
            ApplySettingsToGame();
            Debug.Log("Data a nastavení načtena a aplikována.");
        }
        else
        {
            Debug.Log("JSON nenalezen, čekám na inicializaci přes InitializeGameJson...");
        }
    }

    public static void SaveData(string fileName = null, bool autoSave = false)
    {
        if (currentGameData != null)
        {
            CaptureCurrentSettings();

            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.SaveQuestsToData();
            }

            string json = JsonUtility.ToJson(currentGameData, true);
            string finalName;

            if (string.IsNullOrEmpty(fileName))
            {
                string date = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
                finalName = autoSave ? $"AutoSave[{date}]_{userDefaultName}.json" : "Data.json";
            }
            else
            {
                finalName = $"{fileName}.json";
            }

            string path = Path.Combine(Application.persistentDataPath, finalName);
            File.WriteAllText(path, json);
            Debug.Log("Uloženo do: " + path);
        }
    }

    public static void ApplySettingsToGame()
    {
        var s = currentGameData.settings;
        if (s == null) return;

        // Herní nastavení
        GameSettings.autoSave = s.autoSave;
        GameSettings.autoSaveTime = s.autoSaveTime;
        GameSettings.currentDifficulty = s.currentDifficulty;
        GameSettings.masterVolume = s.masterVolume;
        GameSettings.musicVolume = s.musicVolume;
        GameSettings.sfxVolume = s.sfxVolume;
        GameSettings.currentLanguage = s.currentLanguage;
        GameSettings.needToEat = s.needToEat;
        GameSettings.needToDrink = s.needToDrink;
        GameSettings.needToSleep = s.needToSleep;
        GameSettings.staminaEnabled = s.staminaEnabled;
        GameSettings.inventoryKapacityEnabled = s.inventoryKapacityEnabled;
        GameSettings.inventoryKapacity = s.inventoryKapacity;
        GameSettings.ambientVolume = s.ambientVolume;
        GameSettings.ambientVolumeEnabled = s.ambientVolumeEnabled;
        GameSettings.sfxVolumeEnabled = s.sfxVolumeEnabled;
        GameSettings.musicVolumeEnabled = s.musicVolumeEnabled;

        // Klávesnice
        KeyBoardSetting.keyUp = s.keyUp;
        KeyBoardSetting.keyDown = s.keyDown;
        KeyBoardSetting.keyLeft = s.keyLeft;
        KeyBoardSetting.keyRight = s.keyRight;
        KeyBoardSetting.keyRun = s.keyRun;
        KeyBoardSetting.jump = s.jump;
        KeyBoardSetting.dodge = s.dodge;
        KeyBoardSetting.parry = s.parry;
        KeyBoardSetting.doAccept = s.doAccept;
        KeyBoardSetting.doBack = s.doBack;
    }

    public static void CaptureCurrentSettings()
    {
        if (currentGameData.settings == null) currentGameData.settings = new SettingsSaver();
        var s = currentGameData.settings;

        s.autoSave = GameSettings.autoSave;
        s.autoSaveTime = GameSettings.autoSaveTime;
        s.currentDifficulty = GameSettings.currentDifficulty;
        s.masterVolume = GameSettings.masterVolume;
        s.currentLanguage = GameSettings.currentLanguage;

        s.keyUp = KeyBoardSetting.keyUp;
        s.keyDown = KeyBoardSetting.keyDown;
        s.keyLeft = KeyBoardSetting.keyLeft;
        s.keyRight = KeyBoardSetting.keyRight;
        s.jump = KeyBoardSetting.jump;
        s.dodge = KeyBoardSetting.dodge;
        s.parry = KeyBoardSetting.parry;
    }
}