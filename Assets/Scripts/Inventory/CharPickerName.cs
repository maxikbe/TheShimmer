using UnityEngine;
using TMPro; 
using System.IO;
using System.Collections.Generic; 

public class CharPickerName : MonoBehaviour
{
    [SerializeField] private TMP_Text nazevHrace;
    [SerializeField] public int characterId; 
    private string savePath;

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "Data.json");
        if (nazevHrace == null) nazevHrace = GetComponent<TMP_Text>();
    }

    void Start()
    {
        GameData data = LoadData();

        if (data != null && data.characters != null)
        {
            Character nalezenaPostava = data.characters.Find(c => c.id == characterId);

            if (nalezenaPostava != null)
            {
                nazevHrace.text = nalezenaPostava.name;
            }
            else
            {
                nazevHrace.text = "ID " + characterId + " nenalezeno";
            }
        }
        else
        {
            nazevHrace.text = "Soubor nenalezen";
        }
    }

    public GameData LoadData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            return JsonUtility.FromJson<GameData>(json);
        }
        return null;
    }
}