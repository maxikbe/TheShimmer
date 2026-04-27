using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacerLoader : MonoBehaviour
{
    [SerializeField] private GameObject campfirePrefab;
    [SerializeField] private GameObject campfireBluePrintPrefab;

    void Start()
    {
        SpawnSavedCampfires();
    }

    private void SpawnSavedCampfires()
    {
        if (gameDataManager.currentGameData == null || gameDataManager.currentGameData.player == null) return;
        
        List<CampFire> savedFires = gameDataManager.currentGameData.player.campFires;

        if (savedFires == null) return;

        foreach (CampFire fireData in savedFires)
        {
            GameObject prefab = fireData.isBlueprint ? campfireBluePrintPrefab : campfirePrefab;
            Vector3 spawnPos = new Vector3(fireData.pos.x, fireData.pos.y, 0f);
            GameObject spawned = Instantiate(prefab, spawnPos, Quaternion.identity);
            
            if (fireData.isBlueprint)
            {
                BluePrintObjectScript bpScript = spawned.GetComponent<BluePrintObjectScript>();
                if (bpScript != null) bpScript.Initialize(fireData);
            }
            else
            {
                campFireScript cfScript = spawned.GetComponent<campFireScript>();
                if (cfScript != null) cfScript.Initialize(fireData);
            }
        }
    }
}