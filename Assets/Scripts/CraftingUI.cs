using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI; // Required for LayoutRebuilder

public class CraftingUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject recipeSlotPrefab;
    // Removed: public Transform slotContainer;

    [Header("System References")]
    public CraftingManager craftingManager;

    [Header("Recipe Data")]
    public List<Recipe> allRecipes;

    private List<RecipeSlot> recipeSlots = new List<RecipeSlot>();

    void Start()
    {
        // Safety checks for required references
        // Removed check for slotContainer
        craftingManager = FindFirstObjectByType<CraftingManager>();
        
        if (craftingManager == null)
        {
             Debug.LogError("Crafting Manager is NOT assigned on CraftingUI!", this.gameObject);
             return;
        }
         if (recipeSlotPrefab == null)
        {
             Debug.LogError("Recipe Slot Prefab is NOT assigned on CraftingUI!", this.gameObject);
             return;
        }

        PopulateRecipeList();
        Debug.Log("Forcing initial UI update after populating.");
        UpdateAllButtons();
    }

    void PopulateRecipeList()
    {
        // --- UPDATED Clearing Logic ---
        // Clear children of THIS transform
        List<GameObject> childrenToDestroy = new List<GameObject>();
        foreach (Transform child in transform) // Target THIS object's children
        {
             if (child.GetComponent<RecipeSlot>() != null)
             {
                childrenToDestroy.Add(child.gameObject);
             }
        }
        foreach (GameObject child in childrenToDestroy)
        {
            Destroy(child);
        }
        recipeSlots.Clear();

        Debug.Log("--- POPULATING RECIPE UI ---");

        // Create a slot for each recipe
        foreach (Recipe recipe in allRecipes)
        {
            if (recipe == null)
            {
                Debug.LogWarning("Found a NULL recipe in the All Recipes list. Skipping.");
                continue;
            }

            // --- UPDATED Instantiation ---
            // Instantiate as a child of THIS transform
            GameObject slotGO = Instantiate(recipeSlotPrefab, transform);
            RecipeSlot slotScript = slotGO.GetComponent<RecipeSlot>();

            if (slotScript != null)
            {
                slotScript.Setup(recipe, craftingManager);
                recipeSlots.Add(slotScript);
            }
            else
            {
                 Debug.LogError($"Instantiated Recipe Slot Prefab for recipe '{recipe.name}' is MISSING the RecipeSlot script!", slotGO);
            }
        }
         // --- UPDATED Layout Rebuild ---
         // Target THIS object's RectTransform
         LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
    }


    public void UpdateAllButtons()
    {
        // Ensure recipeSlots is not null before iterating
        if (recipeSlots == null)
        {
            Debug.LogError("recipeSlots list is null in UpdateAllButtons!");
            return;
        }

        foreach (RecipeSlot slot in recipeSlots)
        {
            if (slot != null) // Add null check for individual slots
            {
                slot.UpdateVisuals();
            }
            else
            {
                 Debug.LogWarning("Found a null slot reference in recipeSlots list during UpdateAllButtons.");
            }
        }
    }


    /// <summary>
    /// A helper function that allows other scripts (like RecipeSlot)
    /// to check if a recipe can be crafted by passing the request to the CraftingManager.
    /// </summary>
    public bool CanCraft(Recipe recipe)
    {
       // --- FIXED IMPLEMENTATION ---
       // Add safety check for manager
        if (craftingManager == null)
        {
            Debug.LogError("CraftingManager reference is null in CraftingUI.CanCraft!");
            return false; // Return false if manager is missing
        }
        // Always return the result from the actual CraftingManager
        return craftingManager.CanCraft(recipe);
    }
}

