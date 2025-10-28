using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    [Header("Setup")]
    public List<InventorySlot> inventorySlots; // Assign your 7 hotbar slots in order (0-6)
    public GameObject inventoryItemPrefab;
    public int maxStackSize = 5;

    [Tooltip("Drag your Player GameObject here. Must have the PlayerScript component.")]
    public PlayerScript player; // Assign your Player (with PlayerScript) in the Inspector

    [Header("Runtime Info")]
    public int selectedSlot = -1;

    void Start()
    {
        ChangeSelectedSlot(0);
    }

    void Update()
    {
        if (Input.inputString != null && Input.inputString.Length > 0)
        {
            bool isNumber = int.TryParse(Input.inputString[0].ToString(), out int number);
            if (isNumber && number > 0 && number < 8)
            {
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
        // First, try to find a stackable slot
        foreach (InventorySlot slot in inventorySlots)
        {
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null && itemInSlot.itemData == item && item.stackable && itemInSlot.count < maxStackSize)
            {
                itemInSlot.count++;
                itemInSlot.RefreshCount();
                NotifyCraftingUI();
                return true; // Item was stacked
            }
        }

        // If no stackable slot, find an empty slot
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.transform.childCount == 0)
            {
                if (inventoryItemPrefab == null)
                {
                     Debug.LogError("InventoryItem Prefab is NOT ASSIGNED on InventoryManager!");
                     return false; // Failed to add
                }
                GameObject newItemGO = Instantiate(inventoryItemPrefab, slot.transform);
                InventoryItem inventoryItem = newItemGO.GetComponent<InventoryItem>();
                 if(inventoryItem == null)
                {
                    Debug.LogError("Instantiated prefab is MISSING the InventoryItem script!", newItemGO);
                    Destroy(newItemGO);
                    return false; // Failed to add
                }
                inventoryItem.InitializeItem(item);
                NotifyCraftingUI();
                return true; // Item was added to new slot
            }
        }

        Debug.LogWarning("Failed to add item. Inventory is full!");
        return false; // <-- FIX: Default return if no slots are found
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
            return false; // Not enough items
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
        return true; // <-- FIX: Default return for success
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
        return countFound >= requiredQuantity; // <-- FIX: This line is correct, it's outside the loop.
    }

    /// <summary>
    /// Changes the currently selected inventory slot and updates visuals.
    /// </summary>
    public void ChangeSelectedSlot(int newValue)
    {
        if (newValue < 0 || newValue >= inventorySlots.Count)
        {
            return; 
        }

        if (selectedSlot >= 0 && selectedSlot < inventorySlots.Count)
            inventorySlots[selectedSlot].Deselect();

        selectedSlot = newValue;

        if (selectedSlot >= 0 && selectedSlot < inventorySlots.Count)
            inventorySlots[selectedSlot].Select();
            
        UpdatePlayerHeldItem();
    }

    /// <summary>
    /// Tells the PlayerScript to update its held item sprite and damage.
    /// </summary>
    private void UpdatePlayerHeldItem()
    {
        if (player == null)
        {
            if (Time.frameCount < 10)
                 Debug.LogError("Player is not assigned in the InventoryManager!");
            return;
        }
        
        if (selectedSlot < 0 || selectedSlot >= inventorySlots.Count)
        {
            player.UpdateHeldItem(null);
            player.UpdateCurrentDamage(1f); // Default fist damage
            return;
        }

        InventoryItem itemInSlot = inventorySlots[selectedSlot].GetComponentInChildren<InventoryItem>();

        if (itemInSlot != null && itemInSlot.itemData != null)
        {
            player.UpdateHeldItem(itemInSlot.itemData.heldSprite);

            // Check the item's ActionType. Only apply damage if it's an "Attack" item.
            if (itemInSlot.itemData.actionType == ActionType.Attack)
            {
                player.UpdateCurrentDamage(itemInSlot.itemData.damage);
            }
            else
            {
                player.UpdateCurrentDamage(1f); // Default damage for non-attack items
            }
        }
        else
        {
            player.UpdateHeldItem(null);
            player.UpdateCurrentDamage(1f); // Default fist damage for empty slot
        }
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
            return itemData; // Item was found
        }
        return null; // <-- FIX: Default return if slot is empty
    }

    /// <summary>
    /// Helper method to find and notify the CraftingUI.
    /// </summary>
    private void NotifyCraftingUI()
    {
        // FIX FOR WARNING: Use FindFirstObjectByType instead of FindObjectOfType
        CraftingUI craftingUIInstance = FindFirstObjectByType<CraftingUI>(); 
        if (craftingUIInstance != null)
        {
            craftingUIInstance.UpdateAllButtons();
        }
    }
}