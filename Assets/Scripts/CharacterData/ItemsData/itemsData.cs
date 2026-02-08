using UnityEngine;
using System.IO;

public class itemsData : MonoBehaviour
{
    private string savePath;
    public GameData data;

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "Data.json");
    }

    void Start()
    {
        
        if (!File.Exists(savePath))
        {
            UnityEngine.Debug.Log("První spuštění: Vytvářím základní JSON data.");
            data = InitializeGameJson.SaveInitialData(); 
        }
        else
        {
            UnityEngine.Debug.Log("Soubor nalezen, načítám itemy...");
            data = LoadData();
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
}
