using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(CraftingStationIdentifier))] // Good practice to ensure the identifier exists
public class CraftingStationTrigger : MonoBehaviour
{
    // No longer need a public variable for station type here!
    private CraftingStation stationType;

    // Drag your CraftingManager object here in the Inspector
    public CraftingManager craftingManager;

    // public GameObject craftingTable;

    void Start()
    {
        // Get the station type from the identifier script on this same object.
        stationType = GetComponent<CraftingStationIdentifier>().stationType;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Player entered {stationType} station area.");
            craftingManager.SetCurrentCraftingStation(stationType);
            //craftingTable.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Player exited {stationType} station area.");
            craftingManager.SetCurrentCraftingStation(CraftingStation.None);
            //craftingTable.SetActive(false);
        }
    }
}