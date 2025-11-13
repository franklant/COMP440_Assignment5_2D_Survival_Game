using UnityEngine;

[RequireComponent(typeof(Animator))] 
public class FriendlyNPC : MonoBehaviour
{
    // --- Movement Fields ---
    [Header("Patrol")]
    public Transform[] waypoints; // A list of points to patrol
    public float moveSpeed = 2f;
    public float waitTimeAtWaypoint = 3f; // How long to wait at each point

    // --- Private Variables ---
    private Animator animator;
    private Vector2 lastPosition;
    private Vector2 lastMoveDirection; 
    private int currentWaypointIndex = 0;
    private float waitTimer;
    private bool isWaiting;

    // How close we need to be to "reach" a waypoint
    private const float waypointReachedDistance = 0.1f;
    private const float moveThreshold = 0.01f; 

    void Start()
    {
        animator = GetComponent<Animator>();
        lastPosition = transform.position;
        lastMoveDirection = Vector2.down; // Default to facing down
        waitTimer = 0f;
        isWaiting = false;
    }

    void Update()
    {
        // --- PATROL LOGIC ---
        
        // If we don't have waypoints, just stay idle.
        if (waypoints.Length == 0)
        {
            isWaiting = true;
        }
        // Check if we are currently waiting at a waypoint
        else if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                // Done waiting, get the next waypoint
                isWaiting = false;
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length; // Loops back to 0
            }
        }
        // We are not waiting, so we should be moving.
        else 
        {
            Transform targetWaypoint = waypoints[currentWaypointIndex];
            float distanceToTarget = Vector2.Distance(transform.position, targetWaypoint.position);

            if (distanceToTarget > waypointReachedDistance)
            {
                // Move towards the waypoint
                transform.position = Vector2.MoveTowards(
                    transform.position, 
                    targetWaypoint.position, 
                    moveSpeed * Time.deltaTime
                );
            }
            else
            {
                // We've reached the waypoint, so start waiting
                isWaiting = true;
                waitTimer = waitTimeAtWaypoint;
            }
        }

        // --- ANIMATOR LOGIC ---
        // This part is the same as before and works perfectly.
        // It checks if our position changed (due to the patrol)
        // and sets the animator parameters automatically.

        Vector2 currentPosition = transform.position;
        Vector2 movement = currentPosition - lastPosition;
        
        if (movement.sqrMagnitude > moveThreshold * moveThreshold)
        {
            // --- WE ARE MOVING ---
            animator.SetBool("isMoving", true);
            lastMoveDirection = movement.normalized;
        }
        else
        {
            // --- WE ARE IDLE ---
            animator.SetBool("isMoving", false);
        }

        // Set direction floats (uses lastMoveDirection if idle)
        animator.SetFloat("moveX", lastMoveDirection.x);
        animator.SetFloat("moveY", lastMoveDirection.y);

        lastPosition = currentPosition;
    }
}