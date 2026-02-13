using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private GameObject charPickerUI;
    [SerializeField] private GameObject inventoryMenuUI;
    [SerializeField] private GameObject characterMenuUI;


    public void openCharPicker()
    {
        charPickerUI.SetActive(true);
        inventoryMenuUI.SetActive(false);
        characterMenuUI.SetActive(false);
    }

    public void openInventory()
    {
        inventoryMenuUI.SetActive(true);
        charPickerUI.SetActive(false);
        characterMenuUI.SetActive(false);
    }

    public void openCharacterMenu()
    {
        characterMenuUI.SetActive(true);
        inventoryMenuUI.SetActive(false);
        charPickerUI.SetActive(false);
    }

    [SerializeField] private GameObject MainGrid;
    [SerializeField] private GameObject GunChooseGrid;
    public void OpenMainGrid()
    {
        MainGrid.SetActive(true);
        GunChooseGrid.SetActive(false);
    }

    public void OpenGunChooseGrid()
    {
        MainGrid.SetActive(false);
        GunChooseGrid.SetActive(true);
    }

}
