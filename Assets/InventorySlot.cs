using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData) // Corrected from PointerEvent to PointerEventData
    {
        Debug.Log("Item dropped on " + gameObject.name);
        // Check if the slot is empty before dropping
        if (transform.childCount == 0)
        {
            // Get the InventoryItem component from the object being dragged
            GameObject droppedObject = eventData.pointerDrag;
            InventoryItem inventoryItem = droppedObject.GetComponent<InventoryItem>();

            // Set the item's new parent to be this slot's transform
            inventoryItem.parentAfterDrag = transform;
        }
    }
}

