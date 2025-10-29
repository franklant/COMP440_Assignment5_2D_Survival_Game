using UnityEngine;
using UnityEngine.Tilemaps;

public enum ItemType
{
    BuildingBlock,
    Tool
}

public enum ActionType
{
    Attack, 
    Mine
}

[CreateAssetMenu(fileName = "New Item", menuName = "Survival Game/Item")]
public class Item : ScriptableObject
{
    [Header("General Info")]
    public string itemName;
    public Sprite itemIcon;
    public Sprite heldSprite; 

    [Header("Gameplay Properties")]
    public ItemType type;
    public ActionType actionType;
    public TileBase tile; 
    
    // --- ⭐ NEW FIELD ADDED HERE ---
    [Tooltip("Default damage is 1 (fist). Only applies if ActionType is Attack.")]
    public float damage = 1f; 
    // -----------------------------
    
    [Header("Inventory Properties")]
    public bool stackable = true;

    
    [Header("Food Properties")]
    [Tooltip("Can this item be eaten?")]
    public bool isFood = false;
    [Tooltip("How much hunger this item restores when eaten.")]
    public float hungerRestore = 0f;
    [Tooltip("How much health this item restores. Can be negative (e.g., for raw meat).")]
    public float healthRestore = 0f;
    // ---------------------------------
}