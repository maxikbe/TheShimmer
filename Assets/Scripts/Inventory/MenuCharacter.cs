using UnityEngine;
using UnityEngine.UI; 
using TMPro; 
using System.IO;
using System.Collections.Generic;


public class MenuCharacter : MonoBehaviour
{
    public int characterId; 
    CharPicker charPicker;
    private string savePath;
    private GameData loadedData;
    private int lastProcessedId = -1;
    [SerializeField] private GameObject characterMenuUI;
    [SerializeField] private TMP_Text nazevHrace;
    [SerializeField] private Sprite[] seznamPostav;
    [SerializeField] private Image uiImageDisplay;

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "Data.json");
        LoadDataIntoMemory();
    }

    void Start()
    {
        if (charPicker == null)
        {
            charPicker = FindFirstObjectByType<CharPicker>(FindObjectsInactive.Include);
        }
    }

    void Update()
    {
        Debug.Log(charPicker);
        if (characterMenuUI.activeSelf && charPicker != null)
        {
            characterId = charPicker.currentIndex;
            if (characterId != lastProcessedId)
            {
                UpdateCharacterUI();
                lastProcessedId = characterId;
            }
        }
    }

    

    private void UpdateCharacterUI()
    {
        if (loadedData != null && loadedData.characters != null)
        {
            Character postava = loadedData.characters.Find(c => c.id == characterId + 1);
            
            if (postava != null)
            {
                nazevHrace.text = postava.name;

                if (characterId >= 0 && characterId < seznamPostav.Length)
                {
                    uiImageDisplay.sprite = seznamPostav[characterId];
                }
            }
        }
    }

    private void LoadDataIntoMemory()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            loadedData = JsonUtility.FromJson<GameData>(json);
        }
    }



}
