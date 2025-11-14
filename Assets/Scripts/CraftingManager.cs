using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    // Assign your main CraftingPanel here
    public CraftingUI craftingUI; 
    
    // --- THIS IS THE MISSING PIECE ---
    // Assign your new TradingPanel here
    public CraftingUI tradingUI; 
    // ---

    public List<Recipe> recipes;
    private CraftingStation currentStation = CraftingStation.None;
    
    // Assign your InventoryManager in the Inspector
    public InventoryManager inventoryManager;

    void Start()
    {
         if (inventoryManager == null)
         {
             Debug.LogError("InventoryManager is NOT ASSIGNED on the CraftingManager!", this);
         }
         
         // Notify the main crafting UI on start
         NotifyCraftingUI();
    }

    public void SetCurrentCraftingStation(CraftingStation newStation)
    {
        currentStation = newStation;
        NotifyCraftingUI(); 
    }

    public bool CanCraft(Recipe recipe)
    {
         if (inventoryManager == null) return false;
         if (recipe == null) return false;
         
        if (recipe.requiredStation != CraftingStation.None && recipe.requiredStation != currentStation)
        {
            return false;
        }

        foreach (Ingredient ingredient in recipe.ingredients)
        {
            if (ingredient == null || ingredient.item == null) return false;
            if (!inventoryManager.HasItem(ingredient.item, ingredient.quantity))
            {
                return false; 
            }
        }
        return true;
    }


    public void Craft(Recipe recipe)
    {
        if (recipe == null) return;

        if (CanCraft(recipe))
        {
            StartCoroutine(CraftingCoroutine(recipe));
        }
        else
        {
            Debug.LogWarning($"Crafting failed for {recipe.outputItem?.itemName ?? "Unknown"}.");
            NotifyCraftingUI(); 
        }
    }

    private IEnumerator CraftingCoroutine(Recipe recipe)
    {
        if (recipe == null || recipe.outputItem == null) yield break;

        Debug.Log($"Crafting {recipe.outputItem.itemName}...");
        yield return new WaitForSeconds(recipe.craftingTime); 

        // Consume Ingredients
        foreach (Ingredient ingredient in recipe.ingredients)
        {
            if (ingredient == null || ingredient.item == null) continue; 
            inventoryManager.RemoveItem(ingredient.item, ingredient.quantity);
        }

        // Add Output Item
        inventoryManager.AddItem(recipe.outputItem);
        Debug.Log($"<color=green>Successfully crafted {recipe.outputItem.itemName}!</color>");
        
        NotifyCraftingUI();
    }

    // --- THIS IS THE UPDATED FIX ---
    // This helper notifies BOTH UI panels
    public void NotifyCraftingUI()
    {
        // Update the main crafting UI if it's assigned AND open
        if (craftingUI != null && craftingUI.gameObject.activeInHierarchy)
        {
            craftingUI.UpdateAllButtons();
        }
        
        // Update the trading UI if it's assigned AND open
        if (tradingUI != null && tradingUI.gameObject.activeInHierarchy)
       {
            tradingUI.UpdateAllButtons();
        }
    }
}