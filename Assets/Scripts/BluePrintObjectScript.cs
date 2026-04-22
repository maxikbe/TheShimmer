using UnityEngine;

public class BluePrintObjectScript : MonoBehaviour
{
    public string itemName = "Campfire";
    public int woodRequired = 3;
    public int rocksRequired = 2;
    public GameObject finalPrefab;

    private int currentWood = 0;
    private int currentRocks = 0;
    private bool isPlayerNearby = false;

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            TryAddMaterials();
        }
    }

    void TryAddMaterials()
    {
        if (currentWood < woodRequired) currentWood++;
        else if (currentRocks < rocksRequired) currentRocks++;

        if (currentWood >= woodRequired && currentRocks >= rocksRequired)
        {
            Instantiate(finalPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) isPlayerNearby = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) isPlayerNearby = false;
    }
}