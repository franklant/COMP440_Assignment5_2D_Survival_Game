using UnityEngine;
using System.Collections.Generic; // Needed for List in handleAttacks

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
        // ------------------------------------------------
    }

    // --- Update() Method ---
    void Update()
    {
        prototypeMovement();        
        handleMovementAnimations(); 
        handleFightAnimations();    
        handleAttacks();            

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
        else
        {
             myAnimator.SetBool("isLeftRight", false);
             myAnimator.SetBool("isUp", false);
             myAnimator.SetBool("isDown", false);
        }
    }
    
    // --- handleFightAnimations() Method ---
    void handleFightAnimations()
    {
         myAnimator.SetBool("isAttacking", isAttacking); 

        if (isAttacking)
        {
            handleMovementAnimations(); // Reuse movement anim logic for direction

            leftHitBox.SetActive(direction == "left");
            rightHitBox.SetActive(direction == "right");
            upHitBox.SetActive(direction == "up");
            downHitBox.SetActive(direction == "down");
        }
        else 
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
    void CheckHitbox(GameObject hitbox, Vector3 knockbackDirection)
    {
         if (!hitbox.activeInHierarchy) return; 

         List<Collider2D> results = new List<Collider2D>();
         ContactFilter2D filter = ContactFilter2D.noFilter; 
         int hitCount = hitbox.GetComponent<Collider2D>().Overlap(filter, results);

         if (hitCount > 0)
         {
             foreach (Collider2D c in results)
             {
                 if (c.CompareTag("Enemy"))
                 {
                     FollowPlayerScript enemyScript = c.gameObject.GetComponent<FollowPlayerScript>();
                     if (enemyScript != null)
                     {
                         enemyScript.TakeDamage(currentItemDamage);
                         attackCooldownActive = true; 
                         
                          Rigidbody2D enemyRb = c.attachedRigidbody;
                          if (enemyRb != null)
                          {
                               enemyRb.linearVelocity = knockbackDirection * knockBack;
                          }
                         return; 
                     }
                 }
             }
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