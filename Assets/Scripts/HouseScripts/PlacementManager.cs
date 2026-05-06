using System.IO.Compression;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    [SerializeField] private GameObject tentPrefab;
    [SerializeField] private GameObject ghostPreview;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private LayerMask validLayer;
    [SerializeField] private float maxDistance = 3f;
    [SerializeField] private float minDistance = 1.5f;
    [SerializeField] private float checkRadius = 0.3f;

    private bool isBuilding = false;
    public bool hasPlacedTent = false;
    private SpriteRenderer ghostRenderer;
    private Vector2 savedTentPos;

    void Awake()
    {
        savedTentPos = gameDataManager.currentGameData.player.tentPos;
        if(!hasPlacedTent && gameDataManager.currentGameData.player.isTentPlaced)
        {
            Debug.Log(savedTentPos.x+""+savedTentPos.y);
            Instantiate(tentPrefab, new Vector3 (savedTentPos.x, savedTentPos.y, 0f), Quaternion.identity);
            hasPlacedTent = true;
        }
        ghostPreview.SetActive(false);
        ghostRenderer = ghostPreview.GetComponent<SpriteRenderer>();
    }

    public void NotifyTentRemoved()
    {
        hasPlacedTent = false;
        gameDataManager.currentGameData.player.isTentPlaced = hasPlacedTent;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyBoardSetting.Tent) && !hasPlacedTent)
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

    bool IsAreaBlocked(Vector3 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, checkRadius);

        foreach (var col in colliders)
        {
            if (col != null)
            {
                if (col.isTrigger) continue;

                return true; 
            }

            if (col.GetComponent<BoxCollider2D>() != null)
            {
                return true; 
            }
        }
        return false;
    }

    void HandleGhostLogic()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        float dist = Vector2.Distance(playerTransform.position, mousePos);
        bool onValidLayer = Physics2D.OverlapCircle(mousePos, 0.2f, validLayer);

        ghostPreview.transform.position = mousePos;
        ghostPreview.transform.rotation = Quaternion.identity;

        if (dist <= maxDistance && dist >= minDistance && onValidLayer && !IsAreaBlocked(mousePos))
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

        if (dist <= maxDistance && onValidLayer && !IsAreaBlocked(pos))
        {
            Instantiate(tentPrefab, pos, Quaternion.identity);
            hasPlacedTent = true;
            isBuilding = false;
            ghostPreview.SetActive(false);
            gameDataManager.currentGameData.player.isTentPlaced = hasPlacedTent;
            gameDataManager.currentGameData.player.tentPos = pos;
        }
    }
}