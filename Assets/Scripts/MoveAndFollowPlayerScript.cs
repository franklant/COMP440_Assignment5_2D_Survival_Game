using System;
using UnityEngine;
using Random = UnityEngine.Random; // Be specific because 'System' also has a Random

public class MoveAndFollowPlayerScript : MonoBehaviour
{
    private Transform target; // We will find this in Update()
    public Rigidbody2D myRigidBody;
    public BoxCollider2D myBoxCollider;
    public SpriteRenderer mySpriteRenderer;
    private PlayerScript playerScript; // ⭐ Reference to the player script

    [Header("Stats")]
    public float health = 5;
    public float movementSpeed;

    [Header("Detection")]
    public float detectionRadius = 1f;
    public float stoppingDistance = 0.5f; // ⭐ Increased slightly for easier attack range
    public bool isAttacked = false;

    [Header("AI State")]
    private int scatterState = 0;
    private int followState = 1; // Not explicitly used, but indicates intent
    private int moveState = 2;
    // private int patrolState = 3; // Not used
    public int currentState;
    private float yBounds;
    private float xBounds;
    public float moveDuration;
    private float currentMoveTime = 0;
    public bool shouldFollow = false;

    [Header("Loot Drops")]
    [Tooltip("The 'RawMeat' item prefab to spawn when killed")]
    public GameObject rawMeatPrefab;
    [Tooltip("How many pieces of meat to drop")]
    public int amountToDrop = 1;

    // --- ⭐ NEW ATTACK VARIABLES ---
    [Header("Attack Settings")]
    public float damageAmount = 5f; // How much damage to deal
    public float timeBetweenAttacks = 1.5f; // Seconds between attacks
    private float attackTimer; // Internal timer
    // --- ⭐ END NEW ATTACK VARIABLES ---

    Vector3 endPoint;
    Vector3 direction;


    void Start()
    {
        currentState = scatterState;

        SpriteRenderer backgroundPlaneRenderer = GameObject.FindGameObjectWithTag("Background").GetComponent<SpriteRenderer>();

        if (backgroundPlaneRenderer == null)
        {
            Debug.LogError("Cannot Access Backgrounnd Plane Object/Sprite Renderer!");
            // Consider disabling script if bounds are essential: enabled = false; return;
        } else
        {
            // Calculate bounds based on center pivot assumed
            yBounds = backgroundPlaneRenderer.bounds.extents.y;
            xBounds = backgroundPlaneRenderer.bounds.extents.x;
        }

        attackTimer = 0; // Start ready to attack if close enough
    }

