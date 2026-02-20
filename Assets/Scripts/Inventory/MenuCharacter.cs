using UnityEngine;
using UnityEngine.UI; 
using TMPro; 
using System.IO;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;


public class MenuCharacter : MonoBehaviour
{
    public int characterId; 
    CharPicker charPicker;
    private string savePath;
    private static Database itemDatabase;
    [SerializeField] private Database _databaseReference; 
    private GameData loadedData;
    private int lastProcessedId = -1;
    [SerializeField] private GameObject characterMenuUI;
    [SerializeField] private TMP_Text nazevHrace;
    [SerializeField] private Sprite[] seznamPostav;
    [SerializeField] private Image uiImageDisplay;
    [SerializeField] private GameObject GunchoosePrefabType;
    [SerializeField] private GameObject GunChooseConent;
    [SerializeField] private TMP_Text[] gunChooseText;

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "Data.json");
        LoadDataIntoMemory();
        itemDatabase = _databaseReference;
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
            addPickableButtons();
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
    
    private void addPickableButtons()
    {
        Character postava = loadedData.characters.Find(c => c.id == characterId + 1);
        if (postava != null)
        {
            foreach (int itemId in postava.pickableTurnBaseItemIDs)
            {
                ItemSaveData item = loadedData.OwnedItems.Find(i => i.id == itemId);
                Item itemInfo = FindItemInDatabase(itemId);
                if (item != null)
                {
                    GameObject buttonObj = Instantiate(GunchoosePrefabType, GunChooseConent.transform);
                    TMP_Text buttonText = buttonObj.GetComponentInChildren<TMP_Text>();
                    if (buttonText != null)
                    {
                        buttonText.text = itemInfo.name;
                    }
                    addGunInfo(itemInfo, item);
                }
            }
        }   
    }

    private Item FindItemInDatabase(int itemId)
    {
        if (itemDatabase != null)
        {
            return itemDatabase.GetItemByID(itemId);
        }
        return null;
    }

    private void addGunInfo(Item itemInfo, ItemSaveData itemSaveData)
    {
        gunChooseText[0].text = "Level: " + itemSaveData.level;
        gunChooseText[1].text = "Amount: " + itemSaveData.amount;
        gunChooseText[2].text = "Damage: " + itemInfo.Damage;
        gunChooseText[3].text = "Fire Rate: " + itemInfo.weaponType; 

    }
    



}
