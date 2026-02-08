using UnityEngine;
using TMPro; 
using System.IO;

public class CharpickerStatsHolder : MonoBehaviour
{
    [SerializeField] private TMP_Text nazevHrace;
    [SerializeField] private TMP_Text stat1, stat2, stat3, stat4, stat5;
    
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

      Character postava = loadedData.characters.Find(c => c.id == currentId + 1);

        if (postava != null)
        {
            if (nazevHrace != null) nazevHrace.text = postava.name;
            if (stat1 != null) stat1.text = "HP: " + postava.health;
            if (stat2 != null) stat2.text = "LVL: " + postava.level;
            if (stat3 != null) stat3.text = "Speed: " + postava.speed;
        }
    }
}