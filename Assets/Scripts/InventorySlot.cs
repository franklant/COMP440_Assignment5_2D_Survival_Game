using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    [Header("UI References")]
    public Image image;

    [Header("Selection Colors")]
    public Color selectedColor;
    public Color notSelectedColor;

    private void Awake()
    {
        Deselect();
    }

    public void Select()
    {
        image.color = selectedColor;
    }

    public void Deselect()
    {
        image.color = notSelectedColor;
    }

    // --- UPDATED OnDrop Method ---
    public void OnDrop(PointerEventData eventData)
    {
        // Check if this slot is empty
        if (transform.childCount == 0)
        {
            // Get the InventoryItem script from the object being dragged
            GameObject droppedObject = eventData.pointerDrag;
            InventoryItem inventoryItem = droppedObject.GetComponent<InventoryItem>();

            // If it's a valid InventoryItem...
            if (inventoryItem != null)
            {
                // Set its parentAfterDrag to THIS slot. This tells the item
                // that the drop was successful and where its new home is.
                inventoryItem.parentAfterDrag = transform;

                // --- ADDED THIS LINE ---
                // Immediately set the parent now, so OnEndDrag knows it worked.
                inventoryItem.transform.SetParent(transform);
                inventoryItem.transform.localPosition = Vector3.zero; // Center it

                Debug.Log($"Dropped {inventoryItem.itemData.itemName} onto {this.gameObject.name}");
            }
        }
    }
}

