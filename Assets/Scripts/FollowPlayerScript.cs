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

    void Start()
    {
        mySpriteRenderer.flipX = true;
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }
    
    // --- ⭐ NEW FUNCTION ADDED HERE ---
    /// <summary>
    /// Public function to allow other scripts to deal damage to this enemy.
    /// </summary>
    /// <param name="damageAmount">The amount of health to subtract.</param>
    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        Debug.Log($"Enemy took {damageAmount} damage, {health} health remaining.");
        
        // You could also set isAttacked = true here if you want
    }
    // ---------------------------------

    void Update()
    {
        float distanceToTarget = (transform.position - target.transform.position).magnitude;
        Vector3 distanceToTargetRaw = (transform.position - target.transform.position);
        
        if (distanceToTarget <= detectionRadius && distanceToTarget > stoppingDistance && !isAttacked)
        {
            Vector3 direction = (transform.position - target.transform.position).normalized;
            myRigidBody.linearVelocity = -direction * movementSpeed;

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
        else
        {
            myRigidBody.linearVelocity = Vector3.zero;
        }
        
        // This existing code will automatically handle death
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}