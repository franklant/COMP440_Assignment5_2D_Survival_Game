using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    // Assign your InventoryManager in the Inspector
    public InventoryManager inventoryManager;

    // This can be used later if you have global recipes
    public List<Recipe> recipes;

    private CraftingStation currentStation = CraftingStation.None;

    void Start()
    {
        Debug.Log("On Start, CraftingManager's reference to InventoryManager is: " + inventoryManager);
         // Ensure UI reflects starting station (None)
         NotifyCraftingUI();
    }

    public void SetCurrentCraftingStation(CraftingStation newStation)
    {
        Debug.Log($"Setting Current Crafting Station to: {newStation}"); // Add this
        currentStation = newStation;
        NotifyCraftingUI(); // Notify UI when station changes
    }

    public bool CanCraft(Recipe recipe)
    {
         // --- SAFETY CHECK ---
         if (inventoryManager == null) {
             Debug.LogError("InventoryManager reference is NULL in CanCraft!");
             return false;
         }
         if (recipe == null) {
              Debug.LogError("Attempting to check CanCraft for a NULL recipe!");
              return false;
         }
         // --- END SAFETY CHECK ---


        // --- BREADCRUMB 1: Station Check ---
        Debug.Log($"CanCraft Check for '{recipe.outputItem?.itemName ?? "NULL Recipe Output"}': Required station='{recipe.requiredStation}', Current station='{currentStation}'.");
        if (recipe.requiredStation != CraftingStation.None && recipe.requiredStation != currentStation)
        {
            Debug.LogWarning("-> Station check FAILED.");
            return false;
        }

        // --- BREADCRUMB 2: Ingredient Check ---
        if(recipe.ingredients == null || recipe.ingredients.Count == 0) {
             Debug.LogWarning($"Recipe '{recipe.outputItem?.itemName ?? "Unknown"}' has no ingredients defined!");
             // Decide if recipes with no ingredients are craftable (usually true)
             // return true; // Or false depending on your game rules
        }

        foreach (Ingredient ingredient in recipe.ingredients)
        {
            if (ingredient == null || ingredient.item == null) {
                Debug.LogError($"Recipe '{recipe.outputItem?.itemName ?? "Unknown"}' has a null ingredient or item!", recipe);
                return false;
            }
            bool hasIngredient = inventoryManager.HasItem(ingredient.item, ingredient.quantity);

            // --- BREADCRUMB 3: Specific Ingredient Status ---
            Debug.Log($"CanCraft Ingredient Check: Need {ingredient.quantity} of '{ingredient.item.itemName}'. Player has enough: {hasIngredient}");
            if (!hasIngredient)
            {
                Debug.LogWarning("-> Ingredient check FAILED for " + ingredient.item.itemName);
                return false; // Exit immediately if one ingredient is missing
            }
        }

        // If we got through the station check and ALL ingredient checks passed:
        Debug.Log($"-> CanCraft PASSED for {recipe.outputItem?.itemName ?? "Unknown"}");
        return true;
    }


    public void Craft(Recipe recipe)
    {
        // Add safety check
        if (recipe == null) {
            Debug.LogError("Craft called with a NULL recipe!");
            return;
        }

        Debug.Log($"CraftingManager.Craft() called for {recipe.outputItem?.itemName ?? "Unknown"}. Running CanCraft check...");
        if (CanCraft(recipe))
        {
            StartCoroutine(CraftingCoroutine(recipe));
        }
        else
        {
            Debug.LogWarning($"Crafting failed because CanCraft returned false for {recipe.outputItem?.itemName ?? "Unknown"}.");
        }
    }

    private IEnumerator CraftingCoroutine(Recipe recipe)
    {
        // Add safety check
        if (recipe == null || recipe.outputItem == null) {
            Debug.LogError("CraftingCoroutine started with invalid recipe!");
            yield break; // Stop the coroutine
        }

        Debug.Log($"Crafting {recipe.outputItem.itemName}...");
        yield return new WaitForSeconds(recipe.craftingTime); // Use recipe-specific time

        // Consume Ingredients
        foreach (Ingredient ingredient in recipe.ingredients)
        {
            if (ingredient == null || ingredient.item == null) continue; // Skip bad ingredients

            Debug.Log($"Attempting to remove {ingredient.quantity} {ingredient.item.itemName} from inventory...");
            bool removed = inventoryManager.RemoveItem(ingredient.item, ingredient.quantity);
            if (!removed) {
                 Debug.LogError($"Failed to remove ingredient {ingredient.item.itemName} during crafting! Coroutine stopped.");
                 yield break; // Stop if ingredients couldn't be removed
            }
        }

        // Add Output Item
        inventoryManager.AddItem(recipe.outputItem);
        Debug.Log($"<color=green>Successfully crafted {recipe.outputItem.itemName}!</color>");
    }

    // --- HELPER TO NOTIFY UI ---
     private void NotifyCraftingUI()
    {
        // Debug.Log("Attempting to notify CraftingUI (from CraftingManager)..."); // Can be noisy
        CraftingUI craftingUIInstance = FindAnyObjectByType<CraftingUI>();
        if (craftingUIInstance != null)
        {
            // Debug.Log("<color=purple>Found CraftingUI instance (from CM)! Calling UpdateAllButtons...</color>"); // Can be noisy
            craftingUIInstance.UpdateAllButtons();
        }
    }
}

