using UnityEngine;
using System.IO;

public class itemsData : MonoBehaviour
{
    private string savePath;
    public GameData data;

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "ItemsData.json");
    }

    void Start()
    {
        
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

    }

    public GameData SaveInitialData()
    {
        GameData data = new GameData();

        data.OwnedItems.Add(new Item { id = 1, isOwned = false });
        data.OwnedItems.Add(new Item { id = 2, isOwned = false });
        data.OwnedItems.Add(new Item { id = 3, isOwned = false });
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        
        UnityEngine.Debug.Log("Data uložena a připravena.");
        Debug.Log(json);
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
