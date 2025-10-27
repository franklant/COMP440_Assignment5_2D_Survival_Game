using UnityEngine;
using UnityEngine.UI;
// Note: No EventSystems or Text needed if just displaying icon

public class RecipeItemDisplay : MonoBehaviour 
{
    [Header("UI")]
    public Image image; // Reference to the Image component that shows the icon

    // Removed Data section (itemData) as it's set via Initialize
    // Removed count and countText
    // Removed Start() method - initialization happens via Initialize
    // Removed RefreshCount()
    // Removed drag-and-drop logic (interfaces and methods)

    /// <summary>
    /// Sets the item icon to display. Called by RecipeSlot.
    /// </summary>
    public void InitializeItem(Item newItem)
    {
        if (newItem != null)
        {
            // Directly set the sprite on the Image component
            image.sprite = newItem.itemIcon;
        }
        else
        {
            // Optional: Clear the image if no item is provided
            image.sprite = null; 
        }
    }
}
