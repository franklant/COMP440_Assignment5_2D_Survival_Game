using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    [Header("Setup")]
    public List<InventorySlot> inventorySlots; // Assign your 7 hotbar slots in order (0-6)
    public GameObject inventoryItemPrefab;
    public int maxStackSize = 5;
    public CraftingManager craftingManager; // ⭐ Assign this in the Inspector

    [Tooltip("Drag your Player GameObject here. Must have the PlayerScript component.")]
    public PlayerScript player; // Assign your Player (with PlayerScript) in the Inspector

    [Header("Runtime Info")]
    public int selectedSlot = -1;

    void Start()
    {
        ChangeSelectedSlot(0);

        // ⭐ Safety check for the new variable
        if (craftingManager == null)
        {
            Debug.LogWarning("CraftingManager is not assigned on the InventoryManager!", this);
        }
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
                UpdatePlayerHeldItem(); 
                return true;
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
                NotifyCraftingUI();
                UpdatePlayerHeldItem(); 
                return true;
            }
        }

        Debug.LogWarning("Failed to add item. Inventory is full!");
        return false; 
    }

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

                bool itemWasDestroyed = false; 

                if (itemInSlot.count <= 0)
                {
                    Destroy(itemInSlot.gameObject);
                    itemWasDestroyed = true;
                }

                NotifyCraftingUI();
                
                if (itemWasDestroyed)
                {
                    if (player != null)
                    {
                        player.UpdateHeldItem(null);
                        player.UpdateCurrentDamage(1f);
                    }
                }
                else
                {
                    UpdatePlayerHeldItem(); 
                }
            }
            return itemData;
        }
        return null;
    }
    
    // --- ⭐ MODIFIED HELPER TO NOTIFY UI ---
    // This now uses the direct reference instead of FindFirstObjectByType
    private void NotifyCraftingUI()
    {
        if (craftingManager != null)
        {
            craftingManager.NotifyCraftingUI();
        }
        else
        {
            // This will only show if you forget to link it in the Inspector
            Debug.LogWarning("InventoryManager can't find CraftingManager to notify UI!");
        }
    }
}