using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LabUIManager : MonoBehaviour
{
    [Header("Hlavní Panely (Záložky)")]
    public GameObject researchPanel; // Sem přetáhneš tvůj existující ResearchUI
    public GameObject alchemyPanel;  // Sem dáš nový prázdný panel pro kotlík
    
    [Header("Navigace Nahoře")]
    public Button btnLeft;
    public Button btnRight;
    public TextMeshProUGUI currentModeText; // Text uprostřed šipek

    private int currentMode = 0; // 0 = Výzkum, 1 = Alchymie

    private void Start()
    {
        if (btnLeft != null) btnLeft.onClick.AddListener(SwitchMode);
        if (btnRight != null) btnRight.onClick.AddListener(SwitchMode);
        
        UpdateUI();
    }

    private void SwitchMode()
    {
        // Přepínač mezi 0 a 1 (pokud bys přidal třeba Crafting Zbraní, dal bys tam % 3)
        currentMode = (currentMode + 1) % 2; 
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (currentMode == 0)
        {
            if (currentModeText != null) currentModeText.text = "VÝZKUM VZORKŮ";
            researchPanel.SetActive(true);
            alchemyPanel.SetActive(false);
        }
        else
        {
            if (currentModeText != null) currentModeText.text = "ALCHYMIE (CRAFTING)";
            researchPanel.SetActive(false);
            alchemyPanel.SetActive(true);
        }
    }
}