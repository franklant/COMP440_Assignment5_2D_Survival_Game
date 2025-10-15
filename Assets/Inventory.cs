using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // This makes sure there's only one instance of the inventory and provides a public point of access to it.
    public static Inventory instance;

    void Awake()
    {
        // If an instance already exists and it's not this one, destroy this one.
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            
            instance = this;
            
        }
    }



    public Dictionary<Item, int> items = new Dictionary<Item, int>();



    public void AddItem(Item itemToAdd, int quantity)
    {
        // If we already have the item, just increase its quantity.
        if (items.ContainsKey(itemToAdd))
        {
            items[itemToAdd] += quantity;
        }
        // Otherwise, add the new item to the dictionary.
        else
        {
            items.Add(itemToAdd, quantity);
        }

        // Print a message to the console to confirm the item was added.
        Debug.Log($"Added {quantity} {itemToAdd.itemName}(s). Total: {items[itemToAdd]}");
    }



    public void RemoveItem(Item itemToRemove, int quantity)
    {
        // Check if we even have the item in the first place.
        if (items.ContainsKey(itemToRemove))
        {
            // Reduce the quantity.
            items[itemToRemove] -= quantity;

            // If the quantity drops to 0 or below, remove the item from our dictionary completely.
            if (items[itemToRemove] <= 0)
            {
                items.Remove(itemToRemove);
                Debug.Log($"Removed all {itemToRemove.itemName}(s) from inventory.");
            }
            else
            {
                 Debug.Log($"Removed {quantity} {itemToRemove.itemName}(s). Remaining: {items[itemToRemove]}");
            }
        }
        else
        {
            Debug.LogWarning($"Tried to remove {itemToRemove.itemName}, but it's not in the inventory!");
        }
    }



    public bool HasItem(Item itemToCheck, int requiredQuantity)
    {
        // Check if we have the item and if the quantity is sufficient.
        if (items.ContainsKey(itemToCheck) && items[itemToCheck] >= requiredQuantity)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
