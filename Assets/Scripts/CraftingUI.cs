using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI; // Required for LayoutRebuilder

public class CraftingUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject recipeSlotPrefab;

    [Header("System References")]
    public CraftingManager craftingManager;

    [Header("Recipe Data")]
    public List<Recipe> allRecipes;

    private List<RecipeSlot> recipeSlots = new List<RecipeSlot>();

    // Start() is called only once. We'll leave it empty.
    void Start()
    {
        // ...
    }

    // --- ALL LOGIC MOVED HERE ---
    // OnEnable() is called EVERY time the panel is activated.
    void OnEnable()
    {
        // Safety checks for required references
        if (craftingManager == null)
        {
             Debug.LogError("Crafting Manager is NOT assigned on CraftingUI!", this.gameObject);
             return;
        }
         if (recipeSlotPrefab == null)
        {
             Debug.LogError("Recipe Slot Prefab is NOT assigned on CraftingUI!", this.gameObject);
             return;
        }

        // These functions will now run every time you open the window
        PopulateRecipeList();
        Debug.Log("Forcing initial UI update after populating.");
        UpdateAllButtons();
    }
    // ---

    void PopulateRecipeList()
    {
        // Clear children of THIS transform
        List<GameObject> childrenToDestroy = new List<GameObject>();
        foreach (Transform child in transform) // Target THIS object's children
        {
             if (child.GetComponent<RecipeSlot>() != null)
             {
                childrenToDestroy.Add(child.gameObject);
             }
        }
        foreach (GameObject child in childrenToDestroy)
        {
            Destroy(child);
        }
        recipeSlots.Clear();

        Debug.Log("--- POPULATING RECIPE UI ---");

        // Create a slot for each recipe
        foreach (Recipe recipe in allRecipes)
        {
            if (recipe == null)
            {
                Debug.LogWarning("Found a NULL recipe in the All Recipes list. Skipping.");
                continue;
            }

            // Instantiate as a child of THIS transform
            GameObject slotGO = Instantiate(recipeSlotPrefab, transform);
            RecipeSlot slotScript = slotGO.GetComponent<RecipeSlot>();

            if (slotScript != null)
            {
                slotScript.Setup(recipe, craftingManager);
                recipeSlots.Add(slotScript);
            }
            else
            {
                 Debug.LogError($"Instantiated Recipe Slot Prefab for recipe '{recipe.name}' is MISSING the RecipeSlot script!", slotGO);
            }
        }
         LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
    }


    public void UpdateAllButtons()
    {
        // This is the debug line you requested
        Debug.Log($"<color=cyan>UI REFRESH: {gameObject.name} is updating all buttons.</color>");

        if (recipeSlots == null)
        {
            Debug.LogError("recipeSlots list is null in UpdateAllButtons!");
            return;
        }

        foreach (RecipeSlot slot in recipeSlots)
        {
            if (slot != null) 
            {
                slot.UpdateVisuals();
            }
            else
            {
                 Debug.LogWarning("Found a null slot reference in recipeSlots list during UpdateAllButtons.");
            }
        }
    }

    public bool CanCraft(Recipe recipe)
    {
       if (craftingManager == null)
        {
            Debug.LogError("CraftingManager reference is null in CraftingUI.CanCraft!");
            return false;
        }
        return craftingManager.CanCraft(recipe);
    }
}