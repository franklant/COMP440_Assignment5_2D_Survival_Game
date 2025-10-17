using UnityEngine;
using System.Collections;
public class Resource2D : MonoBehaviour
{
    public string resourceName = "Wood";
    public int amount = 1;
    public bool isGathered = false;

    public string requiredTool = "Axe";
    public float respawnTime = 10f;
    public int hitsToGather = 3;
    private int currentHits = 0;

    private SpriteRenderer rend;
    private Collider2D col;
    
    [Header("Audio Settings")]
    public AudioClip hitSound;
    public AudioClip breakSound;

    private AudioSource audioSource;

    private void gather()
    {
        PlaySound(breakSound);
        Debug.Log($"Gathered {amount} {resourceName}");
        isGathered = true;
            

        rend.enabled = false;
        col.enabled = false;

        StartCoroutine(Respawn());
        
    }

    public void Hit(string tool)
    {
        if (isGathered) return;
        if (!canGather(tool))
        {
            Debug.LogWarning($"You need a {requiredTool} to hit this resource!");
            return;
        }
        currentHits++;
        PlaySound(hitSound);

        Debug.Log($"{resourceName} hit! ({currentHits}/{hitsToGather})");

        if (currentHits >= hitsToGather)
        {
            gather();
        }
    }

    public bool canGather(string tool)
    {
        return tool == requiredTool;
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);

        isGathered = false;
        rend.enabled = true;
        col.enabled = true;
        currentHits = 0;

        Debug.Log($"{resourceName} has respawned!");
    }


    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
        col = GetComponent<Collider2D>();
    }
    
    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(clip);
        }
    } 
}
