using UnityEngine;

/// <summary>
/// A temporary script for testing the crafting system with keyboard inputs.
/// </summary>
public class CraftingTester : MonoBehaviour
{
    // --- REFERENCES TO OTHER SCRIPTS ---
    private Inventory inventory;
    private CraftingManager craftingManager;

    // --- ASSETS TO TEST ---
    // We link these in the Inspector so we can test with specific items/recipes.
    public Item woodItem;
    public Item stoneItem;
    public Recipe axeRecipe;

    void Start()
    {
        // Find the other manager scripts in the scene when the game starts.
        inventory = FindObjectOfType<Inventory>();
        craftingManager = FindObjectOfType<CraftingManager>();
    }

    void Update()
    {
        // --- ADD ITEM KEYS ---
        // Press '1' to add 5 wood to the inventory.
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            inventory.AddItem(woodItem, 5);
        }

        // Press '2' to add 5 stone to the inventory.
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            inventory.AddItem(stoneItem, 5);
        }


        // --- CRAFTING KEY ---
        // Press 'C' to attempt to craft the axe recipe.
        if (Input.GetKeyDown(KeyCode.C))
        {
            craftingManager.Craft(axeRecipe);
        }
        
        // --- DEBUG STATION KEYS ---
        // Press 'B' to simulate being at a Workbench
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("Moved to Workbench.");
            // --- THIS IS THE CORRECTED LINE ---
            craftingManager.SetCurrentCraftingStation(CraftingStation.Workbench);
        }

        // Press 'N' to simulate being away from any station
        if (Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log("Left station area.");
            // --- THIS IS THE CORRECTED LINE ---
            craftingManager.SetCurrentCraftingStation(CraftingStation.None);
        }
    }
}

