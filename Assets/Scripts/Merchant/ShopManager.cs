using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [Header("UI Reference")]
    public GameObject shopPanel; // UI obchodu

    private Merchant currentMerchant; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }


    public void OpenShop(Merchant merchant)
    {
        currentMerchant = merchant;
        
        // zapne hlavni UI okno
        shopPanel.SetActive(true);
        

        Debug.Log($"Otevřel jsi shop! Kšeftuješ s: {currentMerchant.name}");
        Debug.Log($"Má u sebe {currentMerchant.currentInventory.Count} věcí k prodeji.");
        
        // Tady se později bude volat nějaká funkce LoadShopUI(), která ti ty itemy vykreslí na obrazovku
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        currentMerchant = null;
    }
}