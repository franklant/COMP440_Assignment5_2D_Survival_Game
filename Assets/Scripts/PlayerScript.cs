using UnityEngine;
using System.Collections.Generic; // Needed for List in handleAttacks

public class PlayerScript : MonoBehaviour
{
    // --- Variables from "His" Script (Includes Rigidbody & Speed) ---
    public Rigidbody2D myRigidBody;
    public Animator myAnimator;
    public SpriteRenderer mySpriteRenderer;
    public GameObject leftHitBox;
    public GameObject rightHitBox;
    public GameObject upHitBox;
    public GameObject downHitBox;
    public float movementSpeed;
    public float knockBack = 7;
    private string direction = "down";
    private bool isAttacking = false;
    private bool isMoving = false;
    private float attackCooldown = 0;
    private bool attackCooldownActive = false;
    public float attackCooldownDuration = 0.5f; // Defaulted from "His" script, adjust if needed

    // --- CraftingManager Reference (Present in Both) ---
    private CraftingManager craftingManager;

    // --- Start() Method (Using Tag from "His" Script) ---
    void Start()
    {
        // Find the CraftingManager using the tag "Crafting".
        // Ensure the GameObject with CraftingManager has this tag assigned in the Inspector.
        GameObject craftingManagerObject = GameObject.FindGameObjectWithTag("Crafting");
        if (craftingManagerObject != null)
        {
            craftingManager = craftingManagerObject.GetComponent<CraftingManager>();
        }
        
        // Error handling if not found
        if (craftingManager == null)
        {
            Debug.LogError("Player could not find the CraftingManager in the scene! Ensure an object has the 'Crafting' tag and the CraftingManager script.");
        }
    }

    // --- Update() Method (From "His" Script) ---
    void Update()
    {
        prototypeMovement();        // Handles movement input and state
        handleMovementAnimations(); // Handles animation based on movement state
        handleFightAnimations();    // Handles animation based on attack state
        handleAttacks();            // Handles hitbox activation and collision checks
    }

