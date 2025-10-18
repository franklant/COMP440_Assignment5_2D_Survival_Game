using UnityEditor.Experimental.GraphView;
using UnityEngine;

// NOTE: Removed UnityEngine.AI since we are not using NavMeshAgent in 2D.

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
    void Update()
    {
        float distanceToTarget = (transform.position - target.transform.position).magnitude;
        Vector3 distanceToTargetRaw = (transform.position - target.transform.position);
        // Debug.Log("Distance: " + distanceToTargetRaw);

        if (distanceToTarget <= detectionRadius && distanceToTarget > stoppingDistance && !isAttacked)
        {
            Vector3 direction = (transform.position - target.transform.position).normalized;

            myRigidBody.linearVelocity = -direction * movementSpeed;

            // 4. Optional: Rotate to face target (2D rotation)
            // LookAt is generally not used in 2D. We calculate the angle instead:
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            // Debug.Log("Angle" + angle.ToString());
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
        
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
