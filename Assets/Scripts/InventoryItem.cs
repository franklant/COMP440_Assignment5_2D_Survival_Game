using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Data")]
    public Item itemData;

    [Header("UI")]
    public Image image;
    public Text countText;

    [HideInInspector] public int count = 1;
    [HideInInspector] public Transform parentAfterDrag; // The slot it returns to if dropped outside

    // Flag to track if the drop was successful
    private bool dropSuccessful = false;

    void Start()
    {
        InitializeItem(itemData);
    }

    public void InitializeItem(Item newItem)
    {
        if (newItem != null)
        {
            itemData = newItem;
            image.sprite = itemData.itemIcon;
            RefreshCount();
        }
    }

    public void RefreshCount()
    {
        countText.text = count.ToString();
        bool textActive = count > 1;
        countText.gameObject.SetActive(textActive);
    }

    // --- Drag and Drop Logic ---

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin Drag: " + itemData.itemName);
        parentAfterDrag = transform.parent; // Remember the original slot
        transform.SetParent(transform.root); // Detach from the slot to follow mouse
        transform.SetAsLastSibling();
        image.raycastTarget = false; // Allow raycasts to pass through to slots underneath
        dropSuccessful = false; // Reset the flag
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition; // Item follows the mouse
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End Drag: " + itemData.itemName);

        // --- UPDATED LOGIC ---
        // The OnDrop method on the InventorySlot sets parentAfterDrag if successful.
        // If parentAfterDrag is still the original slot, the drop was outside.
        if (transform.parent == transform.root) // Check if it wasn't reparented by OnDrop
        {
             // If the drop wasn't on a valid slot, snap back to the original parent
             transform.SetParent(parentAfterDrag);
             // Ensure it's positioned correctly within the slot
             transform.localPosition = Vector3.zero;
        }

        image.raycastTarget = true; // Make the item graphic clickable/draggable again
    }
}

