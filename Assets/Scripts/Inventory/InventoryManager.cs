using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject charPickerUI;
    [SerializeField] private GameObject inventoryMenuUI;
    private bool isOpen = false;

     public void Resume()
    {
        charPickerUI.SetActive(false);
        inventoryMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isOpen = false;
    }

    public void Pause()
    {
        charPickerUI.SetActive(true);
        Time.timeScale = 0f;
        isOpen = true;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isOpen)
                Resume();
            else
                Pause();
        }
    }
}
