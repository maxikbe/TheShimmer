using UnityEngine;
using TMPro; 
using System.IO;

public class CharpickerStatsHolder : MonoBehaviour
{
    [SerializeField] private TMP_Text nazevHrace;
    [SerializeField] private TMP_Text[] statList;
    
    private string savePath;
    private GameData loadedData;

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "Data.json");
        LoadDataIntoMemory();
    }

    private void LoadDataIntoMemory()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            loadedData = JsonUtility.FromJson<GameData>(json);
        }
    }

    public void UpdateStats(int currentId)
    {
        LoadDataIntoMemory();

        if (loadedData == null || loadedData.characters == null) return;
        bool isCzech = gameDataManager.currentGameData.settings.currentLanguage != 0;
        Character character = loadedData.characters.Find(c => c.id == currentId + 1);

        if (character != null)
        {
            if (nazevHrace != null) nazevHrace.text = (isCzech ? "Aktuální postava: " : "Current Character choosed: ") + character.name;
            for (int i = 0; i < statList.Length; i++)
            {
                if (statList[i] != null)
                {
                    switch (i)
                    {
                        case 0:
                            statList[i].text = (isCzech ? "Vaši obecné informace: " : "Your general info: ");
                            break;
                        case 1:
                            statList[i].text = (isCzech ? "Vylepšení zbraně: " : "Gun Upgraders: ") + gameDataManager.currentGameData.player.numberOfGunUpgraders;
                            break;
                        case 2:
                            statList[i].text = (isCzech ? "Materiály: " : "Materials: ") + gameDataManager.currentGameData.player.numberOfMaterial;
                            break;
                        case 3:
                            statList[i].text = (isCzech ? "Mince: " : "Coins: ") + gameDataManager.currentGameData.player.numberOfCoins;
                            break;
                        case 4:
                            statList[i].text = (isCzech ? "Žízeň: " : "Thirst: ") + gameDataManager.currentGameData.player.thirstLevel;
                            break;
                        case 5:
                            statList[i].text = (isCzech ? "Hlad: " : "Hunger: ") + gameDataManager.currentGameData.player.hungerLevel;
                            break;
                        case 6:
                            statList[i].text = (isCzech ? "Výdrž: " : "Stamina: ") + gameDataManager.currentGameData.player.staminaLevel;
                            break;
                        case 7:
                            statList[i].text = (isCzech ? "Spánek: " : "Sleep: ") + gameDataManager.currentGameData.player.sleepLevel;
                            break;
                    }
                }
            }
        }
    }
}