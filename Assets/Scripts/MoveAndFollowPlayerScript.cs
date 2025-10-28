using System;
using UnityEngine;

public class MoveAndFollowPlayerScript : MonoBehaviour
{
    private Transform target;
    public Rigidbody2D myRigidBody;
    public BoxCollider2D myBoxCollider;
    public SpriteRenderer mySpriteRenderer;
    public float health = 5;
    public float movementSpeed;
    public float detectionRadius = 1f;
    public float stoppingDistance = 0.2f;
    public bool isAttacked = false;
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


    Vector3 endPoint;
    Vector3 direction;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentState = scatterState;

        SpriteRenderer backgroundPlaneRenderer = GameObject.FindGameObjectWithTag("Background").GetComponent<SpriteRenderer>();

        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        if (backgroundPlaneRenderer == null)
        {
            Debug.LogError("Cannot Access Backgrounnd Plane Object/Sprite Renderer!");
        } else
        {
            yBounds = backgroundPlaneRenderer.size.y;
            xBounds = backgroundPlaneRenderer.size.x;
        }
    }

    // Update is called once per frame
    void Update()
    {
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
        // Debug.Log("Distance: " + distanceToTargetRaw);

        if (distanceToTarget <= detectionRadius && distanceToTarget > stoppingDistance && !isAttacked)
        {
            shouldFollow = true;
            direction = Vector3.zero;
        }
        else
        {
            //myRigidBody.linearVelocity = Vector3.zero;
            shouldFollow = false;
        }

        if (health <= 0)
        {
            Destroy(gameObject);
        }

        // flip sprite properly
        // if (direction.x > 0 && (currentState == scatterState || currentState == moveState))
        // {
        //     mySpriteRenderer.flipX = true;
        // } else if (direction.x < 0 && (currentState == scatterState || currentState == moveState))
        // {
        //     mySpriteRenderer.flipX = true;
        // } else
        // {
        //     mySpriteRenderer.flipX = false;
        // }
    }

    public void scatter()
    {
        endPoint = new Vector3(UnityEngine.Random.Range(-xBounds, xBounds), UnityEngine.Random.Range(-yBounds, yBounds), 0);
        direction = (endPoint - transform.position).normalized;
        float distance = Vector3.Distance(endPoint, transform.position);

        Debug.Log(direction);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction);

        if (hit.collider != null) // raycast has hit an object
        {
            // abort and calculate new path
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

            float distanceToPoint = Vector3.Distance(endPoint, transform.position);
            //Debug.Log("Distance to Point: " + distanceToPoint);

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
        direction = (transform.position - target.transform.position).normalized;

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

        if (!shouldFollow)
        {
            Debug.Log("STOP FOLLOWING");
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.tag == "Obstacles")
        {
            currentState = scatterState;
            scatter();
        }
    }
}
