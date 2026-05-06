using UnityEngine;
using TMPro;

public class Tent : MonoBehaviour
{
    public string TextContent = "PickUp [F]";
    private TextMeshProUGUI interactUIHOLDText;
    private GameObject InteractUIHOLD;
    private PlacementManager manager;

    [Header("Settings")]
    public float holdDuration = 3f; 
    private float holdTimer = 0f;
    private bool isPlayerNearby = false;

    void Start()
    {
        manager = FindFirstObjectByType<PlacementManager>();
    }

    void Update()
    {
        if (isPlayerNearby)
        {
            if (Input.GetKey(KeyBoardSetting.Pack))
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
    }

    public void PickUp()
    {
        if (InteractUIHOLD != null) InteractUIHOLD.SetActive(false);
        
        if (manager != null)
        {
            manager.NotifyTentRemoved();
        }
        Destroy(gameObject.transform.parent.gameObject);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            InteractUIHOLD = other.transform.Find("PlayerInfoUICanvas/LongInteractionHOLD").gameObject;
            
            if (InteractUIHOLD != null)
            {
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
            if (InteractUIHOLD != null) InteractUIHOLD.SetActive(false);
        }
    }
}