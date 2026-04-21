using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    [SerializeField] private GameObject tentPrefab;
    [SerializeField] private GameObject ghostPreview;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private LayerMask validLayer;
    [SerializeField] private float maxDistance = 3f;
    [SerializeField] private float minDistance = 1.5f;

    private bool isBuilding = false;
    private bool hasPlacedTent = false;
    private SpriteRenderer ghostRenderer;

    void Start()
    {
        ghostPreview.SetActive(false);
        ghostRenderer = ghostPreview.GetComponent<SpriteRenderer>();
    }

    public void NotifyTentRemoved()
    {
        hasPlacedTent = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && !hasPlacedTent)
        {
            isBuilding = !isBuilding;
            ghostPreview.SetActive(isBuilding);
        }

        if (isBuilding && !hasPlacedTent)
        {
            HandleGhostLogic();

            if (Input.GetMouseButtonDown(0))
            {
                TryPlaceTent();
            }
        }
    }

    void HandleGhostLogic()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        float dist = Vector2.Distance(playerTransform.position, mousePos);
        bool onValidLayer = Physics2D.OverlapCircle(mousePos, 0.2f, validLayer);

        ghostPreview.transform.position = mousePos;
        ghostPreview.transform.rotation = Quaternion.identity;

        if (dist <= maxDistance && dist >= minDistance && onValidLayer)
        {
            ghostRenderer.color = new Color(0, 1, 0, 0.5f);
        }
        else
        {
            ghostRenderer.color = new Color(1, 0, 0, 0.5f);
        }
    }

    void TryPlaceTent()
    {
        Vector3 pos = ghostPreview.transform.position;
        float dist = Vector2.Distance(playerTransform.position, pos);
        bool onValidLayer = Physics2D.OverlapCircle(pos, 0.2f, validLayer);

        if (dist <= maxDistance && onValidLayer)
        {
            Instantiate(tentPrefab, pos, Quaternion.identity);
            hasPlacedTent = true;
            isBuilding = false;
            ghostPreview.SetActive(false);
        }
    }
}