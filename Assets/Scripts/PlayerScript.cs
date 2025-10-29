using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using System.Threading; // Needed for List in handleAttacks

public class PlayerScript : MonoBehaviour
{
    // --- (Your existing variables) ---
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
    public float attackCooldownDuration = 0.5f;

    private CraftingManager craftingManager;
    
    [Header("Item Holding")]
    public SpriteRenderer heldItemSprite; // Drag your 'HeldItem' child object's SpriteRenderer here
    private float currentItemDamage = 1f; // Stores the damage of the currently held item. Defaults to 1 (fist).

    // --- ⭐ NEW VARIABLES ADDED HERE ---
    [Header("Component References")]
    [Tooltip("Drag your InventoryManager object here")]
    public InventoryManager inventoryManager;
    [Tooltip("Drag your HungerBar object here")]
    public HungerBar hungerBar;
    [Tooltip("Drag your HealthBar object here")]
    public HealthBar healthBar;
    public int currentHealth;
    public bool isTakingDamage = false;
    public float takeDamageTimer = 0;
    public float takeDamageDuration = 1f;   
    // ---------------------------------

    // --- Start() Method ---
    void Start()
    {
        GameObject craftingManagerObject = GameObject.FindGameObjectWithTag("Crafting");
        if (craftingManagerObject != null)
        {
            craftingManager = craftingManagerObject.GetComponent<CraftingManager>();
        }
        if (craftingManager == null)
        {
            Debug.LogError("Player could not find the CraftingManager!");
        }

        // --- ⭐ ADDED: Safety checks for new components ---
        if (inventoryManager == null)
        {
            // Try to find it if not assigned
            inventoryManager = FindFirstObjectByType<InventoryManager>();
            if (inventoryManager == null)
                Debug.LogError("InventoryManager is not assigned on Player and could not be found!");
        }
        if (hungerBar == null)
        {
            // Try to find it if not assigned
            hungerBar = FindFirstObjectByType<HungerBar>();
            if (hungerBar == null)
                Debug.LogError("HungerBar is not assigned on Player and could not be found!");
        }
        if (healthBar == null)
        {
            // Try to find it if not assigned
            healthBar = FindFirstObjectByType<HealthBar>();
            if (healthBar == null)
                Debug.LogError("HungerBar is not assigned on Player and could not be found!");
        }
        currentHealth = healthBar.currentHealth;
        // ------------------------------------------------
    }

    // --- Update() Method ---
    void Update()
    {
        prototypeMovement();        
        handleMovementAnimations(); 
        handleFightAnimations();
        handleAttacks();
        takeDamage();

        // --- ⭐ NEW "EAT" LOGIC ADDED HERE ---
        // Check for right mouse click (Mouse1) to eat
        if (Input.GetKeyDown(KeyCode.Mouse1)) 
        {
            HandleEating();
        }
        // ------------------------------------
    }

    // --- ⭐ NEW FUNCTION ADDED HERE ---
    private void HandleEating()
    {
        if (inventoryManager == null || hungerBar == null) return;

        // Get the currently selected item
        Item selectedItem = inventoryManager.GetSelectedItem(false); // false = don't use/consume item yet

        // Check if we actually got an item and if that item is food
        if (selectedItem != null && selectedItem.isFood)
        {
            // It's food! Now tell the inventory to consume one
            inventoryManager.GetSelectedItem(true); // true = use/consume one item
            
            // Call the HungerBar's EatFood function
            hungerBar.EatFood(selectedItem.hungerRestore, selectedItem.healthRestore);
        }
    }
    // ---------------------------------
    
    // --- prototypeMovement() Method ---
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
    
