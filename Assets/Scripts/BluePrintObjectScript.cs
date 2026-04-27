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
    private CampFire myData;

    public void Initialize(CampFire data)
    {
        myData = data;
        currentWood = woodRequired - data.woodLeft;
        currentRocks = rocksRequired - data.stoneLeft;

        if (textInfo != null)
        {
            UpdateUI();
        }
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            TryAddMaterials();
        }

        if (isPlayerNearby && Input.GetKey(KeyCode.F))
        {
            holdTimer += Time.deltaTime;
            if (interactUIHOLDText != null) 
                interactUIHOLDText.text = "Packing... " + (holdTimer / holdDuration * 100f).ToString("F0") + "%";

            if (holdTimer >= holdDuration)
            {
                gameDataManager.currentGameData.player.campFires.Remove(myData);
                gameDataManager.SaveData();
                Destroy(gameObject);
            }
        }
        else if (Input.GetKeyUp(KeyCode.F))
        {
            holdTimer = 0f;
            if (interactUIHOLDText != null) interactUIHOLDText.text = TextContent;
        }
    }

    void TryAddMaterials()
    {
        if (currentWood < woodRequired)
        {
            currentWood++;
            if (myData != null) myData.woodLeft--;
        }
        else if (currentRocks < rocksRequired)
        {
            currentRocks++;
            if (myData != null) myData.stoneLeft--;
        }

        UpdateUI();

        if (currentWood >= woodRequired && currentRocks >= rocksRequired)
        {
            myData.isBlueprint = false;
            GameObject final = Instantiate(finalPrefab, transform.position, Quaternion.identity);
            final.GetComponent<campFireScript>().Initialize(myData);
            gameDataManager.SaveData();
            Destroy(gameObject);
        }
    }

    void UpdateUI()
    {
        if (textInfo != null)
            textInfo.text = "Wood " + currentWood + " / " + woodRequired + "\nRocks " + currentRocks + " / " + rocksRequired;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            Transform canvas = other.transform.Find("PlayerInfoUICanvas");
            if (canvas != null)
            {
                InteractUI = canvas.Find("Interaction").gameObject;
                InteractUIHOLD = canvas.Find("LongInteractionHOLD").gameObject;

                interactUIText = InteractUI.GetComponentInChildren<TextMeshProUGUI>();
                interactUIText.text = interactUITextContent;
                InteractUI.SetActive(true);

                interactUIHOLDText = InteractUIHOLD.GetComponentInChildren<TextMeshProUGUI>();
                interactUIHOLDText.text = TextContent;
                InteractUIHOLD.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            holdTimer = 0f;
            if (InteractUI != null) InteractUI.SetActive(false);
            if (InteractUIHOLD != null) InteractUIHOLD.SetActive(false);
        }
    }
}