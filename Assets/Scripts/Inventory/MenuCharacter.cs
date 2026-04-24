using UnityEngine;
using UnityEngine.UI; 
using TMPro; 
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class MenuCharacter : MonoBehaviour
{
    public int characterId; 
    CharPicker charPicker;
    private static Database itemDatabase;
    [SerializeField] private Database _databaseReference; 
    private GameData loadedData => gameDataManager.currentGameData;

    private int lastProcessedId = -1;
    [SerializeField] private GameObject characterMenuUI;
    [SerializeField] private TMP_Text nazevHrace;
    [SerializeField] private Sprite[] seznamPostav;
    [SerializeField] private Image uiImageDisplay;
    [SerializeField] private TMP_Text[] charText;

    [SerializeField] private GameObject GunchoosePrefabType;
    [SerializeField] private GameObject GunChooseConent;
    [SerializeField] private TMP_Text[] gunChooseText;
    private int currentGunID;
    private int PickedPerkIndex = 0;
    [SerializeField] private GameObject perkChoosePrefabType;
    [SerializeField] private GameObject perkChooseContent;
    [SerializeField] private TMP_Text[] perkChooseText;
    [SerializeField] private Image perkChooseIcon;
    [SerializeField] private GameObject[] perkPickerButtons;

    void Awake()
    {
        // Pokud data v manageru ještě nejsou, zkusíme je načíst
        if (gameDataManager.currentGameData == null)
        {
            gameDataManager.LoadData();
        }
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

    private void addCharInfo(Character character)
    {
        if (character == null) return;
        charText[0].text = character.name;
        charText[1].text = "Level: " + character.level.ToString();
        charText[2].text = "HP: " + character.health.ToString();
        charText[3].text = "Speed: " + character.speed.ToString();
        charText[4].text = "Perk Upgrade: " + character.perkUpgradersNumber.ToString();
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
                    uiImageDisplay.sprite = seznamPostav[characterId];

                addCharInfo(postava);
                addPickableButtons();
                addPickablePerks();

                // Okamžité vykreslení ikon v UI slotech postavy
                Perks[] allPerksFromResources = Resources.LoadAll<Perks>("PerksData");
                UpdateSlotUI(1, postava.pickePerkID1, allPerksFromResources);
                UpdateSlotUI(2, postava.pickePerkID2, allPerksFromResources);
                UpdateSlotUI(3, postava.pickePerkID3, allPerksFromResources);
            }
        }
    }

    public void PickGun()
    {
        if (loadedData == null) return;

        Character postava = loadedData.characters.Find(c => c.id == characterId + 1);
        if (postava != null)
        {
            postava.pickedItemID = currentGunID;
            Debug.Log($"[PickGun] Postava {postava.id} vybavena zbraní {postava.pickedItemID}");  
            addPickableButtons();
            
            
            gameDataManager.currentGameData.characters = loadedData.characters;
        }
    }
    
    private void addPickableButtons()
    {
        foreach (Transform child in GunChooseConent.transform) Destroy(child.gameObject);

        Character postava = loadedData.characters.Find(c => c.id == characterId + 1);
        if (postava == null) return;

        foreach (int itemId in postava.pickableTurnBaseItemIDs)
        {
            ItemSaveData item = loadedData.OwnedItems.Find(i => i.id == itemId);
            Item itemInfo = FindItemInDatabase(itemId);
            if (item == null || itemInfo == null) continue;

            GameObject buttonObj = Instantiate(GunchoosePrefabType, GunChooseConent.transform);
            Button btn = buttonObj.GetComponent<Button>();
            btn.transition = Selectable.Transition.None; 

            btn.onClick.AddListener(() => {
                setCurrentGunId(itemInfo.id);
            });

            LongPressHandler lph = buttonObj.GetComponent<LongPressHandler>();
            if (lph == null) lph = buttonObj.AddComponent<LongPressHandler>();
            
            int localId = itemInfo.id;
            lph.onLongPress.RemoveAllListeners();
            lph.onLongPress.AddListener(() => {
                currentGunID = localId;
                PickGun();
            });

            bool isCurrentlyPicked = (itemInfo.id == postava.pickedItemID);
            lph.SetPickedStatus(isCurrentlyPicked);

            TMP_Text buttonText = buttonObj.GetComponentInChildren<TMP_Text>();
            if (buttonText != null) buttonText.text = itemInfo.name;
            
            Transform imageTransform = buttonObj.transform.Find("Image");
            if (imageTransform != null)
            {
                Image buttonImage = imageTransform.GetComponent<Image>();
                if (buttonImage != null) buttonImage.sprite = itemInfo.icon;
            }

            if (isCurrentlyPicked && btn.transform.childCount > 0) btn.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    private void setCurrentGunId(int index)
    {
        currentGunID = index;
        Item itemInfo = FindItemInDatabase(currentGunID);
        ItemSaveData item = loadedData.OwnedItems.Find(i => i.id == index);
        addGunInfo(itemInfo, item);
    }

    private Item FindItemInDatabase(int itemId)
    {
        return itemDatabase != null ? itemDatabase.GetItemByID(itemId) : null;
    }

    private void addGunInfo(Item itemInfo, ItemSaveData itemSaveData)
    {
        if (itemInfo == null || itemSaveData == null) return;
        gunChooseText[0].text = "Level: " + itemSaveData.level;
        gunChooseText[1].text = "Amount: " + itemSaveData.amount;
        gunChooseText[2].text = "Damage: " + itemInfo.Damage;
        gunChooseText[3].text = "Fire Rate: " + itemInfo.weaponType; 
    }
    
    private void addPickablePerks()
    {
        Perks[] allPerksFromResources = Resources.LoadAll<Perks>("PerksData");

        foreach (Transform child in perkChooseContent.transform) Destroy(child.gameObject);

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
                    if (PickedPerkIndex == 0) addPerkInfo(perk);
                    else PickPerk(currentId);
                });
            }
        }
    }

    public void PickPerk(int idOfPerk)
    {
        Perks[] allPerksFromResources = Resources.LoadAll<Perks>("PerksData");
        if (loadedData == null || loadedData.characters == null) return;

        Character postava = loadedData.characters.Find(c => c.id == characterId + 1);
        if (postava == null || PickedPerkIndex == 0) return;

        Perks selectedPerk = allPerksFromResources.FirstOrDefault(p => p.id == idOfPerk);
        if (selectedPerk != null) 
        {            
            int oldPerkId = 0;
            if (PickedPerkIndex == 1) oldPerkId = postava.pickePerkID1;
            else if (PickedPerkIndex == 2) oldPerkId = postava.pickePerkID2;
            else if (PickedPerkIndex == 3) oldPerkId = postava.pickePerkID3;

            foreach (Character c in loadedData.characters)
            {
                if (c.pickePerkID1 == idOfPerk) { c.pickePerkID1 = oldPerkId; break; }
                if (c.pickePerkID2 == idOfPerk) { c.pickePerkID2 = oldPerkId; break; }
                if (c.pickePerkID3 == idOfPerk) { c.pickePerkID3 = oldPerkId; break; }
            }

            if (PickedPerkIndex == 1) postava.pickePerkID1 = idOfPerk;
            else if (PickedPerkIndex == 2) postava.pickePerkID2 = idOfPerk;
            else if (PickedPerkIndex == 3) postava.pickePerkID3 = idOfPerk;

            gameDataManager.currentGameData.characters = loadedData.characters;
        
            UpdateSlotUI(1, postava.pickePerkID1, allPerksFromResources);
            UpdateSlotUI(2, postava.pickePerkID2, allPerksFromResources);
            UpdateSlotUI(3, postava.pickePerkID3, allPerksFromResources);

            ResetPerkSelectionUI();
        }
    }

    private void ResetPerkSelectionUI()
    {
        foreach (GameObject btn in perkPickerButtons)
        {
            if (btn == null) continue;
            foreach (Graphic g in btn.GetComponentsInChildren<Graphic>()) g.color = Color.white;
        }
        PickedPerkIndex = 0;
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
            if (perkId == 0) { iconImage.sprite = null; iconImage.color = new Color(1, 1, 1, 0); }
            else
            {
                Perks foundPerk = database.FirstOrDefault(p => p.id == perkId);
                if (foundPerk != null) { iconImage.sprite = foundPerk.icon; iconImage.color = Color.white; }
            }
        }
    }

    private void addPerkInfo(Perks perk)
    {
        perkChooseText[0].text = perk.perkName;
        perkChooseText[1].text = perk.perkType.ToString();
        perkChooseText[2].text = perk.description;
        perkChooseIcon.sprite = perk.icon;
    }

    public void PickedIndexSetter(int index)
    {
        ResetPerkSelectionUI();
        if (index != PickedPerkIndex) 
        {
            PickedPerkIndex = index;
            int arrayIndex = index - 1; 
            if (arrayIndex >= 0 && arrayIndex < perkPickerButtons.Length)
            {
                foreach (Graphic g in perkPickerButtons[arrayIndex].GetComponentsInChildren<Graphic>())
                    g.color = new Color(1f, 1f, 1f, 0.8f);
            }
        }
    }
}