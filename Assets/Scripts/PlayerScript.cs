using UnityEngine;
using System.Collections.Generic; // Needed for List in handleAttacks
using Random = UnityEngine.Random;
using JetBrains.Annotations;
using UnityEngine.SocialPlatforms; // Be specific

public class PlayerScript : MonoBehaviour
{
    // --- Components & Setup ---
    public Rigidbody2D myRigidBody;
    public Animator myAnimator;
    public SpriteRenderer mySpriteRenderer;

    [Header("Combat")]
    public GameObject leftHitBox;
    public GameObject rightHitBox;
    public GameObject upHitBox;
    public GameObject downHitBox;
    public float knockBack = 7;
    private bool isAttacking = false;
    private float attackCooldown = 0;
    private bool attackCooldownActive = false;
    public float attackCooldownDuration = 0.5f;
    private bool isAttacked = false;
    private float time = 0;

    [Header("Access Crafting Table")]
    public GameObject craftingTable;

    [Header("Movement")]
    public float movementSpeed;
    private string direction = "down";
    private bool isMoving = false;

    [Header("Item Holding")]
    public SpriteRenderer heldItemSprite;
    private float currentItemDamage = 1f; // Default damage (fist)

    [Header("System References")]
    [Tooltip("Drag your InventoryManager object here")]
    public InventoryManager inventoryManager;
    [Tooltip("Drag your HungerBar object here")]
    public HungerBar hungerBar;
    [Tooltip("Drag your LogicManager (with the CraftingManager) here")]
    public CraftingManager craftingManager;

    [Header("Audio")]
    public AudioSource footstepAudioSource; // Assign in Inspector
    public List<AudioClip> snowStepSounds;  // Assign sounds in Inspector
    public float timeBetweenSteps = 0.4f;
    private float footstepTimer;

    [Tooltip("Sound to play when the player is attacked")]
    public AudioClip hitSound;

    [Tooltip("Drag your HealthBar here")]
    public HealthBar healthBar;
    public float currentHealth;

    // --- Unity Methods ---

    void Start()
    {
        // --- Use Inspector references ---
        if (craftingManager == null)
            Debug.LogError("CraftingManager is not assigned on Player!", this);

        if (inventoryManager == null)
            Debug.LogError("InventoryManager is not assigned on Player!", this);
        // --- End Inspector references ---

        // Attempt to find others if not assigned
        if (hungerBar == null)
        {
            hungerBar = FindFirstObjectByType<HungerBar>();
            if (hungerBar == null)
                Debug.LogError("HungerBar is not assigned on Player and could not be found!");
        }
        if (footstepAudioSource == null)
        {
            footstepAudioSource = GetComponent<AudioSource>();
            if (footstepAudioSource == null)
                Debug.LogWarning("Footstep Audio Source is not assigned and could not be found on Player!", this);
        }
        if (healthBar == null)
        {
            healthBar = FindFirstObjectByType<HealthBar>();
            if (healthBar == null)
                Debug.LogError("HealthBar is not assigned on Player and could not be found!");
        }
        currentHealth = healthBar.slider.value;
    }

    void Update()
    {
        prototypeMovement();
        handleMovementAnimations();
        handleFightAnimations();
        handleAttacks();
        HandleFootsteps();

        // Eating input
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            HandleEating();
        }

