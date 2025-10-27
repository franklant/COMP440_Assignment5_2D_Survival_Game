using UnityEngine;
using UnityEngine.UI;

public class RecipeSlot : MonoBehaviour
{
    [Header("UI References")]
    public Image backgroundImage; // Assign the Image component of this GameObject (the slot background)
    public Button craftButton;
    public GameObject recipeItemPrefab; // Assign your RecipeItemDisplay_Prefab here

    [Header("State Colors")]
    public Color craftableColor = Color.green; // Example: Green when craftable
    public Color lockedColor = Color.red;    // Example: Red when locked

    // Internal data
    private Recipe currentRecipe;
    private CraftingManager craftingManager;
    private RecipeItemDisplay displayedItem;

    /// <summary>
    /// Fills this slot with recipe data and creates the visual item display.
    /// </summary>
    public void Setup(Recipe recipe, CraftingManager manager)
    {
        currentRecipe = recipe;
        craftingManager = manager;

        // --- SAFETY CHECKS ---
        if (recipe == null || recipe.outputItem == null)
        {
            Debug.LogError($"Recipe or its Output Item is null! Cannot setup slot.", this.gameObject);
            if (backgroundImage != null) backgroundImage.enabled = false; // Hide background if bad data
            if (craftButton != null) craftButton.interactable = false;
            if (displayedItem != null) Destroy(displayedItem.gameObject);
            return;
        }
        if (recipeItemPrefab == null)
        {
            Debug.LogError($"Recipe Item Prefab is not assigned on {this.gameObject.name}! Cannot display item.", this.gameObject);
            if (backgroundImage != null) backgroundImage.enabled = false;
            if (craftButton != null) craftButton.interactable = false;
            return;
        }
        if (backgroundImage == null)
        {
            Debug.LogError($"Background Image is not assigned on {this.gameObject.name}!", this.gameObject);
            // Don't return, but visuals won't update color
        }
        // --- END CHECKS ---


        // Create an instance of the recipe item display prefab
        GameObject itemGO = Instantiate(recipeItemPrefab, transform);
        displayedItem = itemGO.GetComponent<RecipeItemDisplay>();

        if (displayedItem != null)
        {
            displayedItem.InitializeItem(recipe.outputItem); // Use InitializeItem
        }
        else
        {
            Debug.LogError($"Instantiated Recipe Item Prefab on {this.gameObject.name} is missing the RecipeItemDisplay script!", itemGO);
        }

        if (craftButton != null)
        {
            craftButton.onClick.AddListener(OnCraftButtonClick);
        }
        else
        {
            Debug.LogError($"Craft Button is not assigned on {this.gameObject.name}!", this.gameObject);
        }

        UpdateVisuals();
    }

    /// <summary>
    /// Checks if the recipe is craftable and updates the background color and button state.
    /// </summary>
    public void UpdateVisuals()
    {
        // Prevent errors if setup failed or references are missing
        if (currentRecipe == null || craftingManager == null || craftButton == null || backgroundImage == null) return;

        bool canCraft = craftingManager.CanCraft(currentRecipe);
        craftButton.interactable = canCraft;

        // --- UPDATED VISUAL LOGIC ---
        // Change the BACKGROUND color based on craftability
        backgroundImage.color = canCraft ? craftableColor : lockedColor;

        // Keep the item icon fully visible regardless of state (optional)
        // If you want the icon to fade slightly when locked, uncomment the next lines
        /*
        if (displayedItem != null && displayedItem.itemIconImage != null)
        {
             displayedItem.itemIconImage.color = canCraft ? Color.white : new Color(0.7f, 0.7f, 0.7f, 0.8f); // Slightly faded white
        }
        */
    }

    /// <summary>
    /// Called when the player clicks the button.
    /// </summary>
    private void OnCraftButtonClick()
    {
        if (currentRecipe != null && craftingManager != null)
        {
            craftingManager.Craft(currentRecipe);
        }
    }
}

