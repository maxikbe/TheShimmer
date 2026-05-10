using UnityEngine;
using UnityEngine.UI; 
using TMPro; 
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

public class MenuCharacter : MonoBehaviour
{
    public int characterId; 
    CharPicker charPicker;
    private static Database itemDatabase;
    private static SkillDatabase skillDatabase;
    [SerializeField] private Database _databaseReference; 
    [SerializeField] private SkillDatabase _skillDatabaseReference;
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
    bool isCzech;

    [SerializeField] private GameObject skillNodePrefab;
    [SerializeField] private Transform skillTreeContent; 
    [SerializeField] private TMP_Text xpText;
    [SerializeField] private TMP_Text[] skillInfoTexts;
    private Skills[] allSkills;
    private int skillCharacterID;
    private List<SkillSaveData> savedSkills;


    void Awake()
    {
        // Pokud data v manageru ještě nejsou, zkusíme je načíst
        if (gameDataManager.currentGameData == null)
        {
            gameDataManager.LoadData();
        }
        itemDatabase = _databaseReference;
        skillDatabase = _skillDatabaseReference;
        savedSkills = gameDataManager.currentGameData.Skills;
    }

    void Start()
    {
        if (charPicker == null)
        {
            charPicker = FindFirstObjectByType<CharPicker>(FindObjectsInactive.Include);
        }
        isCzech = gameDataManager.currentGameData.settings.currentLanguage != 0;
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
        charText[0].text = (isCzech ? "Jméno postavy: " : "Char Name: ") + character.name;
        charText[1].text = (isCzech ? "Level: " : "Level: ") + character.level.ToString();
        charText[2].text = "HP: " + character.health.ToString();
        charText[3].text = (isCzech ? "Rychlost: " : "Speed: ") + character.speed.ToString();
        charText[4].text = (isCzech ? "Aktuální zkušenosti: " : "Current Experience: ") + character.currentEXP.ToString() + "/ 100";
        charText[5].text = (isCzech ? "Max HP: " : "Max HP: ") + character.maxHealth.ToString();
        charText[6].text = (isCzech ? "Mana: " : "Mana: ") + character.mana.ToString();
        charText[7].text = "XP: " + character.ExperiencePoints.ToString();
        charText[8].text = (isCzech ? "Šance na krit: " : "Crit Chance: ") + character.critChance.ToString() + "%";
        charText[9].text = (isCzech ? "Vylepšení zbraně: " : "Gun Upgraders: ") + gameDataManager.currentGameData.player.numberOfGunUpgraders.ToString();
        charText[10].text = (isCzech ? "Materiály: " : "Materials: ") + gameDataManager.currentGameData.player.numberOfMaterial.ToString();
        charText[11].text = (isCzech ? "Mince: " : "Coins: ") + gameDataManager.currentGameData.player.numberOfCoins.ToString();
        charText[12].text = (isCzech ? "Žízeň: " : "Thirst: ") + gameDataManager.currentGameData.player.thirstLevel.ToString();
        charText[13].text = (isCzech ? "Hlad: " : "Hunger: ") + gameDataManager.currentGameData.player.hungerLevel.ToString();
        charText[14].text = (isCzech ? "Výdrž: " : "Stamina: ") + gameDataManager.currentGameData.player.staminaLevel.ToString();
        charText[15].text = (isCzech ? "Spánek: " : "Sleep: ") + gameDataManager.currentGameData.player.sleepLevel.ToString();
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
                BuildTree(); 

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
        gunChooseText[0].text = (isCzech ? "Název zbraně: " : "Weapon Name: ") + itemInfo.itemName;
        gunChooseText[1].text = (isCzech ? "Typ zbraně: " : "Weapon Type: ") + itemInfo.weaponType;
        gunChooseText[2].text = (isCzech ? "Poškození: " : "Damage: ") + itemInfo.Damage;
        gunChooseText[3].text = (isCzech ? "Popis: " : "Description: ") + itemInfo.description;
        gunChooseText[4].text = (isCzech ? "Úroveň: " : "Level: ") + itemSaveData.level;
        gunChooseText[5].text = (isCzech ? "Množství: " : "Amount: ") + itemSaveData.amount;
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
        perkChooseText[0].text = (isCzech ? "Název perku: " : "Perk Name: ") + perk.perkName;
        perkChooseText[1].text = (isCzech ? "Typ perku: " : "Perk Type: ") + perk.perkType.ToString();
        perkChooseText[2].text = (isCzech ? "Popis: " : "Description: ") + perk.description;
        perkChooseText[3].text = (isCzech ? "Úroveň: " : "Level: ") + perk.levelOfPerk;
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

    private void BuildTree()
    {
        skillCharacterID = characterId + 1;
        foreach (Transform child in skillTreeContent) Destroy(child.gameObject);

        allSkills = skillDatabase.GetAllSkills()
                            .Where(s => s.characterID == skillCharacterID)
                            .OrderBy(s => s.id)
                            .ToArray();

        Character character = gameDataManager.currentGameData.characters
                                .Find(c => c.id == skillCharacterID);
        if (character == null) return;

        xpText.text = "XP: " + character.ExperiencePoints;

        // Vypni GridLayoutGroup — pozice řídíme ručně
        GridLayoutGroup grid = skillTreeContent.GetComponent<GridLayoutGroup>();
        if (grid != null) grid.enabled = false;

        foreach (Skills skill in allSkills)
        {
            SpawnNode(skill, character);
        }
    }

    private void SpawnNode(Skills skill, Character character)
    {
        SkillSaveData saveData = savedSkills?.Find(s => s.id == skill.id);
        bool isResearched = saveData?.isResearched ?? false;
        bool isUnlockable = IsUnlockable(skill, character);

        GameObject node = Instantiate(skillNodePrefab, skillTreeContent);

        float cellW = 120f;
        float cellH = 120f;

        RectTransform rt = node.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(
            skill.gridX * cellW + cellW / 2f,
            -(skill.gridY * cellH + cellH / 2f)
        );
        rt.sizeDelta = new Vector2(cellW - 10f, cellH - 10f);

        // Icon
        Transform iconTransform = node.transform.Find("Icon");
        if (iconTransform != null)
            iconTransform.GetComponent<Image>().sprite = skill.icon;
        else
            Debug.LogWarning($"[SkillTree] 'Icon' child not found on node for skill id={skill.id}");

        // Cost text
        TMP_Text costText = node.GetComponentInChildren<TMP_Text>();
        costText.text = isResearched ? "" : skill.pointsToResearch + "";

        // Opacity
        float alpha = (isResearched || isUnlockable) ? 1f : 0.35f;
        foreach (Graphic g in node.GetComponentsInChildren<Graphic>())
            g.color = new Color(g.color.r, g.color.g, g.color.b, alpha);

        // Button
        Button btn = node.GetComponent<Button>();
        btn.transition = Selectable.Transition.None;
        btn.interactable = isUnlockable && !isResearched;
        btn.onClick.AddListener(() => ResearchSkill(skill, character));

        // Hover — zobraz info o skillu
        EventTrigger trigger = node.GetComponent<EventTrigger>() ?? node.AddComponent<EventTrigger>();

        EventTrigger.Entry onEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        onEnter.callback.AddListener(_ => ShowSkillInfo(skill, isResearched, isUnlockable, character));
        trigger.triggers.Add(onEnter);

        EventTrigger.Entry onExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        onExit.callback.AddListener(_ => ClearSkillInfo());
        trigger.triggers.Add(onExit);
    }

    private void ShowSkillInfo(Skills skill, bool isResearched, bool isUnlockable, Character character)
    {
        if (skillInfoTexts == null || skillInfoTexts.Length == 0) return;

        // skillInfoTexts[0] — název
        if (skillInfoTexts.Length > 0 && skillInfoTexts[0] != null)
            skillInfoTexts[0].text = skill.skillName;

        // skillInfoTexts[1] — cena / stav
        if (skillInfoTexts.Length > 1 && skillInfoTexts[1] != null)
        {
            if (isResearched)
                skillInfoTexts[1].text = isCzech ? "Již odemčeno" : "Already researched";
            else
                skillInfoTexts[1].text = (isCzech ? "Cena: " : "Cost: ") + skill.pointsToResearch + " XP";
        }

        // skillInfoTexts[2] — popis
        if (skillInfoTexts.Length > 2 && skillInfoTexts[2] != null)
            skillInfoTexts[2].text = skill.description;

        // skillInfoTexts[3] — dostupnost
        if (skillInfoTexts.Length > 3 && skillInfoTexts[3] != null)
        {
            if (isResearched)
                skillInfoTexts[3].text = "";
            else if (isUnlockable)
                skillInfoTexts[3].text = isCzech ? "✓ Lze odemknout" : "✓ Can be unlocked";
            else if (character.ExperiencePoints < skill.pointsToResearch)
                skillInfoTexts[3].text = (isCzech ? "✗ Nedostatek XP (máš " : "✗ Not enough XP (you have ")
                                        + character.ExperiencePoints + " XP)";
            else
                skillInfoTexts[3].text = isCzech ? "✗ Nejprve odemkni předchozí skill" : "✗ Unlock previous skill first";
        }
    }

    private void ClearSkillInfo()
    {
        if (skillInfoTexts == null) return;
        foreach (TMP_Text t in skillInfoTexts)
            if (t != null) t.text = "";
    }

    private bool IsUnlockable(Skills skill, Character character)
    {
        // Má dost XP?
        if (character.ExperiencePoints < skill.pointsToResearch) return false;

        // Jsou parent skills odemčené?
        if (skill.mustBeActivedSkillID != 0)
        {
            SkillSaveData parent = savedSkills?.Find(s => s.id == skill.mustBeActivedSkillID);
            if (parent == null || !parent.isResearched) return false;
        }

        return true;
    }

    private void ResearchSkill(Skills skill, Character character)
    {
        character.ExperiencePoints -= skill.pointsToResearch;

        SkillSaveData save = gameDataManager.currentGameData.Skills
                                .Find(s => s.id == skill.id);
        if (save == null)
        {
            gameDataManager.currentGameData.Skills.Add(
                new SkillSaveData { id = skill.id, isResearched = true }
            );
        }
        else save.isResearched = true;

        gameDataManager.SaveData();
        BuildTree();
        addCharInfo(character); 
    }


}