using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class WaypointData
{
    public Transform worldObject;
    public RectTransform mapIcon;
}

public class MapControler : MonoBehaviour
{
    public GameObject mapUI;
    public RectTransform playerIcon;
    public Transform playerTransform;
    public float mapScale = 1f;
    public Vector2 mapOffset;

    public List<WaypointData> waypoints = new List<WaypointData>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            mapUI.SetActive(true);
        }
        
        if (Input.GetKeyUp(KeyCode.G))
        {
            mapUI.SetActive(false);
        }

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
}