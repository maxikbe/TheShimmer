using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CharPicker : MonoBehaviour
{
    [Header("Propojení")]
    [SerializeField] private CharpickerStatsHolder statsHolder; 

    [Header("Nastavení kontejneru")]
    [SerializeField] private RectTransform container; 
    [SerializeField] private float elementWidth = 200f; 
    [SerializeField] private float slideDuration = 0.3f;
    

    [Header("Stav")]
    public int currentIndex = 0; 
    private bool isAnimating = false;

    private Vector3 startPosition;

    void Start()
    {
        if (container != null)
        {
            startPosition = container.localPosition;
        }
        
        // Zavoláme hned na začátku, aby se načetla první postava
        if (statsHolder != null) statsHolder.UpdateStats(currentIndex);
        
        UpdatePositionImmediate();
    }

    // TOTO JE NOVÉ: Když se objekt (UI) zapne, ujistíme se, že je vše na svém místě
    void OnEnable()
    {
        isAnimating = false;
        UpdatePositionImmediate();
    }

    // TOTO JE HLAVNÍ OPRAVA: Když se objekt vypne, zastavíme animaci
    void OnDisable()
    {
        StopAllCoroutines();
        isAnimating = false;
        // Volitelně: Hned snapneme pozici na aktuální index, aby to nezůstalo "viset" v půlce
        UpdatePositionImmediate();
    }

    void Update()
    {
        // Pokud je hra pauznutá, Update stále běží (protože InventoryManager je asi nastaven na ignorování pauzy nebo UI běží), 
        // ale musíme si dát pozor na vstupy.
        
        if (isAnimating) return;

        int direction = 0;
        if (Input.GetKeyDown(KeyCode.RightArrow)) direction = 1;
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) direction = -1;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) direction = -1; 
        else if (scroll < 0f) direction = 1;

        if (direction != 0)
        {
            int nextIndex = Mathf.Clamp(currentIndex + direction, 0, 4);
            
            if (nextIndex != currentIndex)
            {
                StartCoroutine(SlideToCharacter(nextIndex));
            }
        }
    }

    IEnumerator SlideToCharacter(int targetIndex)
    {
        isAnimating = true;

        if (statsHolder != null)
        {
            statsHolder.UpdateStats(targetIndex);
        }
        
        Vector3 startPos = container.localPosition;
        Vector3 endPos = startPosition + new Vector3(-targetIndex * elementWidth, 0, 0);

        float time = 0;
        while (time < slideDuration)
        {
            // DŮLEŽITÁ ZMĚNA: Používáme unscaledDeltaTime, aby animace běžela i při PAUZE (Time.timeScale = 0)
            time += Time.unscaledDeltaTime; 
            
            float t = time / slideDuration;
            t = Mathf.SmoothStep(0, 1, t); 
            
            container.localPosition = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        container.localPosition = endPos;
        currentIndex = targetIndex;
        isAnimating = false;
        
        Debug.Log($"Vybrán charakter č.: {currentIndex + 1}");
    }

    void UpdatePositionImmediate()
    {
        if (container != null)
            // Tady jsem přidal výpočet startPosition, pokud by Start() ještě neproběhl (pro jistotu)
            container.localPosition = (startPosition == Vector3.zero ? container.localPosition : startPosition) + new Vector3(-currentIndex * elementWidth, 0, 0);
    }
}