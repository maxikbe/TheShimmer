using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class LootUIManager : MonoBehaviour
{
    public static LootUIManager Instance { get; private set; }

    [Header("UI Nastavení")]
    public GameObject notificationPrefab;
    public Transform notificationContainer; 
    public float displayTime = 2.5f;
    public float delayBetweenItems = 0.3f;

    [Header("Databáze")]
    public Database itemDatabase;

    // Vytvoříme si přepravku, aby fronta věděla, co s itemem děláme
    private struct LootEvent
    {
        public Item item;
        public bool isAdding;
    }

    // Fronta teď polyká naši novou přepravku místo samotného Itemu
    private Queue<LootEvent> notificationQueue = new Queue<LootEvent>();
    private bool isProcessing = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Přidán parametr isAdding (defaultně true, takže se starý kód nerozbije)
    public void ShowLoot(Item item, bool isAdding = true)
    {
        if (item == null) return;
        
        notificationQueue.Enqueue(new LootEvent { item = item, isAdding = isAdding });
        
        if (!isProcessing) StartCoroutine(ProcessQueue());
    }

    // To samé pro ID
    public void ShowLoot(int itemID, bool isAdding = true)
    {
        if (itemDatabase == null) return;

        Item foundItem = itemDatabase.GetItemByID(itemID);
        if (foundItem != null) ShowLoot(foundItem, isAdding);
    }

    private IEnumerator ProcessQueue()
    {
        isProcessing = true;

        while (notificationQueue.Count > 0)
        {
            LootEvent currentEvent = notificationQueue.Dequeue();
            
            GameObject notifObj = Instantiate(notificationPrefab, notificationContainer);
            
            LootNotificationItem logic = notifObj.GetComponent<LootNotificationItem>();
            if (logic != null)
            {
                // Pošleme prefabu informaci, jestli přidáváme nebo odebíráme
                logic.Initialize(currentEvent.item, displayTime, currentEvent.isAdding);
            }

            yield return new WaitForSeconds(delayBetweenItems); 
        }

        isProcessing = false;
    }
}