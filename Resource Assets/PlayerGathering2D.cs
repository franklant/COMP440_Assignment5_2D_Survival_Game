using UnityEngine;

public class PlayerGathering2D : MonoBehaviour
{
    [Header("Gathering Settings")]
    public float gatherRange = 1.5f;
    public KeyCode gatherKey = KeyCode.X;

    [Header("Tool Settings")]
    public string equippedTool = "Axe";

    private Inventory inventory;
    private ResourceUI ui;

    private void Start()
    {
        ui = FindObjectOfType<ResourceUI>();
        inventory = FindObjectOfType<Inventory>();
    }

    private void Update()
    {
        HandleToolSwitching();

        if (Input.GetKeyDown(gatherKey))
        {
            TryGather();
        }
    }

    private void HandleToolSwitching()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            equippedTool = "Axe";
            Debug.Log("Equipped Axe");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            equippedTool = "Pickaxe";
            Debug.Log("Equipped Pickaxe");
        }
    }

    private void TryGather()
    {
    Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, gatherRange);
        foreach (Collider2D col in hits)
        {
            Resource2D resource = col.GetComponent<Resource2D>();
            if (resource != null)
            {
                resource.Hit(equippedTool);
                if (!resource.GetComponent<SpriteRenderer>().enabled)
                {
                    inventory.addResources(resource.resourceName, resource.amount);
                    ui.UpdateResourceText(inventory.getTotal());
                }
            return;
            }
        }
    Debug.Log("No resource in range to gather.");
    }

}

