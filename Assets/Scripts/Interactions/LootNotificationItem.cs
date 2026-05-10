using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class LootNotificationItem : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    [SerializeField] private float fadeSpeed = 3f;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f; 
    }

    public void Initialize(Item item, float displayTime, bool isAdding)
    {
        // Najdeme všechny tři tvoje objekty přesně podle těch screenů
        Transform iconTransform = transform.Find("Image");
        Transform textTransform = transform.Find("Text");
        Transform signTransform = transform.Find("Sign"); // Tady je ten tvůj nový vagón

        // 1. Nastavíme ikonku
        if (iconTransform != null && item.icon != null) 
        {
            iconTransform.GetComponent<Image>().sprite = item.icon;
        }

        // Určíme barvu (Bílá pro zisk, červená pro ztrátu)
        Color textColor = isAdding ? Color.white : new Color(1f, 0.33f, 0.33f);

        // 2. Nastavíme Znaménko (+ nebo -)
        if (signTransform != null)
        {
            TextMeshProUGUI signUI = signTransform.GetComponent<TextMeshProUGUI>();
            signUI.text = isAdding ? "+" : "-";
            signUI.color = textColor; // Ať to zčervená i se znaménkem
        }

        // 3. Nastavíme Text (Množství a Název)
        if (textTransform != null) 
        {
            TextMeshProUGUI textUI = textTransform.GetComponent<TextMeshProUGUI>();
            textUI.text = $"1 {item.itemName}"; // Už bez znaménka, to je vedle
            textUI.color = textColor;
        }

        // Odstartujeme animaci
        StartCoroutine(LifeCycle(displayTime));
    }

    private IEnumerator LifeCycle(float waitTime)
    {
        yield return StartCoroutine(FadeRoutine(1f)); // Zjevení
        yield return new WaitForSeconds(waitTime);    // Čekačka
        yield return StartCoroutine(FadeRoutine(0f)); // Zmizení
        Destroy(gameObject);                          // Koš
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
        while (!Mathf.Approximately(canvasGroup.alpha, targetAlpha))
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
            yield return null;
        }
    }
}