using UnityEngine;
using System.Collections.Generic;
public class Inventory : MonoBehaviour
{
    private Dictionary<string, int> resources = new Dictionary<string, int>();
    public void addResources(string resourceName, int amount)
    {
        if (resources.ContainsKey(resourceName))
        {
            resources[resourceName] += amount;
        }
        else
        {
            resources[resourceName] = amount;
        }
    }

    public int getResourceAmount(string resourceName)
    {
        return resources.ContainsKey(resourceName) ? resources[resourceName] : 0;
    }

    public int getTotal()
    {
        int total = 0;
        foreach (var kvp in resources)
        {
            total += kvp.Value;
        }
        return total;
    }
}
