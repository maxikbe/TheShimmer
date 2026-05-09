using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class LootUIManager : MonoBehaviour
{
    // Singleton, ať k němu má SimpleLoot snadný přístup bez hledání
    public static LootUIManager Instance;

    [Header("UI Nastavení")]
    [Tooltip("Prefab notifikace s ikonou a textem")]
    [SerializeField] private GameObject notificationPrefab;
    
    [Tooltip("Kontejner v rohu obrazovky (ideálně s Vertical Layout Group)")]
    [SerializeField] private Transform notificationContainer; 
    
    [Tooltip("Jak dlouho notifikace zůstane na obrazovce")]
    [SerializeField] private float displayTime = 2.5f;
    
    [Tooltip("Pauza mezi zobrazením jednotlivých itemů")]
    [SerializeField] private float delayBetweenItems = 0.3f;

    private Queue<Item> notificationQueue = new Queue<Item>();
    private bool isProcessing = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Metoda, kterou volá SimpleLoot
    public void ShowLootNotification(Item item)
    {
        notificationQueue.Enqueue(item);
        
        if (!isProcessing)
        {
            StartCoroutine(ProcessNotificationQueue());
        }
    }

    private IEnumerator ProcessNotificationQueue()
    {
        isProcessing = true;

        while (notificationQueue.Count > 0)
        {
            Item itemToShow = notificationQueue.Dequeue();
            
            // Spawnneme prefab do pravého dolního rohu
            GameObject notifObj = Instantiate(notificationPrefab, notificationContainer);
            
            // Najdeme UI prvky (názvy "Icon" a "Text" uprav podle toho, jak si pojmenuješ objekty v prefabu)
            Image icon = notifObj.transform.Find("Icon").GetComponent<Image>();
            TextMeshProUGUI text = notifObj.transform.Find("Text").GetComponent<TextMeshProUGUI>();

            if (icon != null && itemToShow.icon != null) icon.sprite = itemToShow.icon;
            if (text != null) text.text = $"+1 {itemToShow.itemName}";

            // Zničíme notifikaci po určeném čase
            Destroy(notifObj, displayTime);

            // Počkáme malou chvíli, než vyhodíme další item z fronty (dělá to hezký efekt postupné kaskády)
            yield return new WaitForSeconds(delayBetweenItems); 
        }

        isProcessing = false;
    }
}