    // --- handleMovementAnimations() Method ---
    void handleMovementAnimations()
    {
        myAnimator.SetBool("isMoving", isMoving); 

        if (isMoving) 
        {
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
                mySpriteRenderer.flipX = false; 
            }
            else if (direction == "down")
            {
                myAnimator.SetBool("isLeftRight", false);
                myAnimator.SetBool("isUp", false);
                myAnimator.SetBool("isDown", true);
                mySpriteRenderer.flipX = false; 
            }
        }
    }
    
    // --- handleFightAnimations() Method ---
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
    
    // --- handleAttacks() Method ---
    void handleAttacks()
    {
        if (attackCooldownActive)
        {
             attackCooldown += Time.deltaTime;
             if(attackCooldown >= attackCooldownDuration)
             {
                 attackCooldown = 0;
                 attackCooldownActive = false;
             }
        }

        if (isAttacking && !attackCooldownActive)
        {
            CheckHitbox(leftHitBox, Vector3.left);
            CheckHitbox(rightHitBox, Vector3.right);
            CheckHitbox(upHitBox, Vector3.up);
            CheckHitbox(downHitBox, Vector3.down);
        }
    }

    // --- CheckHitbox() Method ---
    void CheckHitbox(GameObject hitbox, Vector2 knockbackDirection)
    {
        if (!hitbox.activeInHierarchy) return; // Don't check inactive hitboxes

        List<Collider2D> results = new List<Collider2D>();
        // Correct way to get overlaps for 2D Physics
        ContactFilter2D filter = ContactFilter2D.noFilter; // Or configure filter if needed
        int hitCount = hitbox.GetComponent<Collider2D>().Overlap(filter, results);

        if (hitCount > 0)
        {
            foreach (Collider2D c in results)
            {
                if (c.CompareTag("Enemy")) // Use CompareTag for efficiency
                {
                    FollowPlayerScript enemyScript = c.gameObject.GetComponent<FollowPlayerScript>();
                    MoveAndFollowPlayerScript enemyScript2 = c.gameObject.GetComponent<MoveAndFollowPlayerScript>();

                    if (enemyScript != null)
                    {
                        //Debug.Log("ENEMY HEALTH: " + enemyScript.health);
                        enemyScript.health -= 1; // Assuming damage is 1

                        // Apply knockback if enemy has Rigidbody2D
                        Rigidbody2D enemyRb = c.attachedRigidbody;

                        if (enemyRb != null)
                        {
                            enemyRb.position += knockbackDirection * knockBack;
                            //c.attachedRigidbody.linearVelocity = knockbackDirection * knockBack;
                            attackCooldownActive = true; // Start cooldown
                        }
                        //break; // Only hit one enemy per swing in this direction
                    }

                    // for moving and follow enemies
                    if (enemyScript2 != null)
                    {
                        //Debug.Log("ENEMY HEALTH: " + enemyScript2.health);
                        enemyScript2.health -= 1; // Assuming damage is 1

                        // Apply knockback if enemy has Rigidbody2D
                        Rigidbody2D enemyRb = c.attachedRigidbody;

                        if (enemyRb != null)
                        {
                            enemyRb.position += knockbackDirection * knockBack;
                            attackCooldownActive = true; // Start cooldown
                        }
                        break; // Only hit one enemy per swing in this direction
                    }
                }
                // Add checks for other tags here (e.g., "ResourceNode") if needed
            }
        }
    }

    void takeDamage()
    {
        if (isTakingDamage)
        {
            if (takeDamageTimer < takeDamageDuration)
            {
                takeDamageTimer += Time.deltaTime;
            }
            else
            {
                // timer is up, take more damage off the player
                currentHealth -= 5;
                takeDamageTimer = 0;
            }
        }
        healthBar.slider.value = currentHealth; // always update the current health
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            isTakingDamage = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            isTakingDamage = false;
            takeDamageTimer = 0;
        }
    }

    // --- OnTriggerEnter2D() Method ---
    private void OnTriggerEnter2D(Collider2D other)
    {
        CraftingStationIdentifier station = other.GetComponent<CraftingStationIdentifier>();
        if (station != null && craftingManager != null)
        {
            craftingManager.SetCurrentCraftingStation(station.stationType);
        }
    }
    
    // --- OnTriggerExit2D() Method ---
    private void OnTriggerExit2D(Collider2D other)
    {
        CraftingStationIdentifier station = other.GetComponent<CraftingStationIdentifier>();
        if (station != null && craftingManager != null)
        {
            craftingManager.SetCurrentCraftingStation(CraftingStation.None);
        }
    }
    
    // --- UpdateHeldItem() Method ---
    public void UpdateHeldItem(Sprite spriteToShow)
    {
        if (heldItemSprite == null)
        {
            Debug.LogError("HeldItemSprite is not assigned on the PlayerScript!");
            return;
        }

        if (spriteToShow == null)
        {
            heldItemSprite.sprite = null;
            heldItemSprite.enabled = false;
        }
        else
        {
            heldItemSprite.sprite = spriteToShow;
            heldItemSprite.enabled = true;
        }
    }
    
    // --- UpdateCurrentDamage() Method ---
    public void UpdateCurrentDamage(float newDamage)
    {
        // If the item has 0 or invalid damage, default to 1 (fist)
        if (newDamage <= 0)
        {
            currentItemDamage = 1f;
        }
        else
        {
            currentItemDamage = newDamage;
        }
        Debug.Log($"Player's damage updated to: {currentItemDamage}");
    }
}