    // --- prototypeMovement() Method (From "His" Script) ---
    void prototypeMovement()
    {
        // Check for attack input first
        if (Input.GetKey(KeyCode.Space)) // Assuming Space is attack
        {
            isMoving = false;
            isAttacking = true; // Set attack state
        }
        else
        {
            isAttacking = false; // Clear attack state if Space is not held
        }

        // Only allow movement if not attacking
        if (!isAttacking)
        {
            Vector3 moveDirection = Vector3.zero;

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                moveDirection = Vector3.left;
                direction = "left";
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                moveDirection = Vector3.right;
                direction = "right";
            }
            else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                moveDirection = Vector3.up;
                direction = "up";
            }
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                moveDirection = Vector3.down;
                direction = "down";
            }

            // Apply movement and update state
            if (moveDirection != Vector3.zero)
            {
                isMoving = true;
                transform.position += (moveDirection * movementSpeed) * Time.deltaTime;
            }
            else
            {
                isMoving = false; // No movement keys pressed
            }
        } else {
             isMoving = false; // Ensure not moving if attacking
        }
    }

    // --- handleMovementAnimations() Method (From "His" Script) ---
    void handleMovementAnimations()
    {
        myAnimator.SetBool("isMoving", isMoving); // Directly use the state variable

        if (isMoving) // Only update direction/flip if actually moving
        {
            // Simplified logic using the 'direction' variable
            if (direction == "left")
            {
                myAnimator.SetBool("isLeftRight", true);
                myAnimator.SetBool("isUp", false);
                myAnimator.SetBool("isDown", false);
                mySpriteRenderer.flipX = true;
            }
            else if (direction == "right")
            {
                myAnimator.SetBool("isLeftRight", true);
                myAnimator.SetBool("isUp", false);
                myAnimator.SetBool("isDown", false);
                mySpriteRenderer.flipX = false;
            }
            else if (direction == "up")
            {
                myAnimator.SetBool("isLeftRight", false);
                myAnimator.SetBool("isUp", true);
                myAnimator.SetBool("isDown", false);
                mySpriteRenderer.flipX = false; // Assuming default facing right/down
            }
            else if (direction == "down")
            {
                myAnimator.SetBool("isLeftRight", false);
                myAnimator.SetBool("isUp", false);
                myAnimator.SetBool("isDown", true);
                mySpriteRenderer.flipX = false; // Assuming default facing right/down
            }
        }
        else
        {
             // Ensure directional states are false if not moving
             myAnimator.SetBool("isLeftRight", false);
             myAnimator.SetBool("isUp", false);
             myAnimator.SetBool("isDown", false);
        }
    }

    // --- handleFightAnimations() Method (From "His" Script) ---
    void handleFightAnimations()
    {
         myAnimator.SetBool("isAttacking", isAttacking); // Use the state variable

        // Only activate hitboxes and set specific attack anims if attacking
        if (isAttacking)
        {
            // Update directional animations based on 'direction'
            handleMovementAnimations(); // Reuse movement anim logic for direction

            // Activate correct hitbox based on 'direction'
            leftHitBox.SetActive(direction == "left");
            rightHitBox.SetActive(direction == "right");
            upHitBox.SetActive(direction == "up");
            downHitBox.SetActive(direction == "down");
        }
        else // Ensure all hitboxes are off if not attacking
        {
            leftHitBox.SetActive(false);
            rightHitBox.SetActive(false);
            upHitBox.SetActive(false);
            downHitBox.SetActive(false);
        }
    }

    // --- handleAttacks() Method (From "His" Script) ---
    void handleAttacks()
    {
        // Cooldown Logic
        if (attackCooldownActive)
        {
             attackCooldown += Time.deltaTime;
             if(attackCooldown >= attackCooldownDuration)
             {
                 attackCooldown = 0;
                 attackCooldownActive = false;
             }
        }

        // Only check for hits if attacking and cooldown is not active
        if (isAttacking && !attackCooldownActive)
        {
            CheckHitbox(leftHitBox, Vector3.left);
            CheckHitbox(rightHitBox, Vector3.right);
            CheckHitbox(upHitBox, Vector3.up);
            CheckHitbox(downHitBox, Vector3.down);
        }
    }

    // --- Helper Method for handleAttacks() (Extracted from "His" Script) ---
    void CheckHitbox(GameObject hitbox, Vector3 knockbackDirection)
    {
         if (!hitbox.activeInHierarchy) return; // Don't check inactive hitboxes

         List<Collider2D> results = new List<Collider2D>();
         // Correct way to get overlaps for 2D Physics
         ContactFilter2D filter = new ContactFilter2D().NoFilter(); // Or configure filter if needed
         int hitCount = hitbox.GetComponent<Collider2D>().Overlap(filter, results);

         if (hitCount > 0)
         {
             foreach (Collider2D c in results)
             {
                 if (c.CompareTag("Enemy")) // Use CompareTag for efficiency
                 {
                     FollowPlayerScript enemyScript = c.gameObject.GetComponent<FollowPlayerScript>();
                     if (enemyScript != null)
                     {
                         Debug.Log("ENEMY HEALTH: " + enemyScript.health);
                         enemyScript.health -= 1; // Assuming damage is 1
                         attackCooldownActive = true; // Start cooldown
                         
                         // Apply knockback if enemy has Rigidbody2D
                          Rigidbody2D enemyRb = c.attachedRigidbody;
                          if (enemyRb != null)
                          {
                               enemyRb.linearVelocity = knockbackDirection * knockBack;
                          }
                         return; // Only hit one enemy per swing in this direction
                     }
                 }
                 // Add checks for other tags here (e.g., "ResourceNode") if needed
             }
         }
    }


    // --- Crafting Station Detection (Present in Both) ---
    private void OnTriggerEnter2D(Collider2D other)
    {
        CraftingStationIdentifier station = other.GetComponent<CraftingStationIdentifier>();
        if (station != null && craftingManager != null)
        {
            Debug.Log("Entered crafting station area: " + station.stationType);
            craftingManager.SetCurrentCraftingStation(station.stationType);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        CraftingStationIdentifier station = other.GetComponent<CraftingStationIdentifier>();
        if (station != null && craftingManager != null)
        {
            Debug.Log("Left crafting station area: " + station.stationType);
            // Only reset if leaving the *current* station (prevents issues if overlapping triggers)
            // if (craftingManager.GetCurrentStation() == station.stationType) // Assumes GetCurrentStation() exists
            craftingManager.SetCurrentCraftingStation(CraftingStation.None);
        }
    }
}
