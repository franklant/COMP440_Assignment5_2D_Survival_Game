using UnityEngine;
using UnityEngine.UI;
using TMPro; // Use this for modern text (recommended)

public class RecipeItem : MonoBehaviour
{
    [Header("UI")]
    public Image image;
    public TextMeshProUGUI countText; // Optional: for output quantity

    /// <summary>
    /// Sets the display with an item's data.
    /// </summary>
    public void Initialize(Item item)
    {
        if (item != null)
        {
            image.sprite = item.itemIcon;
            
            // This part is optional, but useful if a recipe creates more than one item
            if (countText != null)
            {
                countText.text = "1"; // Or based on recipe output quantity
                countText.gameObject.SetActive(false); // Hide if count is 1
            }
        }
    }
}