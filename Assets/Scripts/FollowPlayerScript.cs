using UnityEngine;

public class FollowPlayerScript : MonoBehaviour
{
    private Transform target;
    public Rigidbody2D myRigidBody;
    public SpriteRenderer mySpriteRenderer;
    public float health = 5;
    public float movementSpeed;
    public float detectionRadius = 1f;
    public float stoppingDistance = 0.2f;
    public bool isAttacked = false;

    // --- ⭐ NEW DAMAGE VARIABLES ---
    [Header("Attack Settings")]
    public float damageAmount = 5f; // How much damage to deal
    public float timeBetweenAttacks = 1.0f; // Seconds between attacks
    private float attackTimer; // Internal timer
    private PlayerScript playerScript; // Reference to the player's script
    // --- ⭐ END NEW VARIABLES ---

    void Start()
    {
        mySpriteRenderer.flipX = true;
        // ⭐ Find Player object and get both Transform and PlayerScript
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            target = playerObject.GetComponent<Transform>();
            playerScript = playerObject.GetComponent<PlayerScript>(); // Get the script too
        }
        else
        {
            Debug.LogError("FollowPlayerScript could not find the Player!", this);
            enabled = false; // Disable script if player not found
        }
        attackTimer = 0; // Start ready to attack
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        Debug.Log($"Enemy took {damageAmount} damage, {health} health remaining.");
        // isAttacked = true; // Maybe add knockback or flee logic here
    }

    void Update()
    {
        // Safety check if target or player script is somehow lost
        if (target == null || playerScript == null)
        {
            myRigidBody.linearVelocity = Vector3.zero;
            return;
        }

        float distanceToTarget = (transform.position - target.transform.position).magnitude;

        // --- ⭐ MODIFIED MOVEMENT/ATTACK LOGIC ---
        if (distanceToTarget <= detectionRadius && !isAttacked)
        {
            // Player is within detection range

            if (distanceToTarget > stoppingDistance)
            {
                // Move towards player if not too close
                Vector3 direction = (transform.position - target.transform.position).normalized;
                myRigidBody.linearVelocity = -direction * movementSpeed;
                RotateTowardsPlayer(direction);
                attackTimer = timeBetweenAttacks; // Reset attack timer if moving
            }
            else
            {
                // Stop moving, we are close enough to attack
                myRigidBody.linearVelocity = Vector3.zero;
                // Ensure facing player even when stopped
                Vector3 direction = (transform.position - target.transform.position).normalized;
                RotateTowardsPlayer(direction);


                // Attack Cooldown Logic
                attackTimer -= Time.deltaTime;
                if (attackTimer <= 0)
                {
                    AttackPlayer();
                    attackTimer = timeBetweenAttacks; // Reset cooldown
                }
            }
        }
        else
        {
            // Player is out of range or enemy is attacked, stop moving
            myRigidBody.linearVelocity = Vector3.zero;
            attackTimer = 0; // Reset timer if player runs away
        }
        // --- ⭐ END MODIFIED LOGIC ---

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    // --- ⭐ NEW HELPER FUNCTIONS ---
    void RotateTowardsPlayer(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        if (angle >= 90 || angle <= -90)
        {
            mySpriteRenderer.flipY = true;
        }
        else
        {
            mySpriteRenderer.flipY = false;
        }
    }

    void AttackPlayer()
    {
        Debug.Log("Enemy attacks player!");
        playerScript.TakeDamage(damageAmount); // Call TakeDamage on the player
        // Optional: Play attack animation or sound effect here
    }
    // --- ⭐ END NEW FUNCTIONS ---
}