using UnityEngine;
using UnityEngine.UI; 
using TMPro; 
using System.IO;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using System.Linq;

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
    private int PickedPerkIndex = 0;
    [SerializeField] private GameObject perkChoosePrefabType;
    [SerializeField] private GameObject perkChooseContent;
    [SerializeField] private TMP_Text[] perkChooseText;
    [SerializeField] private GameObject[] perkPickerButtons;

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
            addPickablePerks();
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
    
    private void addPickablePerks()
    {
       Perks[] allPerksFromResources = Resources.LoadAll<Perks>("PerksData");

        foreach (Transform child in perkChooseContent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Perks perk in allPerksFromResources)
        {
            GameObject buttonObj = Instantiate(perkChoosePrefabType, perkChooseContent.transform);
            TMP_Text buttonText = buttonObj.GetComponentInChildren<TMP_Text>();
            if (buttonText != null) buttonText.text = perk.perkName;

            Image buttonImage = buttonObj.GetComponentInChildren<Image>();
            if (buttonImage != null) buttonImage.sprite = perk.icon;

            Button btn = buttonObj.GetComponent<Button>();
            if (btn != null)
            {
                int currentId = perk.id; 
                
                btn.onClick.AddListener(() => {
                    if (PickedPerkIndex == 0)
                    {
                        addPerkInfo(perk);
                    }
                    else 
                    {
                        PickPerk(currentId);
                        Debug.Log("Vybrán perk s ID: " + currentId + " do slotu: " + PickedPerkIndex);
                    }
                });
            }
        }
    }

    private void addPerkInfo(Perks perk)
    {
        if (perkChooseText.Length >= 3)
        {
            perkChooseText[0].text = "Name: " + perk.perkName;
            perkChooseText[1].text = "Type: " + perk.perkType.ToString();
            perkChooseText[2].text = "Description: " + perk.description;
        }
    }
    
    public void PickedIndexSetter(int index)
    {
        // 1. Nejdříve resetujeme všechna tlačítka na bílou (aby nezůstalo černé to minulé)
        foreach (GameObject btn in perkPickerButtons)
        {
            if (btn == null) continue;
            Graphic[] allGraphics = btn.GetComponentsInChildren<Graphic>();
            foreach (Graphic g in allGraphics)
            {
                g.color = Color.white;
            }
        }

        // 2. Logika přepínání indexu
        if (index != PickedPerkIndex) 
        {
            PickedPerkIndex = index;
            
            // 3. Zčernáme děti POUZE u aktivního tlačítka
            // index - 1 protože v Unity voláš Button 1, 2, 3, ale pole je 0, 1, 2
            int arrayIndex = index - 1; 

            if (arrayIndex >= 0 && arrayIndex < perkPickerButtons.Length)
            {
                GameObject targetButton = perkPickerButtons[arrayIndex];
                Graphic[] childrenGraphics = targetButton.GetComponentsInChildren<Graphic>();

                foreach (Graphic g in childrenGraphics)
                {
                    g.color = Color.black;
                }
            }
        }
        else 
        {
            // Pokud klikneš na stejné tlačítko podruhé, zruší se výběr (vše už je bílé z kroku 1)
            PickedPerkIndex = 0;
        }

        Debug.Log("Aktivní slot pro perk: " + PickedPerkIndex);
    }


    public void PickPerk(int idOfPerk)
    {
        Perks[] allPerksFromResources = Resources.LoadAll<Perks>("PerksData");
        if (loadedData != null && loadedData.characters != null)
        {
            Character postava = loadedData.characters.Find(c => c.id == characterId + 1);
            if (postava != null)
            {
                Perks selectedPerk = allPerksFromResources.FirstOrDefault(p => p.id == idOfPerk);

                if (selectedPerk != null) 
                {
                    // Pomocná proměnná pro tlačítko, se kterým zrovna pracujeme
                    GameObject currentButton = null;

                    switch(PickedPerkIndex)
                    {
                        case 1: 
                            postava.pickePerkID1 = idOfPerk;
                            currentButton = perkPickerButtons[0];
                            break;
                        case 2:
                            postava.pickePerkID2 = idOfPerk;
                            currentButton = perkPickerButtons[1];
                            break;
                        case 3:
                            postava.pickePerkID3 = idOfPerk;
                            currentButton = perkPickerButtons[2];
                            break;
                    }

                    if (currentButton != null)
                    {
                        // Najdeme objekt "Child" pod tlačítkem a změníme mu Sprite
                        Transform childTransform = currentButton.transform.Find("Child");
                        if (childTransform != null)
                        {
                            childTransform.GetComponent<Image>().sprite = selectedPerk.icon;
                        }
                        else
                        {
                            Debug.LogError("Objekt jménem 'Child' nebyl pod tlačítkem nalezen!");
                        }
                        
                        PickedPerkIndex = 0;
                    }
                }
            }
        }
    }
}
