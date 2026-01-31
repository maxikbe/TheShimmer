using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI; 
using TMPro;

public class Inventory : MonoBehaviour
{
    [SerializeField] private Database database;
    [SerializeField] private Transform container;    
    [SerializeField] private GameObject itemPrefab;  
    [SerializeField] private TextMeshProUGUI statsTitle;       
    [SerializeField] private TextMeshProUGUI statsDescription; 
    [SerializeField] private Image statsIcon;
    

    private List<Item> allItems; 

    void Start()
    {
        allItems = database.GetAllItems();
        RefreshInventory(ItemType.Weapon);
        ClearStats();
    }

    

    public void RefreshInventory(ItemType filter)
    {
        if (allItems == null || allItems.Count == 0) {
            Debug.LogWarning("Seznam allItems je prázdný! Zkouším znovu načíst z databáze.");
            allItems = database.GetAllItems();
        }
        foreach (Transform child in container) {
            Destroy(child.gameObject);
        }
        foreach (Item item in allItems)
        {
            if (item.isOwned && item.itemType == filter)
            {
                GameObject slot = Instantiate(itemPrefab, container);
            
                slot.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = item.itemName;
                Transform iconTransform = slot.transform.Find("Image"); 
                if (iconTransform != null) {
                    Image iconImage = iconTransform.GetComponent<Image>();
                    if (item.icon != null) iconImage.sprite = item.icon;
                }

                Button btn = slot.GetComponent<Button>();

                btn.onClick.AddListener(() => ShowStats(item));

                if (!item.isUsable) {
                    btn.interactable = false;
                }
            }
        }
    }


    public void ShowStats(Item item)
    {
        statsTitle.text = item.itemName;
        string enumTypeName = item.itemType.ToString();
        Debug.Log($"Zobrazuji statistiky pro položku: {item.itemName} typu {enumTypeName}");
        switch (item.itemType)
        {
            case ItemType.Consumable:
                statsDescription.text = $"Type: {enumTypeName}\nConsume Amount: {item.consumeAmount}\nWater Amount: {item.waterAmount}";
                break;
            case ItemType.Healing:
                statsDescription.text = $"Type: {enumTypeName}\nHeal Amount: {item.HealAmount}";
                break;
            case ItemType.Armor:
                statsDescription.text = $"Type: {enumTypeName}\nArmor Type: {item.armorType}\nArmor: {item.Armor}\nDurability: {item.durability}\nWeight: {item.weight}";
                break;
            case ItemType.Resource:
                statsDescription.text = $"Type: {enumTypeName}\nDescription: {item.description}";
                break;
            case ItemType.Weapon:
                if(item.weaponType == WeaponType.Ranged) statsDescription.text = $"Weapon Type: {item.weaponType}\nDamage: {item.Damage}\nFire Rate: {item.FireRate}\nRange: {item.Range}\nReload Time: {item.ReloadTime}\nAmmo Capacity: {item.AmmoCapacity}";
                if(item.weaponType == WeaponType.Melee) statsDescription.text = $"Weapon Type: {item.weaponType}\nDamage: {item.Damage}";
                if(item.weaponType == WeaponType.Magic) statsDescription.text = $"Weapon Type: {item.weaponType}\nDamage: {item.Damage}\nFire Rate: {item.FireRate}\nRange: {item.Range}\nReload Time: {item.ReloadTime}\nAmmo Capacity: {item.AmmoCapacity}\nMagical Element: {item.magicalElement}";
                      
                break;
            default:
                statsDescription.text = "No stats available.";
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
        else
        {
            Debug.LogError($"Chyba: '{typeName}' neodpovídá žádnému typu v ItemType Enumu!");
        }
    }

    public void ClearStats()
    {
        statsTitle.text = "";
        statsDescription.text = "";
        statsIcon.sprite = null;
    }
}