using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    [Header("Setup")]
    public List<InventorySlot> inventorySlots; // Assign your 7 hotbar slots in order (0-6)
    public GameObject inventoryItemPrefab;
    public int maxStackSize = 5;

    [Header("Runtime Info")]
    // --- UPDATED: Start at -1 to match screenshot logic ---
    public int selectedSlot = -1;

    // --- ADDED: Start() method to select initial slot ---
    void Start()
    {
        // Select the initial slot (slot 0) when the game starts
        ChangeSelectedSlot(0);
    }

    // --- UPDATED: Replicated Update logic from screenshot ---
    void Update()
    {
        // Check if any key was pressed this frame
        if (Input.inputString != null && Input.inputString.Length > 0)
        {
            // Try to parse the first character of the input as a number
            bool isNumber = int.TryParse(Input.inputString[0].ToString(), out int number);

            // Check if it was a number between 1 and 7 (inclusive)
            if (isNumber && number > 0 && number < 8) // Checks for keys 1, 2, 3, 4, 5, 6, 7
            {
                // Convert the number pressed (1-7) to a zero-based index (0-6)
                ChangeSelectedSlot(number - 1);
            }
        }
    }

    /// <summary>
    /// Adds an item to the inventory, handling stacking (up to maxStackSize) and finding empty slots.
    /// Returns true if the item was added, false if the inventory is full.
    /// </summary>
    public bool AddItem(Item item)
    {
        // First, try to find a slot that already has this item and can be stacked
        foreach (InventorySlot slot in inventorySlots)
        {
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null && itemInSlot.itemData == item && item.stackable && itemInSlot.count < maxStackSize)
            {
                itemInSlot.count++;
                itemInSlot.RefreshCount();
                Debug.Log($"Stacked {item.itemName} in an existing slot (now {itemInSlot.count}).");
                NotifyCraftingUI();
                return true;
            }
        }

        // If no stackable slot was found, find the first empty slot
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.transform.childCount == 0)
            {
                if (inventoryItemPrefab == null)
                {
                     Debug.LogError("InventoryItem Prefab is NOT ASSIGNED on InventoryManager!");
                     return false;
                }
                GameObject newItemGO = Instantiate(inventoryItemPrefab, slot.transform);
                InventoryItem inventoryItem = newItemGO.GetComponent<InventoryItem>();
                 if(inventoryItem == null)
                {
                    Debug.LogError("Instantiated prefab is MISSING the InventoryItem script!", newItemGO);
                    Destroy(newItemGO);
                    return false;
                }
                inventoryItem.InitializeItem(item);
                Debug.Log($"Placed {item.itemName} in a new slot.");
                NotifyCraftingUI();
                return true;
            }
        }

        Debug.LogWarning("Failed to add item. Inventory is full!");
        return false;
    }

    /// <summary>
    /// Removes a certain quantity of an item from the inventory.
    /// Returns true on success and false if not enough items were found.
    /// </summary>
    public bool RemoveItem(Item item, int quantityToRemove)
    {
        if (!HasItem(item, quantityToRemove))
        {
            Debug.LogWarning($"Failed to remove {item.itemName}; not enough in inventory.");
            return false;
        }

        int quantityLeftToRemove = quantityToRemove;
        for (int i = inventorySlots.Count - 1; i >= 0; i--)
        {
            InventoryItem itemInSlot = inventorySlots[i].GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null && itemInSlot.itemData == item)
            {
                if (itemInSlot.count > quantityLeftToRemove)
                {
                    itemInSlot.count -= quantityLeftToRemove;
                    itemInSlot.RefreshCount();
                    quantityLeftToRemove = 0;
                }
                else
                {
                    quantityLeftToRemove -= itemInSlot.count;
                    Destroy(itemInSlot.gameObject);
                }
                if (quantityLeftToRemove <= 0) break;
            }
        }

        Debug.Log($"Successfully removed {quantityToRemove} {item.itemName} from inventory.");
        NotifyCraftingUI();
        return true;
    }

    /// <summary>
    /// Correctly checks if the inventory contains enough of a specific item across all stacks.
    /// </summary>
    public bool HasItem(Item item, int requiredQuantity)
    {
        int countFound = 0;
        foreach (InventorySlot slot in inventorySlots)
        {
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null && itemInSlot.itemData == item)
            {
                countFound += itemInSlot.count;
            }
        }
        return countFound >= requiredQuantity;
    }

    /// <summary>
    /// Changes the currently selected inventory slot and updates visuals (Replicated Logic).
    /// </summary>
    public void ChangeSelectedSlot(int newValue)
    {
        // Make sure the new value is within the bounds of your hotbar
        if (newValue < 0 || newValue >= inventorySlots.Count)
        {
            return; // Invalid slot index
        }

        // --- UPDATED: Replicated logic from screenshot ---
        // Deselect the previously selected slot (visually), only if it was valid (>= 0)
        if (selectedSlot >= 0 && selectedSlot < inventorySlots.Count) // Add safety check
            inventorySlots[selectedSlot].Deselect();

        // Update the index
        selectedSlot = newValue;

        // Select the new slot (visually)
        if (selectedSlot >= 0 && selectedSlot < inventorySlots.Count) // Add safety check
            inventorySlots[selectedSlot].Select();
    }


    /// <summary>
    /// Returns the Item data from the currently selected slot.
    /// If 'useItem' is true, it also consumes one from the stack.
    /// </summary>
    public Item GetSelectedItem(bool useItem)
    {
        if (selectedSlot < 0 || selectedSlot >= inventorySlots.Count) return null;

        InventoryItem itemInSlot = inventorySlots[selectedSlot].GetComponentInChildren<InventoryItem>();
        if (itemInSlot != null)
        {
            Item itemData = itemInSlot.itemData;
            if (useItem)
            {
                itemInSlot.count--;
                itemInSlot.RefreshCount();
                if (itemInSlot.count <= 0)
                {
                    Destroy(itemInSlot.gameObject);
                }
                NotifyCraftingUI();
            }
            return itemData;
        }
        return null;
    }

    /// <summary>
    /// Helper method to find and notify the CraftingUI.
    /// </summary>
    private void NotifyCraftingUI()
    {
        Debug.Log("Attempting to notify CraftingUI...");
        CraftingUI craftingUIInstance = FindFirstObjectByType<CraftingUI>(); // Find it first
        if (craftingUIInstance != null)
        {
            Debug.Log("<color=yellow>Found CraftingUI instance! Calling UpdateAllButtons...</color>");
            craftingUIInstance.UpdateAllButtons(); // Call the method
        }
        else
        {
            // Don't log error if crafting UI might not always be active
            // Debug.LogError("Could not find active CraftingUI component in the scene!");
        }
    }
}

