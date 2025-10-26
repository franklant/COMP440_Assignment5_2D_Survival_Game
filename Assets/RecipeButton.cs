using UnityEngine;
using UnityEngine.UI;
using TMPro; // Use this if you are using TextMeshPro for your text

public class RecipeButton : MonoBehaviour
{
    [Header("UI References")]
    public Button button;
    public Image itemIcon;
    public TextMeshProUGUI itemNameText; // Using TextMeshProUGUI is recommended

    [Header("Visuals")]
    public Color craftableColor = Color.white;
    public Color lockedColor = new Color(0.5f, 0.5f, 0.5f, 0.75f);

    // Internal references
    private Recipe currentRecipe;
    private CraftingUI craftingUI;

    void Awake()
    {
        button.onClick.AddListener(OnButtonClick);
    }

    public void Setup(Recipe recipe, CraftingUI uiController)
    {
        currentRecipe = recipe;
        craftingUI = uiController;

        // --- THIS IS THE CORRECTED LINE ---
        itemIcon.sprite = recipe.outputItem.itemIcon; 
        
        itemNameText.text = recipe.outputItem.itemName;
        UpdateVisuals();
    }
    
    public void UpdateVisuals()
    {
        bool canCraft = craftingUI.CanCraft(currentRecipe);
        button.interactable = canCraft;
        itemIcon.color = canCraft ? craftableColor : lockedColor;
    }

    private void OnButtonClick()
    {
        craftingUI.craftingManager.Craft(currentRecipe);
    }
}