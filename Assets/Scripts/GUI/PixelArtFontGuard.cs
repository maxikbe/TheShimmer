using UnityEngine;
using TMPro;

[ExecuteInEditMode]
[RequireComponent(typeof(TextMeshProUGUI))]
public class PixelArtFontGuard : MonoBehaviour
{
    private TextMeshProUGUI text;
    private const int BaseSize = 16;

    void Update()
    {
        if (text == null) text = GetComponent<TextMeshProUGUI>();

        if (text.enableAutoSizing) text.enableAutoSizing = false;

        float currentSize = text.fontSize;
        if (currentSize % BaseSize != 0)
        {
            text.fontSize = Mathf.Max(BaseSize, Mathf.Round(currentSize / BaseSize) * BaseSize);
        }
    }
}