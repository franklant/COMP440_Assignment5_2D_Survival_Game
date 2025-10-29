using UnityEngine;

public class TreeResource : MonoBehaviour
{
    [Header("Node Stats")]
    public float health = 3f;

    [Header("Loot")]
    public GameObject woodDropPrefab;
    public int dropAmount = 1;

    // --- ⭐ NEW AUDIO VARIABLES ---
    [Header("Audio")]
    [Tooltip("Sound to play on each successful hit")]
    public AudioClip hitSound;
    [Tooltip("Sound to play when the tree is destroyed")]
    public AudioClip breakSound;
    // --- ⭐ END NEW VARIABLES ---


    public void TakeDamage(float damage)
    {
        health -= damage;

        // --- ⭐ PLAY HIT SOUND ---
        if (hitSound != null)
        {
            // Play a "hit" sound at the tree's position
            AudioSource.PlayClipAtPoint(hitSound, transform.position, 0.7f); // 0.7f is volume
        }
        // --- ⭐ ------------------

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // --- ⭐ PLAY BREAK SOUND ---
        if (breakSound != null)
        {
            // Play a "break" sound at the tree's position
            AudioSource.PlayClipAtPoint(breakSound, transform.position);
        }
        // --- ⭐ --------------------

        // Spawn the loot
        if (woodDropPrefab != null)
        {
            for (int i = 0; i < dropAmount; i++)
            {
                Vector3 offset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
                Instantiate(woodDropPrefab, transform.position + offset, Quaternion.identity);
            }
        }

        // Destroy this resource node
        Destroy(gameObject);
    }
}