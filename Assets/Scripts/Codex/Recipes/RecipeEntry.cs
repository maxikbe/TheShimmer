using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeEntry : MonoBehaviour
{
    public TextMeshProUGUI recipeNameText;
    public Image recipeIcon;
    public Button myButton;

    private PotionRecipe myRecipe;
    private CodexUIManager myManager;

    public void Setup(PotionRecipe recipe, CodexUIManager manager, bool isFullyDiscovered)
    {
        myRecipe = recipe;
        myManager = manager;

        if (isFullyDiscovered)
        {
            recipeNameText.text = recipe.resultPotion.itemName;
            recipeIcon.sprite = recipe.resultPotion.icon;
            recipeIcon.color = Color.white;
        }
        else
        {
            recipeNameText.text = "Neznámý lektvar";
            recipeIcon.sprite = recipe.resultPotion.icon;
            recipeIcon.color = Color.black; // Tmavá silueta pro neobjevený lektvar
        }

        myButton.onClick.RemoveAllListeners();
        myButton.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        myManager.ShowRecipeDetails(myRecipe);
    }
}