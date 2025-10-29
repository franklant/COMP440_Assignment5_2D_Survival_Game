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

    // --- CraftingManager Reference (Present in Both) ---
    private CraftingManager craftingManager;

    [Header("Item Holding")] 
    public SpriteRenderer heldItemSprite; 
    private float currentItemDamage = 1f; 

    // --- Component References ---
    [Header("Component References")]
    [Tooltip("Drag your InventoryManager object here")]
    public InventoryManager inventoryManager; 
    [Tooltip("Drag your HungerBar object here")]
    public HungerBar hungerBar;               
    
    void Start()
    {
        craftingManager = FindFirstObjectByType<CraftingManager>();
        
        if (craftingManager == null)
        {
            Debug.LogError("Player could not find the CraftingManager in the scene!");
        }
        
        if (inventoryManager == null)
        {
            inventoryManager = FindFirstObjectByType<InventoryManager>();
            if (inventoryManager == null)
                Debug.LogError("InventoryManager is not assigned on Player and could not be found!");
        }
        if (hungerBar == null)
        {
            hungerBar = FindFirstObjectByType<HungerBar>();
            if (hungerBar == null)
                Debug.LogError("HungerBar is not assigned on Player and could not be found!");
        }
    }
    
    void Update()
    {
        prototypeMovement();        
        handleMovementAnimations(); 
        handleFightAnimations();    
        handleAttacks();            

        if (Input.GetKeyDown(KeyCode.Mouse1)) 
        {
            HandleEating();
        }
    }
    
    private void HandleEating()
    {
        if (inventoryManager == null || hungerBar == null) return;
        
        Item selectedItem = inventoryManager.GetSelectedItem(false); 
        
        if (selectedItem != null && selectedItem.isFood)
        {
            inventoryManager.GetSelectedItem(true); 
            hungerBar.EatFood(selectedItem.hungerRestore, selectedItem.healthRestore);
        }
    }
    
    void prototypeMovement()
    {
        if (Input.GetKey(KeyCode.Space)) 
        {
            isMoving = false;
            isAttacking = true; 
        }
        else
        {
            isAttacking = false; 
        }
        
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
            
            if (moveDirection != Vector3.zero)
            {
                isMoving = true;
                transform.position += (moveDirection * movementSpeed) * Time.deltaTime;
            }
            else
            {
                isMoving = false; 
            }
        } else {
             isMoving = false; 
        }
    }
    
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
    
    void handleFightAnimations()
    {
         myAnimator.SetBool("isAttacking", isAttacking); 

        if (isAttacking)
        {
            handleMovementAnimations(); 

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
            CheckHitbox(leftHitBox, Vector2.left);
            CheckHitbox(rightHitBox, Vector2.right);
            CheckHitbox(upHitBox, Vector2.up);
            CheckHitbox(downHitBox, Vector2.down);
        }
    }

    // --- ⭐ MODIFIED THIS FUNCTION ---
    void CheckHitbox(GameObject hitbox, Vector2 knockbackDirection)
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
                    MoveAndFollowPlayerScript enemyScript2 = c.gameObject.GetComponent<MoveAndFollowPlayerScript>();

                    if (enemyScript != null)
                    {
                        enemyScript.health -= currentItemDamage; 
                        Rigidbody2D enemyRb = c.attachedRigidbody;

                        if (enemyRb != null)
                        {
                            enemyRb.position += knockbackDirection * knockBack;
                            attackCooldownActive = true; 
                        }
                    }
                    
                    if (enemyScript2 != null)
                    {
                        enemyScript2.health -= currentItemDamage; 
                        Rigidbody2D enemyRb = c.attachedRigidbody;

                        if (enemyRb != null)
                        {
                            enemyRb.position += knockbackDirection * knockBack;
                            attackCooldownActive = true;
                        }
                        break; 
                    }
                 }
                 // Check for Resource
                 else if (c.CompareTag("Resource"))
                 {
                     // Check for Trees
                     TreeResource treeScript = c.gameObject.GetComponent<TreeResource>();
                     if (treeScript != null)
                     {
                         treeScript.TakeDamage(currentItemDamage);
                         attackCooldownActive = true; 
                         return; // Only hit one thing
                     }

                     // ⭐ NEW: Check for Stone
                     StoneResource stoneScript = c.gameObject.GetComponent<StoneResource>();
                     if (stoneScript != null)
                     {
                         stoneScript.TakeDamage(currentItemDamage);
                         attackCooldownActive = true;
                         return; // Only hit one thing
                     }
                 }
             }
         }
    }


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
            craftingManager.SetCurrentCraftingStation(CraftingStation.None);
        }
    }
    
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
    
    public void UpdateCurrentDamage(float newDamage)
    {
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