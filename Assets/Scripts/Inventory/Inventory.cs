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
    
        string dynamicInfo = $"<b>Level: {saveItem.level} | Amount: {saveItem.amount}</b>\n\n";
        
        switch (item.itemType)
        {
            case ItemType.Weapon:
                statsDescription.text = dynamicInfo + $"Damage: {item.Damage}\nFire Rate: {item.FireRate}\nRange: {item.Range}";
                break;
            case ItemType.Armor:
                statsDescription.text = dynamicInfo + $"Armor: {item.Armor}\nDurability: {item.durability}";
                break;
            case ItemType.Healing:
            case ItemType.Consumable:
                statsDescription.text = dynamicInfo + $"Heal: {item.HealAmount}\nUses: {saveItem.amount}";
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