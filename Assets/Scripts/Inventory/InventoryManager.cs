using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject charPickerUI;
    [SerializeField] private GameObject inventoryMenuUI;
    [SerializeField] private GameObject characterMenuUI;
    [SerializeField] private GameObject NavbarUI;
    private bool isOpen = false;

     public void Resume()
    {
        charPickerUI.SetActive(false);
        inventoryMenuUI.SetActive(false);
        characterMenuUI.SetActive(false);
        NavbarUI.SetActive(false);
        Time.timeScale = 1f;
        isOpen = false;
    }

    public void Pause()
    {
        charPickerUI.SetActive(true);
        NavbarUI.SetActive(true);
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
