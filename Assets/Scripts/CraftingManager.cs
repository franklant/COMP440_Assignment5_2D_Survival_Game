using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    // A reference to the player's inventory
    private Inventory inventory;
    
    // The crafting station the player is currently at
    private CraftingStation currentStation = CraftingStation.None;

    void Start()
    {
        // Find the inventory in the scene
        inventory = FindObjectOfType<Inventory>();
        if (inventory == null)
        {
            Debug.LogError("CraftingManager could not find the Inventory in the scene!");
        }
    }

    /// <summary>
    /// Allows other scripts (like the Player) to tell the manager which station we are at.
    /// </summary>
    /// <param name="newStation">The station the player has entered or exited.</param>
    public void SetCurrentCraftingStation(CraftingStation newStation)
    {
        currentStation = newStation;
    }

    /// <summary>
    /// Checks if a recipe can be crafted with the current inventory and station.
    /// </summary>
    public bool CanCraft(Recipe recipe)
    {
        // 1. Check if the player is at the required station
        if (recipe.requiredStation != CraftingStation.None && recipe.requiredStation != currentStation)
        {
            // We are not at the right station, so we can't craft.
            return false;
        }

        // 2. Check if the player has all the required ingredients
        foreach (Ingredient ingredient in recipe.ingredients)
        {
            if (!inventory.HasItem(ingredient.item, ingredient.quantity))
            {
                // We are missing an ingredient, so we can't craft.
                return false;
            }
        }

        // If we passed both checks, we can craft!
        return true;
    }

    /// <summary>
    /// Attempts to craft an item from a recipe.
    /// </summary>
    public void Craft(Recipe recipe)
    {
        // First, check if we are able to craft this item.
        if (CanCraft(recipe))
        {
            // Start the crafting process, which includes a timer
            StartCoroutine(CraftingCoroutine(recipe));
        }
        else
        {
            // If we can't craft, log a message to the console for debugging.
            Debug.LogWarning($"Failed to craft {recipe.outputItem.itemName}. Check requirements.");
        }
    }

    /// <summary>
    /// A coroutine that handles the crafting timer and resource consumption.
    /// </summary>
    private IEnumerator CraftingCoroutine(Recipe recipe)
    {
        Debug.Log($"Crafting {recipe.outputItem.itemName}...");

        // Wait for the specified crafting time
        yield return new WaitForSeconds(recipe.craftingTime);

        // After waiting, consume the ingredients
        foreach (Ingredient ingredient in recipe.ingredients)
        {
            inventory.RemoveItem(ingredient.item, ingredient.quantity);
        }

        // Add the crafted item(s) to the inventory
        inventory.AddItem(recipe.outputItem, recipe.outputQuantity);

        Debug.Log($"Successfully crafted {recipe.outputItem.itemName}!");
    }
}

