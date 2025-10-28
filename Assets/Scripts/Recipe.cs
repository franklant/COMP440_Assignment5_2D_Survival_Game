using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ingredient
{
    public Item item;
    public int quantity;
}

// An Enum to define the different types of crafting stations we can have
public enum CraftingStation
{
    None,       // For crafting by hand in the inventory
    Campfire,
    Workbench
}

[CreateAssetMenu(fileName = "New Recipe", menuName = "Survival Game/Recipe")]
public class Recipe : ScriptableObject
{
    [Header("Recipe Details")]
    public Item outputItem;
    public int outputQuantity = 1;

    [Header("Requirements")]
    public List<Ingredient> ingredients;
    public float craftingTime = 1f; // Crafting time in seconds
    public CraftingStation requiredStation;
}