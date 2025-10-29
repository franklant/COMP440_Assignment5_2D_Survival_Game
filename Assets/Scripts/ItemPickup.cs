using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemPickup : MonoBehaviour
{
    [Header("Item Data")]
    [Tooltip("The ScriptableObject for the item to be picked up.")]
    public Item itemData;

    [Tooltip("How many of this item to give. (NOTE: Your InventoryManager must support this!)")]
    public int quantity = 1;

    [Header("Pickup Settings")]
    [Tooltip("The tag of the object that can pick this up.")]
    public string playerTag = "Player";

    private void Awake()
    {
        // Ensure the collider is set to "Is Trigger" so OnTriggerEnter2D works
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object that entered the trigger has the correct tag
        if (other.CompareTag(playerTag))
        {
            // Try to get the InventoryManager from the player object
            InventoryManager inventory = other.GetComponent<InventoryManager>();

            if (inventory == null)
            {
                Debug.LogError($"Pickup failed: The object with tag '{playerTag}' does not have an InventoryManager script.");
                return;
            }

            // --- ⭐ FIXED LINE ---
            // Try to add the item to the inventory.
            // We call AddItem *once* per quantity.
            bool pickedUp = false;
            for(int i = 0; i < quantity; i++)
            {
                pickedUp = inventory.AddItem(itemData);
                
                // If the inventory becomes full, stop trying to add more
                if (!pickedUp)
                {
                    Debug.Log("Inventory is full. Could not pick up all items.");
                    break;
                }
            }
            // --------------------

            // If *at least one* item was successfully added
            if (pickedUp)
            {
                // Destroy this item from the game world
                Destroy(gameObject);
            }
        }
    }
}