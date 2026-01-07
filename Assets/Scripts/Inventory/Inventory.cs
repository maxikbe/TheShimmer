using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI; 
using TMPro;

public class Inventory : MonoBehaviour
{
    [SerializeField] private GameObject inventoryMenuUI;
    [SerializeField] private Database database;
    [SerializeField] private Transform container;    
    [SerializeField] private GameObject itemPrefab;  
    [SerializeField] private TextMeshProUGUI statsTitle;       
    [SerializeField] private TextMeshProUGUI statsDescription; 
    [SerializeField] private Image statsIcon;
    

    private bool isOpen = false;
    private List<Item> allItems; 

    void Start()
    {
        allItems = database.GetAllItems();
        RefreshInventory(ItemType.Weapon);
        ClearStats();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isOpen)
                Resume();
            else
                Pause();
        }
    }
    public void Resume()
    {
        inventoryMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isOpen = false;
    }

    public void Pause()
    {
        inventoryMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isOpen = true;
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
        statsDescription.text = "Damage: " + item.Damage; 
        statsIcon.sprite = item.icon;
    }

    public void SetFilter(string typeName)
    {
        Debug.Log("Pokouším se filtrovat podle: " + typeName);

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