    void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            target = playerObject.GetComponent<Transform>();
            playerScript = playerObject.GetComponent<PlayerScript>(); // ⭐ Get the PlayerScript reference
            if (playerScript == null)
            {
                Debug.LogError("Found Player object, but it's missing the PlayerScript!", this);
            }
        }
        // No need for an error here, Update will keep trying
    }


    void Update()
    {
        // --- Try to find player if target is missing ---
        if (target == null || playerScript == null) // ⭐ Also check playerScript
        {
            FindPlayer();
            // If still missing after trying, wait for next frame
            if (target == null || playerScript == null)
            {
                myRigidBody.linearVelocity = Vector2.zero; // Stop moving if no target
                return;
            }
        }
        // --- Player Found ---

        float distanceToTarget = (transform.position - target.position).magnitude;

        // --- Determine Action based on Distance ---
        if (distanceToTarget <= detectionRadius && !isAttacked)
        {
            shouldFollow = true; // Player is detected

            if (distanceToTarget > stoppingDistance)
            {
                // Move towards player
                currentState = moveState; // Or just set velocity directly
                direction = (target.position - transform.position).normalized; // Correct direction towards player
                myRigidBody.linearVelocity = direction * movementSpeed;
                RotateTowards(direction);
                attackTimer = timeBetweenAttacks; // Reset attack timer while moving closer
            }
            else
            {
                // Stop and Attack
                currentState = followState; // Represents being close/attacking
                myRigidBody.linearVelocity = Vector2.zero; // Stop moving
                direction = (target.position - transform.position).normalized; // Ensure facing player
                RotateTowards(direction);

                // Attack Cooldown
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
            // Player out of range or enemy was attacked - Scatter/Move randomly
            shouldFollow = false;
            attackTimer = 0; // Reset attack timer

            // Perform Scatter/Move logic only if not following
            if (currentState == scatterState)
            {
                scatter();
            }
            if (currentState == moveState)
            {
                move();
            }
            // If was following, switch back to scatter
            if (currentState == followState)
            {
                 currentState = scatterState;
                 myRigidBody.linearVelocity = Vector2.zero; // Stop chasing velocity
            }
        }

        // --- Health Check ---
        if (health <= 0)
        {
            Die(); // Handle death and loot drops
        }
    }

    public void scatter()
    {
        // Calculate random point within bounds
        endPoint = new Vector3(Random.Range(-xBounds, xBounds), Random.Range(-yBounds, yBounds), 0);
        direction = (endPoint - transform.position).normalized;
        // float distance = Vector3.Distance(endPoint, transform.position); // Not needed?

        // Basic obstacle check (optional, can make AI feel stuck)
        // RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 2f); // Short raycast
        // if (hit.collider != null && hit.collider.gameObject != gameObject)
        // {
        //     currentState = scatterState; // Try new direction if blocked
        // } else
        // {
            currentState = moveState; // If clear or no hit, start moving
            currentMoveTime = 0; // Reset move timer
        // }
    }

    public void move()
    {
        // Move towards the random endpoint for a set duration
        if (currentMoveTime < moveDuration)
        {
            myRigidBody.linearVelocity = direction * movementSpeed;
            RotateTowards(direction); // Face movement direction
            currentMoveTime += Time.deltaTime;
        }
        else
        {
            // Time's up, find a new point
            myRigidBody.linearVelocity = Vector2.zero; // Stop before scattering again
            currentState = scatterState;
        }
    }

    // This is no longer needed as follow logic is inside Update
    // public void follow() { ... }

    // --- ⭐ Renamed from follow() to AttackPlayer() ---
    void AttackPlayer()
    {
        // Check playerScript just in case
        if (playerScript != null)
        {
            Debug.Log($"{gameObject.name} attacks player for {damageAmount} damage!");
            playerScript.TakeDamage(damageAmount);
            // Optional: Play attack sound/animation
            // Example: GetComponent<Animator>().SetTrigger("Attack");
        }
        else
        {
            Debug.LogError("Enemy tried to attack, but playerScript reference is missing!", this);
        }
    }
    // --- ⭐ ------------------------------------- ---


    // --- ⭐ Extracted Rotation Logic ---
    void RotateTowards(Vector3 targetDirection)
    {
        // Calculate angle towards the target direction
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        // transform.rotation = Quaternion.Euler(0, 0, angle); // Snaps instantly

        // Optional: Smooth rotation (requires Quaternion.Slerp and rotationSpeed variable)
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle + 90f); // Adjust angle offset based on your sprite's default orientation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f); // 10f is rotation speed

        // Flipping logic might need adjustment based on sprite orientation and rotation method
        // This flipY logic assumes sprite faces right by default
        mySpriteRenderer.flipY = (angle > 90 || angle < -90);

    }
    // --- ⭐ -------------------------- ---


    // --- ⭐ Renamed from Update's health check ---
    void Die()
    {
        Debug.Log($"{gameObject.name} died.");
        // Drop loot
        if (rawMeatPrefab != null)
        {
            for (int i = 0; i < amountToDrop; i++)
            {
                Vector3 offset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
                Instantiate(rawMeatPrefab, transform.position + offset, Quaternion.identity);
            }
        }
        Destroy(gameObject); // Destroy the enemy
    }
    // --- ⭐ --------------------------------- ---


    void OnCollisionEnter2D(Collision2D collision)
    {
        // Simple obstacle bounce logic (might interfere with pathfinding)
        if (collision.collider.CompareTag("Obstacles") && currentState != followState) // Don't bounce if following player
        {
            // Reverse direction or pick a new scatter point
            currentState = scatterState;
            myRigidBody.linearVelocity = Vector2.zero; // Stop current movement
        }
    }

    // --- Public function to take damage (called by PlayerScript) ---
     public void TakeDamage(float damageAmount) // Needs to be public if called from PlayerScript
    {
        health -= damageAmount;
        Debug.Log($"{gameObject.name} took {damageAmount} damage, {health} health remaining.");
        // Optional: Trigger 'hit' animation, sound, or temporary flee state
        // isAttacked = true; // Example: Trigger flee state
        // Invoke("ClearIsAttacked", 1.0f); // Example: Recover after 1 second
    }

    // Example function if using isAttacked for fleeing
    // void ClearIsAttacked() { isAttacked = false; }
}