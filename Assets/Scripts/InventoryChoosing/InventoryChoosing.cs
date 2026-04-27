using UnityEngine;
using UnityEngine.UI; 
using System.Collections.Generic;
using TMPro; 

public class InventoryChoosing : MonoBehaviour
{
    [SerializeField] private Database _databaseReference;
    [SerializeField] private Transform Container;    
    [SerializeField] private GameObject ItemPrefab;  
    private Database itemDatabase;
    private List<ItemSaveData> ItemData;
    private List<ItemSaveData> itemsToDisplay = new List<ItemSaveData>();
    
    void Awake()
    {
        itemDatabase = _databaseReference;
    }

    void Start()
    {
        addAllButtons();
    }

    void addAllButtons()
    {
        ItemData = gameDataManager.currentGameData.OwnedItems;
        itemsToDisplay = ItemData.FindAll(item => item.amount > 0 && itemDatabase.GetItemByID(item.id) != null && item.isOwned && itemDatabase.GetItemByID(item.id).itemType == ItemType.Consumable || itemDatabase.GetItemByID(item.id).itemType == ItemType.Resource);
        
        foreach (Transform child in Container) {
            Destroy(child.gameObject);
        }

        foreach (ItemSaveData item in itemsToDisplay)
        {
            Item staticData = itemDatabase.GetItemByID(item.id);
            if (staticData != null)
            {
                GameObject newButton = Instantiate(ItemPrefab, Container);

                Image icon = newButton.transform.Find("ItemImage").GetComponent<Image>();
                TextMeshProUGUI valueText = newButton.transform.Find("Value").GetComponent<TextMeshProUGUI>();
                if (icon != null)
                {
                    icon.sprite = staticData.icon; 
                    icon.preserveAspect = true;
                }
                if (valueText != null)
                {
                    valueText.text = item.amount.ToString();
                }

                Button btn = newButton.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick.AddListener(() => OnItemClicked(item.id));
                }
            }
        }
    }

    void OnItemClicked(int id)
    {
        Debug.Log("Kliknuto na item s ID: " + id);
        Item currentItem = itemDatabase.GetItemByID(id);
        if (currentItem != null)        {
            Debug.Log("Item název: " + currentItem.itemName);
        }

        if(currentItem.itemType == ItemType.Consumable)
        {
            PlayerGUI.Instance.UpdateHunger(currentItem.consumeAmount);
            PlayerGUI.Instance.UpdateThirst(currentItem.waterAmount);
            PlayerGUI.Instance.UpdateSleep(currentItem.sleepAmount);
        }
         else if(currentItem.itemType == ItemType.Resource)
        {
            Debug.Log("Toto je resource item.");
        }
         else
        {
            Debug.Log("Toto není konzumovatelný ani resource item.");
        }

        gameDataManager.currentGameData.OwnedItems.Find(item => item.id == id).amount--;
        addAllButtons();
    }
}