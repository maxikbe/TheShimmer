using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacerLoader : MonoBehaviour
{
//     [SerializeField] private GameObject campfirePrefab;
//     [SerializeField] private GameObject campfireBluePrintPrefab;
//     private Vector2 savedCampFirePos;

//     void Start()
//     {
//         //SpawnSavedCampfires();
//     }

//     private void SpawnSavedCampfires()
//     {
//         List<CampFire> savedFires = gameDataManager.currentGameData.player.campFires;

//         if (savedFires == null) return;

//         foreach (CampFire fireData in savedFires)
//         {
//             savedCampFirePos = fireData.pos;
//             if (fireData.isBlueprint)
//             {
//                 Instantiate(campfireBluePrintPrefab, new Vector3 (savedCampFirePos.x, savedCampFirePos.y, 0f), Quaternion.identity);
//             }
//             else
//             {
//                 Instantiate(campfirePrefab, new Vector3 (savedCampFirePos.x, savedCampFirePos.y, 0f), Quaternion.identity);
//             }
//         }
//     }
}