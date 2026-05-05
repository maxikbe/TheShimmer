using UnityEngine;
using UnityEngine.EventSystems;

public class PestleDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Odkazy")]
    public AlchemyUI alchemyUI;

    [Header("Fyzika Hmoždíře (Limity pohybu)")]
    public float maxUpDistance = 120f;   // Jak vysoko nad misku můžeš tlouk vytáhnout
    public float maxDownDistance = 20f;  // TADY JE TVOJE DNO! Jak hluboko do misky to zapadne

    [Header("Nastavení úderu")]
    public float strokeThreshold = 80f;  // Kolik pixelů musíš trhnout dolů pro 1 úder

    private Vector3 startLocalPos;
    private float highestY;
    private float lowestY;
    private bool isStrokeReady = true; 

    private void Start()
    {
        // Uložíme si základní pozici, na které tyčka "sedí" ve scéně
        startLocalPos = transform.localPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        highestY = Input.mousePosition.y;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 1. Posuneme tyčku o tolik, o kolik se pohnula myš (Y osa)
        Vector3 newPos = transform.localPosition;
        newPos.y += eventData.delta.y; 

        // 2. KOUZLO: Tvrdé dno a strop! 
        // Nepustíme Y pozici níž než (start - dno) a výš než (start + strop)
        newPos.y = Mathf.Clamp(newPos.y, startLocalPos.y - maxDownDistance, startLocalPos.y + maxUpDistance);
        
        transform.localPosition = newPos;

        // --- LOGIKA ÚDERU ---
        if (isStrokeReady && Input.mousePosition.y < highestY - strokeThreshold)
        {
            RegisterCrush();
            isStrokeReady = false; 
            lowestY = Input.mousePosition.y; 
        }
        else if (!isStrokeReady && Input.mousePosition.y > lowestY + strokeThreshold)
        {
            isStrokeReady = true;
            highestY = Input.mousePosition.y; 
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Vrátíme tyčku na výchozí místo, když ji pustíš
        transform.localPosition = startLocalPos;
        isStrokeReady = true; 
    }

    private void RegisterCrush()
    {
        if (alchemyUI != null && alchemyUI.currentTable != null)
        {
            if (alchemyUI.currentTable.mortarItemStatic != null && alchemyUI.currentTable.mortarItemStatic.isCrushable)
            {
                alchemyUI.ManualCrush();
            }
        }
    }
}