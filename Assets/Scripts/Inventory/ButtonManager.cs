using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private GameObject charPickerUI;
    [SerializeField] private GameObject inventoryMenuUI;
    [SerializeField] private GameObject characterMenuUI;
    [SerializeField] private GameObject NavbarUI;
    

    public void openCharPicker()
    {
        NavbarUI.SetActive(true);
        charPickerUI.SetActive(true);
        inventoryMenuUI.SetActive(false);
        characterMenuUI.SetActive(false);
    }


    public void openInventory()
    {
        NavbarUI.SetActive(true);
        inventoryMenuUI.SetActive(true);
        charPickerUI.SetActive(false);
        characterMenuUI.SetActive(false);
    }

    public void openCharacterMenu()
    {
        NavbarUI.SetActive(true);
        characterMenuUI.SetActive(true);
        inventoryMenuUI.SetActive(false);
        charPickerUI.SetActive(false);
    }

    [SerializeField] private GameObject MainGrid;
    [SerializeField] private GameObject GunChooseGrid;
    [SerializeField] private GameObject PerksChooseGrid;
      [SerializeField] private GameObject SkillTreeGrid;
    public void OpenMainGrid()
    {
        MainGrid.SetActive(true);
        GunChooseGrid.SetActive(false);
        PerksChooseGrid.SetActive(false);
        SkillTreeGrid.SetActive(false);
    }

    public void OpenGunChooseGrid()
    {
        MainGrid.SetActive(false);
        GunChooseGrid.SetActive(true);
        PerksChooseGrid.SetActive(false);
        SkillTreeGrid.SetActive(false);
    }

    public void OpenPerksChooseGrid()
    {
        MainGrid.SetActive(false);
        GunChooseGrid.SetActive(false);
        PerksChooseGrid.SetActive(true);
        SkillTreeGrid.SetActive(false);
    } 
    public void OpenSkillTreeGrid()
    {
        MainGrid.SetActive(false);
        GunChooseGrid.SetActive(false);
        PerksChooseGrid.SetActive(false);
        SkillTreeGrid.SetActive(true);
    }

}
