using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI; 
using TMPro;
using System.IO; 

public class Inventory : MonoBehaviour
{
    [SerializeField] private Database database;
    [SerializeField] private Transform container;    
    [SerializeField] private GameObject itemPrefab;  
    [SerializeField] private TextMeshProUGUI statsTitle;       
    [SerializeField] private TextMeshProUGUI statsDescription; 
    [SerializeField] private Image statsIcon;

    private GameData currentSaveData; 
    private string savePath;

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "Data.json");
    }

    void Start()
    {
        LoadAndRefresh();
        ClearStats();
    }

    public void LoadAndRefresh()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            currentSaveData = JsonUtility.FromJson<GameData>(json);
        }
        else
        {
            Debug.LogError("Save soubor nenalezen! Musíš nejdříve zavolat SaveInitialData.");
            return;
        }

        RefreshInventory(ItemType.Weapon);
    }

    public void RefreshInventory(ItemType filter)
    {
        foreach (Transform child in container) {
            Destroy(child.gameObject);
        }

        if (currentSaveData == null) return;

        foreach (ItemSaveData saveItem in currentSaveData.OwnedItems)
        {
            if (!saveItem.isOwned) continue;

            Item staticData = database.GetItemByID(saveItem.id);

            if (staticData != null && staticData.itemType == filter)
            {
                CreateItemSlot(staticData, saveItem);
            }
        }
    }

    private void CreateItemSlot(Item staticData, ItemSaveData saveItem)
    {
        GameObject slot = Instantiate(itemPrefab, container);
        
        slot.GetComponentInChildren<TextMeshProUGUI>().text = staticData.itemName;

        Transform iconTransform = slot.transform.Find("Image"); 
        if (iconTransform != null) {
            Image iconImage = iconTransform.GetComponent<Image>();
            if (staticData.icon != null) iconImage.sprite = staticData.icon;
        }

        Button btn = slot.GetComponent<Button>();
        btn.onClick.AddListener(() => ShowStats(staticData, saveItem));

        if (!staticData.isUsable) {
            btn.interactable = false;
        }
    }

    public void ShowStats(Item item, ItemSaveData saveItem)
    {
        statsTitle.text = item.itemName;
        string enumTypeName = item.itemType.ToString();
        bool isCzech = gameDataManager.currentGameData.settings.currentLanguage != 0;
    
        string dynamicInfo = $"<b>{(isCzech ? "Level" : "Level")}: {saveItem.level} | {(isCzech ? "Množství" : "Amount")}: {saveItem.amount}</b>\n\n";


        switch (item.itemType)
        {
            case ItemType.Weapon:
                statsDescription.text = dynamicInfo +
                    $"{(isCzech ? "Poškození" : "Damage")}: {item.Damage}\n" +
                    $"{(isCzech ? "Rychlost střelby" : "Fire Rate")}: {item.FireRate}\n" +
                    $"{(isCzech ? "Dosah" : "Range")}: {item.Range}\n" +
                    $"{(isCzech ? "Typ zbraně" : "Weapon Type")}: {item.weaponType}" +
                    (item.isMagical ? $"\n{(isCzech ? "Element" : "Element")}: {item.magicalElement}" : "") +
                    (item.weaponType == WeaponType.Ranged ? $"\n{(isCzech ? "Kapacita munice" : "Ammo Capacity")}: {item.AmmoCapacity}\n{(isCzech ? "Čas přebíjení" : "Reload Time")}: {item.ReloadTime}" : "");
                break;

            case ItemType.Armor:
                statsDescription.text = dynamicInfo +
                    $"{(isCzech ? "Obrana" : "Armor")}: {item.Armor}\n" +
                    $"{(isCzech ? "Odolnost" : "Durability")}: {item.durability}\n" +
                    $"{(isCzech ? "Slot" : "Slot")}: {item.armorType}";
                break;

            case ItemType.Healing:
            case ItemType.Consumable:
                statsDescription.text = dynamicInfo +
                    $"{(isCzech ? "Obnova HP" : "HP adding")}: {item.HealAmount}\n" +
                    $"{(isCzech ? "Obnova jídla" : "Food adding")}: {item.consumeAmount}\n" +
                    $"{(isCzech ? "Obnova vody" : "Water adding")}: {item.waterAmount}\n" +
                    $"{(isCzech ? "Obnova spánku" : "Sleep adding")}: {item.sleepAmount}";
                break;

            case ItemType.Sample:
                if (item.isResearched)
                {
                    statsDescription.text = dynamicInfo +
                        $"{(isCzech ? "Rarita" : "Rarity")}: {item.rarity}\n" +
                        (item.potionHeal > 0 ? $"{(isCzech ? "Obnova HP" : "HP Heal")}: {item.potionHeal}\n" : "") +
                        (item.potionAditionalHealth > 0 ? $"{(isCzech ? "Bonusové HP" : "Bonus HP")}: {item.potionAditionalHealth}\n" : "") +
                        (item.potionBonusSpeed > 0 ? $"{(isCzech ? "Bonusová rychlost" : "Bonus Speed")}: {item.potionBonusSpeed}\n" : "") +
                        (item.potionBonusStamina > 0 ? $"{(isCzech ? "Bonusová stamina" : "Bonus Stamina")}: {item.potionBonusStamina}\n" : "") +
                        (item.potionBonusFOV > 0 ? $"{(isCzech ? "Bonusové FOV" : "Bonus FOV")}: {item.potionBonusFOV}\n" : "") +
                        (item.potionBonushungerSpeed != 0 ? $"{(isCzech ? "Rychlost hladu" : "Hunger Speed")}: {item.potionBonushungerSpeed}\n" : "") +
                        (item.potionBonusdamage > 0 ? $"{(isCzech ? "Bonusové poškození" : "Bonus Damage")}: {item.potionBonusdamage}\n" : "") +
                        (item.hilightResources ? $"{(isCzech ? "Zvýrazňuje suroviny" : "Highlights Resources")}\n" : "") +
                        $"{(isCzech ? "Čas výzkumu" : "Research Time")}: {item.researchTimeMinutes} min";
                }
                else
                {
                    statsDescription.text = dynamicInfo +
                        $"{(isCzech ? "Rarita" : "Rarity")}: {item.rarity}\n" +
                        $"{(isCzech ? "[Ještě nevyzkoumaný]" : "[Not yet researched]")}\n" +
                        $"{(isCzech ? "Čas výzkumu" : "Research Time")}: {item.researchTimeMinutes} min";
                }
                break;

            default:
                statsDescription.text = dynamicInfo + item.description;
                break;
        }
        
        statsIcon.sprite = item.icon;
    }

    public void SetFilter(string typeName)
    {
        if (System.Enum.TryParse(typeName, out ItemType newFilter))
        {
            RefreshInventory(newFilter);
        }
    }

    public void ClearStats()
    {
        statsTitle.text = "";
        statsDescription.text = "";
        statsIcon.sprite = null;
    }
}