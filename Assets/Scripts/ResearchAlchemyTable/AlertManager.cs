using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System; // Tohle je důležité pro "Action"

public class AlertManager : MonoBehaviour
{
    public static AlertManager Instance;

    [Header("UI Reference")]
    public GameObject alertPanel;
    public TextMeshProUGUI alertMessageText;
    public Button yesButton;
    public Button noButton;

    private Action onYesAction; // Sem si uložíme, co se má stát, když se klikne na ANO

    private void Awake()
    {
        // Singleton nastavení
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Přiřadíme tlačítkům jejich úkoly
        if (yesButton != null) yesButton.onClick.AddListener(OnYesClicked);
        if (noButton != null) noButton.onClick.AddListener(HideAlert);
        
        HideAlert(); // Rovnou ho skryjeme na startu
    }

    // Tuto funkci budeme volat z jiných skriptů!
    public void ShowAlert(string message, Action yesAction)
    {
        if (alertMessageText != null) alertMessageText.text = message;
        onYesAction = yesAction; // Uložíme si funkci, kterou nám cizí skript poslal
        
        if (alertPanel != null) alertPanel.SetActive(true);
    }

    private void OnYesClicked()
    {
        // Pokud máme uloženou nějakou akci, spustíme ji (Invoke)
        onYesAction?.Invoke(); 
        HideAlert(); // A panel rovnou zavřeme
    }

    public void HideAlert()
    {
        if (alertPanel != null) alertPanel.SetActive(false);
        onYesAction = null; // Vyčistíme paměť
    }
}