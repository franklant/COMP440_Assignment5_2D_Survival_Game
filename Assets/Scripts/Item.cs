using UnityEngine;
using UnityEngine.Tilemaps; // Added for TileBase functionality

// These enums define the different categories for your items
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

    [Header("Gameplay Properties")]
    public ItemType type;
    public ActionType actionType;
    public TileBase tile; // The tile to place if it's a building block
    
    [Header("Inventory Properties")]
    public bool stackable = true;
}