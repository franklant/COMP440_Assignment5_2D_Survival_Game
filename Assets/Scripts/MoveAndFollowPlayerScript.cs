using System;
using UnityEngine;
using Random = UnityEngine.Random; // ⭐ Be specific because 'System' also has a Random

public class MoveAndFollowPlayerScript : MonoBehaviour
{
    private Transform target; // We will find this in Update()
    public Rigidbody2D myRigidBody;
    public BoxCollider2D myBoxCollider;
    public SpriteRenderer mySpriteRenderer;
    
    [Header("Stats")]
    public float health = 5;
    public float movementSpeed;
    
    [Header("Detection")]
    public float detectionRadius = 1f;
    public float stoppingDistance = 0.2f;
    public bool isAttacked = false;
    
    [Header("AI State")]
    private int scatterState = 0;
    private int followState = 1;
    private int moveState = 2;
    private int patrolState = 3;
    public int currentState;
    private float yBounds;
    private float xBounds;
    public float moveDuration;
    private float currentMoveTime = 0;
    public bool shouldFollow = false;

    // ⭐ --- NEW LOOT VARIABLES --- ⭐
    [Header("Loot Drops")]
    [Tooltip("The 'RawMeat' item prefab to spawn when killed")]
    public GameObject rawMeatPrefab;
    [Tooltip("How many pieces of meat to drop")]
    public int amountToDrop = 1;
    // ⭐ -------------------------- ⭐

    Vector3 endPoint;
    Vector3 direction;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentState = scatterState;

        SpriteRenderer backgroundPlaneRenderer = GameObject.FindGameObjectWithTag("Background").GetComponent<SpriteRenderer>();
        
        if (backgroundPlaneRenderer == null)
        {
            Debug.LogError("Cannot Access Backgrounnd Plane Object/Sprite Renderer!");
        } else
        {
            yBounds = backgroundPlaneRenderer.size.y;
            xBounds = backgroundPlaneRenderer.size.x;
        }
    }

    // NEW FUNCTION to find the player safely
    void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            target = playerObject.GetComponent<Transform>();
        }
    }


    // Update is called once per frame
    void Update()
    {
        // --- MODIFIED SECTION ---
        // If we don't have a target...
        if (target == null)
        {
            FindPlayer(); // ...try to find one.

            // If we still don't have one, stop running this Update.
            // We'll try again next frame.
            if (target == null)
            {
                return; 
            }
        }
        // --- END MODIFIED SECTION ---

        if (currentState == scatterState && !shouldFollow)
        {
            scatter();
        }
        if (currentState == moveState && !shouldFollow)
        {
            move();
        }
        if (shouldFollow)
        {
            follow();
        }

        Debug.DrawRay(transform.position, direction, Color.red);

        float distanceToTarget = (transform.position - target.transform.position).magnitude;
        Vector3 distanceToTargetRaw = (transform.position - target.transform.position);
        
        if (distanceToTarget <= detectionRadius && distanceToTarget > stoppingDistance && !isAttacked)
        {
            shouldFollow = true;
            direction = Vector3.zero;
        }
        else
        {
            shouldFollow = false;
        }

        // ⭐ --- MODIFIED HEALTH/DEATH LOGIC --- ⭐
        if (health <= 0)
        {
            // Drop loot before destroying
            if (rawMeatPrefab != null)
            {
                for (int i = 0; i < amountToDrop; i++)
                {
                    // Spawn it with a slight random offset
                    Vector3 offset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
                    Instantiate(rawMeatPrefab, transform.position + offset, Quaternion.identity);
                }
            }
            
            Destroy(gameObject); // Now destroy the bear
        }
        // ⭐ ----------------------------------- ⭐
    }

    public void scatter()
    {
        endPoint = new Vector3(Random.Range(-xBounds, xBounds), Random.Range(-yBounds, yBounds), 0);
        direction = (endPoint - transform.position).normalized;
        float distance = Vector3.Distance(endPoint, transform.position);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction);

        if (hit.collider != null) // raycast has hit an object
        {
            if (hit.collider.name != myBoxCollider.name)
            {
                Debug.Log(hit.collider.name);
                currentState = scatterState;
            } else
            {
                currentState = moveState;
            }
            
        } else
        {
            currentState = moveState;
        }
    }

    public void move()
    {
        if (currentMoveTime < moveDuration && currentState != followState)
        {
            myRigidBody.linearVelocity = direction * movementSpeed;
            currentMoveTime += Time.deltaTime;
        }
        else
        {
            currentMoveTime = 0;
            currentState = scatterState;
        }
    }
    public void follow()
    {
        if (target == null) { return; }

        direction = (transform.position - target.transform.position).normalized;
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

        if (!shouldFollow)
        {
            Debug.Log("STOP FOLLOWING");
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Obstacles")
        {
            direction = -direction;
        }
    }
}