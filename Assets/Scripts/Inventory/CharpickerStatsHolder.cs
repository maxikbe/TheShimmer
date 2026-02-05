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
        savePath = Path.Combine(Application.persistentDataPath, "CharData.json");
        // Načteme data z JSONu hned při startu do paměti
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

    // Tuto metodu zavolá CharPicker a pošle jí index postavy
    public void UpdateStats(int currentId)
    {
        // 1. Znovu načteme soubor z disku, aby tam byly nové změny (třeba po AddHealth)
        LoadDataIntoMemory();

        if (loadedData == null || loadedData.characters == null) return;

        // 2. Najdeme postavu (lepší je hledat podle ID, ne podle indexu v poli, pokud se pole mění)
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