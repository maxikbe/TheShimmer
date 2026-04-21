using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

[System.Serializable]
public class WaypointData
{
    public Transform worldObject;
    public RectTransform mapIcon;
}

public class MapControler : MonoBehaviour, IDragHandler, IScrollHandler
{
    public GameObject mapUI;
    public RectTransform playerIcon;
    public Transform playerTransform;
    public float mapScale = 1f;
    public Vector2 mapOffset;

    public List<WaypointData> waypoints = new List<WaypointData>();
    public GameObject largeIcons;
    public GameObject smallIcons;


    [SerializeField] private float zoomSpeed = 0.1f;
    [SerializeField] private float minZoom = 1f;
    [SerializeField] private float maxZoom = 5f;

    [Header("Horizontal Bounds")]
    [SerializeField] private float minX = -1000f;
    [SerializeField] private float maxX = 1000f;

    [Header("Vertical Bounds")]
    [SerializeField] private float minY = -1000f;
    [SerializeField] private float maxY = 1000f;

    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G)) mapUI.SetActive(true);
        if (Input.GetKeyUp(KeyCode.G)) mapUI.SetActive(false);

        if (mapUI.activeSelf)
        {
            UpdatePositions();
        }
    }

    void UpdatePositions()
    {
        if (playerTransform != null && playerIcon != null)
        {
            playerIcon.anchoredPosition = CalculateMapPos(playerTransform.position);
        }

        foreach (WaypointData waypoint in waypoints)
        {
            if (waypoint.worldObject != null && waypoint.mapIcon != null)
            {
                waypoint.mapIcon.anchoredPosition = CalculateMapPos(waypoint.worldObject.position);
            }
        }
    }

    Vector2 CalculateMapPos(Vector3 worldPos)
    {
        return new Vector2(
            (worldPos.x * mapScale) + mapOffset.x,
            (worldPos.y * mapScale) + mapOffset.y
        );
    }

    public void OnDrag(PointerEventData eventData)
    {
        mapOffset += eventData.delta;
        ClampOffset();
    }

    public void OnScroll(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localMousePos);

        float oldScale = mapScale;

        float zoomAmount = eventData.scrollDelta.y * zoomSpeed;
        mapScale = Mathf.Clamp(mapScale + zoomAmount, minZoom, maxZoom);

        float scaleRatio = mapScale / oldScale;
        mapOffset = localMousePos - (localMousePos - mapOffset) * scaleRatio;

        if(largeIcons != null && smallIcons != null)
        {
            if(mapScale >= 2)
            {
                largeIcons.SetActive(false);
                smallIcons.SetActive(true);
            }
            else
            {
                largeIcons.SetActive(true);
                smallIcons.SetActive(false);
            }
        }

        ClampOffset();
    }

    private void ClampOffset()
    {
        mapOffset.x = Mathf.Clamp(mapOffset.x, minX, maxX);
        mapOffset.y = Mathf.Clamp(mapOffset.y, minY, maxY);
    }
}