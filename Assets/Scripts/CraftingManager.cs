using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    // Assign these in the Inspector
    public InventoryManager inventoryManager;
    public CraftingUI craftingUI; // ⭐ Assign your CraftingUI panel here

    public List<Recipe> recipes;
    private CraftingStation currentStation = CraftingStation.None;

    void Start()
    {
        // ⭐ Safety checks
        if (craftingUI == null)
        {
            Debug.LogError("CraftingUI is NOT ASSIGNED on the CraftingManager!", this);
        }
        if (inventoryManager == null)
        {
             Debug.LogError("InventoryManager is NOT ASSIGNED on the CraftingManager!", this);
        }
         
         NotifyCraftingUI();
    }

    public void SetCurrentCraftingStation(CraftingStation newStation)
    {
        currentStation = newStation;
        NotifyCraftingUI(); 
    }

    public bool CanCraft(Recipe recipe)
    {
         if (inventoryManager == null) {
             Debug.LogError("InventoryManager reference is NULL in CanCraft!");
             return false;
         }
         if (recipe == null) {
              Debug.LogError("Attempting to check CanCraft for a NULL recipe!");
              return false;
         }
         
        if (recipe.requiredStation != CraftingStation.None && recipe.requiredStation != currentStation)
        {
            return false;
        }

        foreach (Ingredient ingredient in recipe.ingredients)
        {
            if (ingredient == null || ingredient.item == null) {
                Debug.LogError($"Recipe '{recipe.outputItem?.itemName ?? "Unknown"}' has a null ingredient or item!", recipe);
                return false;
            }
            if (!inventoryManager.HasItem(ingredient.item, ingredient.quantity))
            {
                return false; 
            }
        }
        
        return true;
    }


    public void Craft(Recipe recipe)
    {
        if (recipe == null) {
            Debug.LogError("Craft called with a NULL recipe!");
            return;
        }

        if (CanCraft(recipe))
        {
            StartCoroutine(CraftingCoroutine(recipe));
        }
        else
        {
            Debug.LogWarning($"Crafting failed because CanCraft returned false for {recipe.outputItem?.itemName ?? "Unknown"}.");
            
            // ⭐ Force UI update even if craft fails (to fix "stuck green" button)
            NotifyCraftingUI(); 
        }
    }

    private IEnumerator CraftingCoroutine(Recipe recipe)
    {
        if (recipe == null || recipe.outputItem == null) {
            Debug.LogError("CraftingCoroutine started with invalid recipe!");
            yield break;
        }

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
        
        // ⭐ Update UI after a successful craft
        NotifyCraftingUI();
    }

    // --- ⭐ MODIFIED HELPER TO NOTIFY UI ---
    // This now uses the direct reference and is public
    public void NotifyCraftingUI()
    {
        if (craftingUI != null)
        {
            craftingUI.UpdateAllButtons();
        }
        else
        {
            Debug.LogWarning("CraftingManager tried to notify UI, but CraftingUI reference is null.");
        }
    }
}