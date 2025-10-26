using UnityEngine;
using TMPro;

public class ResourceUI : MonoBehaviour
{
    public TextMeshProUGUI resourceText;
    private Inventory inventory;

    void Start() // ✅ must be capitalized
    {
        inventory = FindObjectOfType<Inventory>();

        if (inventory != null)
        {
            UpdateResourceText(inventory.getTotal());
        }
        else
        {
            Debug.LogWarning("No Inventory found in the scene!");
        }
    }

    public void UpdateResourceText(int total)
    {
        if (resourceText != null)
        {
            resourceText.text = "Resources: " + total;
        }
        else
        {
            Debug.LogWarning("Resource Text not assigned in the Inspector!");
        }
    }
}

