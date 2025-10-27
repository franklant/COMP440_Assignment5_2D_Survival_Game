using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResourceType2D
{
    public string name = "Tree";
    public GameObject[] prefabs;       // Sprite prefabs
    public int count = 50;             // How many to spawn
    public float minSpacing = 2f;      // Prevent overlap
    public float rarity = 1f;          // Optional weighting
    public LayerMask groundLayer;      // Optional collider layer
    public float groundOffset = 0f;    // Offset above collider
}

public class ProceduralResourceGenerator2D : MonoBehaviour
{
    [Header("Global Area Settings")]
    public Vector2 areaSize = new Vector2(100, 100);
    public bool generateOnStart = true;
    public bool randomRotation = true;

    [Header("Resource Types")]
    public List<ResourceType2D> resourceTypes = new List<ResourceType2D>();

    private List<GameObject> spawnedResources = new List<GameObject>();

    void Start()
    {
        if (generateOnStart)
            GenerateAllResources();
    }

    [ContextMenu("Generate Resources Now")]
    public void GenerateAllResources()
    {
        ClearExistingResources();

        foreach (var resourceType in resourceTypes)
        {
            SpawnResourceType(resourceType);
        }

        Debug.Log($"[Generator2D] Spawned total {spawnedResources.Count} resources.");
    }

    void SpawnResourceType(ResourceType2D type)
    {
        int spawned = 0;
        int safety = 0;

        while (spawned < type.count && safety < type.count * 20)
        {
            safety++;

            // Random position within area bounds
            Vector2 randomPos = (Vector2)transform.position + new Vector2(
                Random.Range(-areaSize.x / 2, areaSize.x / 2),
                Random.Range(-areaSize.y / 2, areaSize.y / 2)
            );

            // Optional: adjust Y to rest on ground collider
            Vector2 spawnPos = GetGroundPosition(randomPos, type);

            if (spawnPos == Vector2.positiveInfinity)
                continue;

            // Spacing check
            bool tooClose = false;
            foreach (var obj in spawnedResources)
            {
                if (Vector2.Distance(obj.transform.position, spawnPos) < type.minSpacing)
                {
                    tooClose = true;
                    break;
                }
            }

            if (tooClose) continue;

            // Pick prefab + rotation
            GameObject prefab = type.prefabs[Random.Range(0, type.prefabs.Length)];
            Quaternion rotation = randomRotation
                ? Quaternion.Euler(0, 0, Random.Range(0f, 360f))
                : prefab.transform.rotation;

            GameObject instance = Instantiate(prefab, spawnPos, rotation);
            instance.name = $"{type.name}_{spawned}";
            spawnedResources.Add(instance);
            spawned++;
        }

        Debug.Log($"[Generator2D] Spawned {spawned} {type.name}(s).");
    }

    Vector2 GetGroundPosition(Vector2 origin, ResourceType2D type)
    {
        // Optionally drop to the nearest ground collider below
        RaycastHit2D hit = Physics2D.Raycast(origin + Vector2.up * 10f, Vector2.down, 20f, type.groundLayer);

        if (hit.collider != null)
            return hit.point + Vector2.up * type.groundOffset;

        // If no ground detected, return original position
        return origin;
    }

    [ContextMenu("Clear Spawned Resources")]
    public void ClearExistingResources()
    {
        foreach (var res in spawnedResources)
        {
            if (res != null)
                DestroyImmediate(res);
        }

        spawnedResources.Clear();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(areaSize.x, areaSize.y, 0f));
    }
}