        if (!isAttacked)
        {
            mySpriteRenderer.color = Color.white;
            time = 0;
        }
        else
        {
            if (time < 1f)
            {
                time += Time.deltaTime;
                mySpriteRenderer.color = Color.darkRed;
            } else
            {
                isAttacked = false;
                time = 0;
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
            craftingTable.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        CraftingStationIdentifier station = other.GetComponent<CraftingStationIdentifier>();
        if (station != null && craftingManager != null)
        {
            Debug.Log("Left crafting station area: " + station.stationType);
            craftingManager.SetCurrentCraftingStation(CraftingStation.None);
            craftingTable.SetActive(false);
        }
    }

    // --- Input & Movement ---

    void prototypeMovement()
    {
        // Attack overrides movement
        if (Input.GetKey(KeyCode.Space))
        {
            isMoving = false;
            isAttacking = true;
        }
        else
        {
            isAttacking = false;
        }

        // Only move if not attacking
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
                // Use Rigidbody for physics-based movement
                myRigidBody.linearVelocity = moveDirection * movementSpeed;
                // transform.position += (moveDirection * movementSpeed) * Time.deltaTime; // Less ideal for physics
            }
            else
            {
                isMoving = false;
                myRigidBody.linearVelocity = Vector2.zero; // Stop movement
            }
        } else {
             isMoving = false; // Ensure not moving if attacking
             myRigidBody.linearVelocity = Vector2.zero; // Stop movement
        }
    }

    // --- Animation Handling ---

    void handleMovementAnimations()
    {
        myAnimator.SetBool("isMoving", isMoving);

        // Update direction/flip only if moving
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
            handleMovementAnimations(); // Reuse movement anim logic for direction

            // Activate correct hitbox
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

    // --- Combat & Interaction ---

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

        // Check for hits if attacking and cooldown is not active
        if (isAttacking && !attackCooldownActive)
        {
            CheckHitbox(leftHitBox, Vector2.left);
            CheckHitbox(rightHitBox, Vector2.right);
            CheckHitbox(upHitBox, Vector2.up);
            CheckHitbox(downHitBox, Vector2.down);
        }
    }

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
                 // Check for Enemy
                 if (c.CompareTag("Enemy"))
                 {
                    FollowPlayerScript enemyScript = c.gameObject.GetComponent<FollowPlayerScript>();
                    MoveAndFollowPlayerScript enemyScript2 = c.gameObject.GetComponent<MoveAndFollowPlayerScript>();

                    Rigidbody2D enemyRb = c.attachedRigidbody; // Get Rigidbody once

                    if (enemyScript != null)
                    {
                        enemyScript.TakeDamage(currentItemDamage); // Use TakeDamage function
                        if (enemyRb != null) {
                            // Use AddForce for better knockback feel
                            enemyRb.AddForce(knockbackDirection * knockBack, ForceMode2D.Impulse); // knockback not working
                            //enemyRb.position = knockbackDirection * knockBack;
                        }
                        attackCooldownActive = true;
                        return; // Hit only one thing per swing
                    }
                    if (enemyScript2 != null)
                    {
                        enemyScript2.health -= currentItemDamage; // Assuming this script doesn't have TakeDamage yet
                        if (enemyRb != null) {
                            enemyRb.AddForce(knockbackDirection * knockBack, ForceMode2D.Impulse); // knockback not working
                            //enemyRb.position = knockbackDirection * knockBack;
                        }
                        attackCooldownActive = true;
                        return; // Hit only one thing per swing
                    }
                 }
                 // Check for Resource
                 else if (c.CompareTag("Resource"))
                 {
                     TreeResource treeScript = c.gameObject.GetComponent<TreeResource>();
                     if (treeScript != null) {
                         treeScript.TakeDamage(currentItemDamage);
                         attackCooldownActive = true;
                         return; // Hit only one thing
                     }
                     StoneResource stoneScript = c.gameObject.GetComponent<StoneResource>();
                     if (stoneScript != null) {
                         stoneScript.TakeDamage(currentItemDamage);
                         attackCooldownActive = true;
                         return; // Hit only one thing
                     }
                 }
             }
         }
    }

    /// <summary>
    /// Called by enemies or hazards to damage the player.
    /// </summary>
    public void TakeDamage(float damageAmount)
    {
        if (hungerBar != null)
        {
            Debug.Log($"Player took {damageAmount} damage.");
            hungerBar.ModifyHealth(-damageAmount); // Decrease health via HungerBar script
            // Optional: Add hurt sound, screen effect, etc.
        }
        else
        {
            Debug.LogError("Player cannot take damage - HungerBar reference is missing!", this);
        }

        isAttacked = true;

        if (hitSound != null)
            AudioSource.PlayClipAtPoint(hitSound, transform.position, 0.7f); // 0.7f is volume
    }

    // --- Item & Inventory ---

    private void HandleEating()
    {
        if (inventoryManager == null || hungerBar == null) return;

        Item selectedItem = inventoryManager.GetSelectedItem(false); // Check item without consuming

        if (selectedItem != null && selectedItem.isFood)
        {
            inventoryManager.GetSelectedItem(true); // Consume the item
            hungerBar.EatFood(selectedItem.hungerRestore, selectedItem.healthRestore);
        }
    }

    public void UpdateHeldItem(Sprite spriteToShow)
    {
        if (heldItemSprite == null) {
            Debug.LogError("HeldItemSprite is not assigned on the PlayerScript!");
            return;
        }
        if (spriteToShow == null) {
            heldItemSprite.sprite = null;
            heldItemSprite.enabled = false;
        }
        else {
            heldItemSprite.sprite = spriteToShow;
            heldItemSprite.enabled = true;
        }
    }

    public void UpdateCurrentDamage(float newDamage)
    {
        // Use default fist damage if item damage is invalid
        currentItemDamage = (newDamage <= 0) ? 1f : newDamage;
        Debug.Log($"Player's damage updated to: {currentItemDamage}");
    }

    // --- Audio ---

    private void HandleFootsteps()
    {
        // Don't play steps if source/sounds missing or if attacking
        if (footstepAudioSource == null || snowStepSounds.Count == 0 || isAttacking)
        {
            footstepTimer = 0; // Reset timer if not moving/can't play
            return;
        }

        if (isMoving)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0)
            {
                PlayFootstepSound();
                footstepTimer = timeBetweenSteps; // Reset timer
            }
        }
        else
        {
            footstepTimer = 0; // Reset timer if stopped
        }
    }

    private void PlayFootstepSound()
    {
        int index = Random.Range(0, snowStepSounds.Count);
        AudioClip clipToPlay = snowStepSounds[index];

        if (clipToPlay != null) // Safety check for sound clip
        {
            footstepAudioSource.pitch = Random.Range(0.9f, 1.1f);
            footstepAudioSource.PlayOneShot(clipToPlay);
        }
    }
}