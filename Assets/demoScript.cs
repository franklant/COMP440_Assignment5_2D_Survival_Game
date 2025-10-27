using UnityEngine;

public class demoScript : MonoBehaviour
{
    [Header("System References")]
    public InventoryManager inventoryManager;
    public CraftingManager craftingManager;

    [Header("Test Items")]
    // Make sure size is 3 and items (Wood, Stone, Raw Meat) are assigned
    public Item[] itemsToPickup;

    [Header("Crafting Shortcuts")]
    public Recipe axeRecipe;
    public Recipe stickRecipe;
    public Recipe cookedMeatRecipe;

    void Update()
    {
        // --- Item Spawning ---
        if (Input.GetKeyDown(KeyCode.K)) { PickupItem(0); } // Spawns Wood
        if (Input.GetKeyDown(KeyCode.V)) { PickupItem(1); } // Spawns Stone
        if (Input.GetKeyDown(KeyCode.R)) { PickupItem(2); } // Spawns Raw Meat

        // --- Crafting Shortcuts ---
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (axeRecipe != null)
            {
                Debug.Log("--- 'C' key pressed. Starting craft attempt for: " + axeRecipe.name + " ---");
                craftingManager.Craft(axeRecipe);
            }
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
             if (stickRecipe != null)
            {
                Debug.Log("--- 'P' key pressed. Starting craft attempt for: " + stickRecipe.name + " ---");
                craftingManager.Craft(stickRecipe);
            }
        }

        // --- Cooking ---
        if (Input.GetKeyDown(KeyCode.U))
        {
             if (cookedMeatRecipe != null)
            {
                Debug.Log("--- 'U' key pressed. Starting craft attempt for: " + cookedMeatRecipe.name + " ---");
                craftingManager.Craft(cookedMeatRecipe);
            }
        }

        // --- Crafting Station Toggles ---
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("Moved to Workbench.");
            if (craftingManager != null)
                craftingManager.SetCurrentCraftingStation(CraftingStation.Workbench);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log("Left station area.");
            if (craftingManager != null)
                craftingManager.SetCurrentCraftingStation(CraftingStation.None);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Moved to Campfire.");
            if (craftingManager != null)
                craftingManager.SetCurrentCraftingStation(CraftingStation.Campfire);
        }
    }

    // --- Helper Method with Debug Logs ---
    public void PickupItem(int id)
    {
        // --- ADDED LOG ---
        Debug.Log($"PickupItem called for ID: {id}. Trying to add item: {(id < itemsToPickup.Length && itemsToPickup[id] != null ? itemsToPickup[id].itemName : "INVALID ID or ITEM")}");

        if (id < itemsToPickup.Length && itemsToPickup[id] != null)
        {
            if (inventoryManager.AddItem(itemsToPickup[id])) // Call AddItem
            {
                Debug.Log($"Item '{itemsToPickup[id].itemName}' was successfully processed by AddItem.");
            }
            else
            {
                 Debug.LogWarning($"AddItem returned false for '{itemsToPickup[id].itemName}'. Inventory might be full or another issue occurred.");
            }
        }
        else
        {
             Debug.LogError($"Invalid item ID ({id}) or item not assigned in itemsToPickup list!");
        }
    }

    // --- REMOVED GetSelectedItem() as 'P' is now used for crafting ---

    // UseSelectedItem remains the same, but GetSelectedItem(true) needs InventoryManager update
    public void UseSelectedItem()
    {
        // This method assumes GetSelectedItem(true) exists and works in InventoryManager
        Item receivedItem = inventoryManager.GetSelectedItem(true); // 'true' means consume
        if (receivedItem != null)
        {
            Debug.Log("Used item: " + receivedItem.itemName);
        }
        else
        {
            Debug.Log("No item to use in selected slot.");
        }
    }
}
