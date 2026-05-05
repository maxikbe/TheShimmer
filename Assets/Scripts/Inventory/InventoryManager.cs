using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject charPickerUI;
    [SerializeField] private GameObject inventoryMenuUI;
    [SerializeField] private GameObject characterMenuUI;
    [SerializeField] private GameObject NavbarUI;
    [SerializeField] private GameObject normalInventoryUI;
    private KeyCode intentoryKey = KeyBoardSetting.TBinventory;
    private KeyCode normalInventoryKey = KeyBoardSetting.NormalInventory;
    private bool isOpen = false;
    private bool isNormalInventory = false;

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

    public void OpenNormalInventory()
    {
        normalInventoryUI.SetActive(true);
        Time.timeScale = 0f;
        isNormalInventory = true;
    }

    public void CloseNormalInventory()
    {
        normalInventoryUI.SetActive(false);
        Time.timeScale = 1f;
        isNormalInventory = false;
    }

    public void Update()
    {
        if (Input.GetKeyDown(intentoryKey))
        {
            if (isOpen) Resume();
            else Pause();
        }

        if (Input.GetKeyDown(normalInventoryKey))
        {
            if (isNormalInventory) CloseNormalInventory();
            else OpenNormalInventory();    
        }

    }
}
