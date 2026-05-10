using UnityEngine;
using TMPro; // Nezapomeň na namespace pro TextMeshPro
using System.Collections;

public class InteractionManager : MonoBehaviour
{
    // Singleton instance, aby se to dalo volat odevšad bez hledání reference
    public static InteractionManager Instance { get; private set; }

    [Header("UI Reference")]
    [SerializeField] private CanvasGroup panelCanvasGroup;
    [SerializeField] private TextMeshProUGUI interactionText;

    [Header("Nastavení animace")]
    [SerializeField] private float fadeSpeed = 5f; // Rychlost zobrazení/zmizení

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        // Klasickej Singleton setup
        if (Instance == null) 
        { 
            Instance = this; 
        }
        else 
        { 
            Destroy(gameObject); 
        }

        // Na začátku hry se ujistíme, že je panel schovaný
        panelCanvasGroup.alpha = 0f;
        panelCanvasGroup.interactable = false;
        panelCanvasGroup.blocksRaycasts = false;
    }

    // Statická metoda pro zobrazení - tohle je přesně to tvoje NPCInteraction.ShowInteraction(...)
    public static void ShowInteraction(string text)
    {
        if (Instance == null) return;
        
        Instance.interactionText.text = text;
        Instance.FadeTo(1f);
    }

    // Statická metoda pro schování
    public static void HideInteraction()
    {
        if (Instance == null) return;
        
        Instance.FadeTo(0f);
    }

    private void FadeTo(float targetAlpha)
    {
        // Pokud už zrovna probíhá nějaká animace (např. hráč rychle odchází a přichází), zastavíme ji
        if (fadeCoroutine != null) 
        {
            StopCoroutine(fadeCoroutine);
        }
        
        // Spustíme nové prolínání
        fadeCoroutine = StartCoroutine(FadeRoutine(targetAlpha));
    }

    private IEnumerator FadeRoutine(float target)
    {
        // Dokud nejsme na cílové hodnotě, měníme postupně alfu
        while (!Mathf.Approximately(panelCanvasGroup.alpha, target))
        {
            // Mathf.MoveTowards zajistí lineární a plynulý přechod nezávislý na frameratu
            panelCanvasGroup.alpha = Mathf.MoveTowards(panelCanvasGroup.alpha, target, fadeSpeed * Time.deltaTime);
            yield return null; // Počkáme na další snímek
        }

        // Nakonec zapneme nebo vypneme klikatelnost podle toho, jestli je panel vidět
        bool isVisible = target > 0.1f;
        panelCanvasGroup.interactable = isVisible;
        panelCanvasGroup.blocksRaycasts = isVisible;
    }
}