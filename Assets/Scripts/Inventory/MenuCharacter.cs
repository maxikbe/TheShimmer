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
    [SerializeField] private Image perkChooseIcon;
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

            perkChooseText[0].text = perk.perkName;
            perkChooseText[1].text = perk.perkType.ToString();
            perkChooseText[2].text = perk.description;
            perkChooseIcon.sprite = perk.icon;
        }
    }
    
    public void PickedIndexSetter(int index)
    {
        Color targetBlack = new Color(255f, 255f, 255f, 0.8f);

        foreach (GameObject btn in perkPickerButtons)
        {
            if (btn == null) continue;
            foreach (Graphic g in btn.GetComponentsInChildren<Graphic>())
            {
                g.color = Color.white;
            }
        }

        if (index != PickedPerkIndex) 
        {
            PickedPerkIndex = index;
            int arrayIndex = index - 1; 

            if (arrayIndex >= 0 && arrayIndex < perkPickerButtons.Length)
            {
                GameObject targetButton = perkPickerButtons[arrayIndex];
                foreach (Graphic g in targetButton.GetComponentsInChildren<Graphic>())
                {
                    g.color = targetBlack;
                }
            }
        }
        else 
        {
            PickedPerkIndex = 0;
        }
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
                    int oldPerkId = 0;
                    switch(PickedPerkIndex)
                    {
                        case 1: oldPerkId = postava.pickePerkID1; break;
                        case 2: oldPerkId = postava.pickePerkID2; break;
                        case 3: oldPerkId = postava.pickePerkID3; break;
                    }

                    Character ownerOfNewPerk = null;
                    int sourceSlot = 0;

                    foreach (Character c in loadedData.characters)
                    {
                        if (c.pickePerkID1 == idOfPerk) { ownerOfNewPerk = c; sourceSlot = 1; break; }
                        if (c.pickePerkID2 == idOfPerk) { ownerOfNewPerk = c; sourceSlot = 2; break; }
                        if (c.pickePerkID3 == idOfPerk) { ownerOfNewPerk = c; sourceSlot = 3; break; }
                    }

                    if (ownerOfNewPerk != null)
                    {
                        switch(sourceSlot)
                        {
                            case 1: ownerOfNewPerk.pickePerkID1 = oldPerkId; break;
                            case 2: ownerOfNewPerk.pickePerkID2 = oldPerkId; break;
                            case 3: ownerOfNewPerk.pickePerkID3 = oldPerkId; break;
                        }
                        
                        if (ownerOfNewPerk.id == postava.id)
                        {
                            UpdateSlotUI(sourceSlot, oldPerkId, allPerksFromResources);
                        }
                    }

                    switch(PickedPerkIndex)
                    {
                        case 1: postava.pickePerkID1 = idOfPerk; break;
                        case 2: postava.pickePerkID2 = idOfPerk; break;
                        case 3: postava.pickePerkID3 = idOfPerk; break;
                    }

                    UpdateSlotUI(PickedPerkIndex, idOfPerk, allPerksFromResources);

                    foreach (GameObject btn in perkPickerButtons)
                    {
                        if (btn == null) continue;
                        foreach (Graphic g in btn.GetComponentsInChildren<Graphic>())
                        {
                            g.color = Color.white;
                        }
                    }

                    PickedPerkIndex = 0; 
                }
            }
        }
    }

    private void UpdateSlotUI(int slotIndex, int perkId, Perks[] database)
    {
        if (slotIndex < 1 || slotIndex > 3) return;

        GameObject btn = perkPickerButtons[slotIndex - 1];
        if (btn == null) return;

        Transform childTransform = btn.transform.Find("Child");
        if (childTransform != null)
        {
            Image iconImage = childTransform.GetComponent<Image>();

            if (perkId == 0)
            {
                iconImage.sprite = null;
                iconImage.color = new Color(1, 1, 1, 0); 
            }
            else
            {
                Perks foundPerk = database.FirstOrDefault(p => p.id == perkId);
                if (foundPerk != null)
                {
                    iconImage.sprite = foundPerk.icon;
                    iconImage.color = Color.white; 
                }
            }
        }
    }

}
