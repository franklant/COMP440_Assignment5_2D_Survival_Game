using UnityEngine;

public class CraftingTester : MonoBehaviour
{
    [Header("System References")]
    public CraftingManager craftingManager;
    public InventoryManager inventoryManager; // Make sure this is your main inventory script

    [Header("Assets to Test")]
    public Recipe woodenAxeRecipe;
    public Item woodItem;
    public Item stoneItem;

    void Start()
    {
        // Automatically find managers if they aren't assigned in the Inspector
        if (craftingManager == null)
            craftingManager = FindObjectOfType<CraftingManager>();

        if (inventoryManager == null)
            inventoryManager = FindObjectOfType<InventoryManager>();
    }

    void Update()
    {
        // --- CRAFTING KEY ---
        // Press 'C' to attempt to craft the Wooden Axe.
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (woodenAxeRecipe != null)
            {
                Debug.Log("Attempting to craft Wooden Axe...");
                craftingManager.Craft(woodenAxeRecipe);
            }
            else
            {
                Debug.LogError("Wooden Axe Recipe is not assigned in the CraftingTester Inspector!");
            }
        }

        // --- HELPER KEYS FOR TESTING ---
        // Press 'L' to add 5 wood.
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Adding 5 Wood.");
            for (int i = 0; i < 5; i++) { inventoryManager.AddItem(woodItem); }
        }

        // Press 'M' to add 5 stone.
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("Adding 5 Stone.");
            for (int i = 0; i < 5; i++) { inventoryManager.AddItem(stoneItem); }
        }
        
        // Press 'B' to simulate being at a Workbench.
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("Moved to Workbench.");
            craftingManager.SetCurrentCraftingStation(CraftingStation.Workbench);
        }

        // Press 'N' to simulate being away from any station.
        if (Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log("Left station area.");
            craftingManager.SetCurrentCraftingStation(CraftingStation.None);
        }
    }
}