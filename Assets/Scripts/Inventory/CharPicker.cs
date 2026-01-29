using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class CharPicker : MonoBehaviour
{
[Header("Nastavení kontejneru")]
    [SerializeField] private RectTransform container; // Ten objekt, co drží všech 5 postav
    [SerializeField] private float elementWidth = 200f; // Vzdálenost mezi středy postav
    [SerializeField] private float slideDuration = 0.3f;

    [Header("Stav")]
    public int currentIndex = 0; // 0 až 4
    private bool isAnimating = false;

    // Uložíme si startovní pozici kontejneru (střed první postavy)
    private Vector3 startPosition;

    void Start()
    {
        if (container != null)
        {
            startPosition = container.localPosition;
        }
        UpdatePositionImmediate();
    }

    void Update()
    {
        if (isAnimating) return;

        int direction = 0;
        if (Input.GetKeyDown(KeyCode.RightArrow)) direction = 1;
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) direction = -1;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) direction = 1;
        else if (scroll < 0f) direction = -1;

        if (direction != 0)
        {
            // Omezíme výběr na 0 až 4 (aby se nevyjíždělo mimo seznam)
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
        
        Vector3 startPos = container.localPosition;
        // Výpočet cílové pozice: startovní pozice mínus posun o šířku prvků
        Vector3 endPos = startPosition + new Vector3(-targetIndex * elementWidth, 0, 0);

        float time = 0;
        while (time < slideDuration)
        {
            time += Time.deltaTime;
            float t = time / slideDuration;
            // SmoothStep zajistí hezký dojezd (pomalý rozjezd a pomalý konec)
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
        container.localPosition = startPosition + new Vector3(-currentIndex * elementWidth, 0, 0);
    }
}
