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
        // Kontrola, jestli jsou data načtená
        if (loadedData == null) LoadDataIntoMemory();
        if (loadedData == null || loadedData.characters == null) return;

        // Kontrola, jestli index není mimo rozsah seznamu
        if (currentId >= 0 && currentId < loadedData.characters.Count)
        {
            Character postava = loadedData.characters[currentId];

            // Kontrola, jestli jsou Texty přiřazené v Inspectoru, aby to neházelo NullReference
            if (nazevHrace != null) nazevHrace.text = postava.name;
            if (stat1 != null) stat1.text = "HP: " + postava.health;
            if (stat2 != null) stat2.text = "LVL: " + postava.level;
            if (stat3 != null) stat3.text = "Speed: " + postava.speed;
            // stat4, stat5...
        }
    }
}