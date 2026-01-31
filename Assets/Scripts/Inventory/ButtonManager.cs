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

}
