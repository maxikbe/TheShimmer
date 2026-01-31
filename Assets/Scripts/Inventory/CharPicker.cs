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
        UpdatePositionImmediate();

        // Zavoláme hned na začátku, aby se načetla první postava
        if (statsHolder != null) statsHolder.UpdateStats(currentIndex);
    }

   

    void Update()
    {
        if (isAnimating) return;

        int direction = 0;
        if (Input.GetKeyDown(KeyCode.RightArrow)) direction = 1;
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) direction = -1;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        // Zachování tvé funkční logiky scrollu
        if (scroll > 0f) direction = -1; // Pokud ti to scrollovalo naopak, změň na 1
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

        // TADY voláme aktualizaci statistik - posíláme tam targetIndex
        if (statsHolder != null)
        {
            statsHolder.UpdateStats(targetIndex);
        }
        
        Vector3 startPos = container.localPosition;
        Vector3 endPos = startPosition + new Vector3(-targetIndex * elementWidth, 0, 0);

        float time = 0;
        while (time < slideDuration)
        {
            time += Time.deltaTime;
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
            container.localPosition = startPosition + new Vector3(-currentIndex * elementWidth, 0, 0);
    }
}