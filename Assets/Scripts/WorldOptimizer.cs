using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class WorldOptimizer : MonoBehaviour
{
    public Transform treeContainer;
    public float viewDistance = 100f;
    private List<GameObject> chunks = new List<GameObject>();
    private bool isReady = false;
    public String nameContainer;

    private Queue<GameObject> activationQueue = new Queue<GameObject>();

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);

        // Find Hidden Container (God Mode Search)
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        GameObject foundContainer = null;
        foreach (GameObject go in allObjects)
        {
            if (go.name == nameContainer && go.scene.isLoaded)
            {
                foundContainer = go;
                break;
            }
        }

        if (foundContainer == null) yield break;
        treeContainer = foundContainer.transform;
        treeContainer.gameObject.SetActive(false);

        int counter = 0;
        foreach (Transform child in treeContainer)
        {
            chunks.Add(child.gameObject);
            child.gameObject.SetActive(false);
            counter++;
            if (counter % 5000 == 0) yield return null; 
        }

        treeContainer.gameObject.SetActive(true);
        isReady = true;
        
        // Start the background worker that prevents freezes
        StartCoroutine(SmoothActivationWorker());
    }

    void Update()
    {
        if (!isReady || chunks.Count == 0 || Time.frameCount % 10 != 0) return;

        Vector3 pPos = transform.position;
        float sqrDist = viewDistance * viewDistance;

        foreach (GameObject chunk in chunks)
        {
            if (chunk.transform.childCount == 0) continue;

            Vector3 chunkPos = chunk.transform.GetChild(0).position;
            bool shouldBeActive = (pPos - chunkPos).sqrMagnitude < sqrDist;

            if (shouldBeActive && !chunk.activeSelf)
            {
                if (!activationQueue.Contains(chunk))
                    activationQueue.Enqueue(chunk);
            }
            else if (!shouldBeActive && chunk.activeSelf)
            {
                chunk.SetActive(false);
            }
        }
    }

    IEnumerator SmoothActivationWorker()
    {
        while (true)
        {
            if (activationQueue.Count > 0)
            {
                GameObject chunk = activationQueue.Dequeue();
                if (chunk != null)
                {
                    chunk.SetActive(true);
                    yield return null; 
                }
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}