using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public class CraftableItem
{
    public string itemName;
    public GameObject ghostPrefab;
    public GameObject buildSitePrefab;
}

public class BuildingManager : MonoBehaviour
{
    public GameObject craftingMenu;
    public List<CraftableItem> craftableItems;
    public LayerMask buildableLayer;
    public float maxBuildDistance = 3f;
    public Transform playerTransform;

    private GameObject currentGhostInstance;
    private CraftableItem selectedItem;
    private bool isPlacing = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyBoardSetting.Craft))
        {
            craftingMenu.SetActive(!craftingMenu.activeSelf);
        }

        if (isPlacing)
        {
            HandlePlacementLoop();
        }
    }

    public void SelectItemByIndex(int index)
    {
        Debug.Log(index);
        if (index < 0 || index >= craftableItems.Count) return;

        if (currentGhostInstance != null) Destroy(currentGhostInstance);

        selectedItem = craftableItems[index];
        currentGhostInstance = Instantiate(selectedItem.ghostPrefab);
        
        craftingMenu.SetActive(false);
        isPlacing = true;
    }

    void HandlePlacementLoop()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        currentGhostInstance.transform.position = mousePos;

        float dist = Vector2.Distance(playerTransform.position, mousePos);
        bool canPlace = dist <= maxBuildDistance && Physics2D.OverlapCircle(mousePos, 0.1f, buildableLayer);

        UpdateGhostVisuals(canPlace);

        if (Input.GetMouseButtonDown(0) && canPlace)
        {
            PlaceBuildSite();
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            CancelPlacement();
        }
    }

    void UpdateGhostVisuals(bool canPlace)
    {
        SpriteRenderer sr = currentGhostInstance.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = canPlace ? new Color(0, 1, 0, 0.5f) : new Color(1, 0, 0, 0.5f);
        }
    }

    void PlaceBuildSite()
    {
        CampFire newFireData = new CampFire {
            id = Guid.NewGuid().ToString(),
            pos = currentGhostInstance.transform.position,
            isBlueprint = true,
            woodLeft = 3,
            stoneLeft = 2,
            woodFuelAmount = 1,
            isLit = false
        };

        gameDataManager.currentGameData.player.campFires.Add(newFireData);

        GameObject site = Instantiate(selectedItem.buildSitePrefab, newFireData.pos, Quaternion.identity);
        site.GetComponent<BluePrintObjectScript>().Initialize(newFireData);

        CancelPlacement();
    }

    void CancelPlacement()
    {
        if (currentGhostInstance != null) Destroy(currentGhostInstance);
        isPlacing = false;
        selectedItem = null;
    }
}