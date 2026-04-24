using TMPro;
using UnityEngine;

public class BluePrintObjectScript : MonoBehaviour
{
    public string itemName = "Campfire";
    public int woodRequired = 3;
    public int rocksRequired = 2;
    public GameObject finalPrefab;
    public TextMeshProUGUI textInfo;
    private TextMeshProUGUI interactUIText;
    private GameObject InteractUI;
    public string interactUITextContent = "Insert [E]"; 
    public float holdDuration = 3f; 
    private float holdTimer = 0f;

    private int currentWood = 0;
    private int currentRocks = 0;
    private bool isPlayerNearby = false;

    public string TextContent = "Destroy [F]";
    private TextMeshProUGUI interactUIHOLDText;
    private GameObject InteractUIHOLD;
    private PlacementManager manager;

    void Start()
    {
        textInfo.text = "Wood " + currentWood +" / " + woodRequired + "\nRocks " + currentRocks + " / " + rocksRequired;
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            TryAddMaterials();
        }
        if (Input.GetKey(KeyCode.F))
        {
            holdTimer += Time.deltaTime;

            if (interactUIHOLDText != null) 
            interactUIHOLDText.text = "Packing... " + (holdTimer / holdDuration * 100).ToString("F0") + "%";

            if (holdTimer >= holdDuration)
            {
                PickUp();
                holdTimer = 0f;
            }
        }
        else
        {
            holdTimer = 0f;
        }
    }
    public void PickUp()
    {
        if (InteractUIHOLD != null) InteractUIHOLD.SetActive(false);
        if (InteractUI != null) InteractUI.SetActive(false);
        Destroy(gameObject);
    }

    void TryAddMaterials()
    {
        if (currentWood < woodRequired) currentWood++;
        else if (currentRocks < rocksRequired) currentRocks++;

        textInfo.text = "Wood " + currentWood +" / " + woodRequired + "\nRocks " + currentRocks + " / " + rocksRequired;

        if (currentWood >= woodRequired && currentRocks >= rocksRequired)
        {
            Instantiate(finalPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        InteractUI = other.transform.Find("PlayerInfoUICanvas/Interaction").gameObject;
        if (InteractUI != null){
            interactUIText = InteractUI.GetComponentInChildren<TextMeshProUGUI>();
            interactUIText.text = interactUITextContent;
        }

        InteractUIHOLD = other.transform.Find("PlayerInfoUICanvas/LongInteractionHOLD").gameObject;
            
        if (InteractUIHOLD != null)
        {
            interactUIHOLDText = InteractUIHOLD.GetComponentInChildren<TextMeshProUGUI>();
            interactUIHOLDText.text = TextContent;
            InteractUIHOLD.SetActive(true);
        }
        if (other.CompareTag("Player")){ isPlayerNearby = true; InteractUI.SetActive(true);}
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")){ isPlayerNearby = false; InteractUI.SetActive(false); InteractUIHOLD.SetActive(false);}
    }
}