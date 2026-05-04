using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LabUIManager : MonoBehaviour
{
    [Header("Manager Skripty")]
    public ResearchUI researchManager; // Odkaz na tvůj Research skript
    public AlchemyUI alchemyManager;   // Odkaz na tvůj nový Alchemy skript

    [Header("Navigace Nahoře")]
    public Button btnLeft;
    public Button btnRight;
    public TextMeshProUGUI currentModeText;

    private int currentMode = 0; // 0 = Výzkum, 1 = Alchymie
    private LabTable activeTable; // Stůl, u kterého zrovna stojíš

    private void Start()
    {
        if (btnLeft != null) btnLeft.onClick.AddListener(SwitchMode);
        if (btnRight != null) btnRight.onClick.AddListener(SwitchMode);
    }

    // Tuhle funkci musíš nově zavolat z TestTableInteract.cs MÍSTO toho, 
    // abys volal rovnou ResearchUI!
    public void OpenLabTerminals(LabTable table)
    {
        activeTable = table;
        
        // Zobrazíme Canvas (předpokládáme, že LabUIManager je na Canvasu nebo máš zapínání pořešené v TestTableInteract)
        
        // Vždycky začneme na nule (Výzkum)
        currentMode = 0; 
        UpdateUI();
    }

    public void CloseLabTerminals()
    {
        activeTable = null;
        if (researchManager != null) researchManager.CloseCanvas();
        if (alchemyManager != null) alchemyManager.CloseAlchemy();
    }

    private void SwitchMode()
    {
        currentMode = (currentMode + 1) % 2; 
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (activeTable == null) return;

        if (currentMode == 0)
        {
            if (currentModeText != null) currentModeText.text = "VÝZKUM VZORKŮ";
            
            // Vypneme jen vnitřek Alchymie (AlchemyScreen)
            if (alchemyManager != null && alchemyManager.alchemyScreenPanel != null) 
                alchemyManager.alchemyScreenPanel.SetActive(false);
            
            // Zapneme Výzkum
            if (researchManager != null) researchManager.OpenCanvas(activeTable);
        }
        else
        {
            if (currentModeText != null) currentModeText.text = "ALCHYMIE (CRAFTING)";
            
            // ZMĚNA ZDE: Už nevoláme CloseCanvas(), ale jen schováme vnitřek Výzkumu!
            if (researchManager != null) researchManager.HideResearchTabOnly();
            
            // Zapneme Alchymii
            if (alchemyManager != null) alchemyManager.OpenAlchemy(activeTable);
        }
    }
}