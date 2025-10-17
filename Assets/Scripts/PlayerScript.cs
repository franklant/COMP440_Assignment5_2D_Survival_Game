using UnityEngine;
using System.Collections.Generic;

public class PlayerScript : MonoBehaviour
{
    public Rigidbody2D myRigidBody;
    public Animator myAnimator;
    public SpriteRenderer mySpriteRenderer;     // control the way the sprite faces the correct direction
    public GameObject leftHitBox;
    public GameObject rightHitBox;
    public GameObject upHitBox;
    public GameObject downHitBox;
    public float movementSpeed;
    private string direction = "down";          // keeps track of the players current direction
    private bool isAttacking = false;
    private bool isMoving = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        prototypeMovement();            // subject to change
        handleMovementAnimations();     // move animation
        handleFightAnimations();        // fight animation
        handleAttacks();                // attack collision
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

        // Horizontal Movement
        if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && !isAttacking)
        {
            isMoving = true;
            direction = "left";
            transform.position += (Vector3.left * movementSpeed) * Time.deltaTime;
        }
        else if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && !isAttacking)
        {
            isMoving = true;
            direction = "right";
            transform.position += (Vector3.right * movementSpeed) * Time.deltaTime;
        }
        else if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) && !isAttacking)
        {
            isMoving = true;
            direction = "up";
            transform.position += (Vector3.up * movementSpeed) * Time.deltaTime;
        }
        else if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && !isAttacking)
        {
            isMoving = true;
            direction = "down";
            transform.position += (Vector3.down * movementSpeed) * Time.deltaTime;
        }
        else
        {
            isMoving = false;
        }
    }

    // if the player is in it's moving state, the proper animations will play depending on the current move direction of the player
    void handleMovementAnimations()
    {
        if (isMoving)
        {
            isAttacking = false;
            myAnimator.SetBool("isMoving", true);
            myAnimator.SetBool("isAttacking", false);

            if (direction == "left" && isMoving)
            {
                myAnimator.SetBool("isLeftRight", true);
                mySpriteRenderer.flipX = true;
            }
            else if (direction == "right" && isMoving)
            {
                myAnimator.SetBool("isLeftRight", true);
                mySpriteRenderer.flipX = false;
            }
            else
            {
                myAnimator.SetBool("isLeftRight", false);
            }

            if (direction == "up" && isMoving)
            {
                myAnimator.SetBool("isUp", true);
            }
            else
            {
                myAnimator.SetBool("isUp", false);
            }

            if (direction == "down" && isMoving)
            {
                myAnimator.SetBool("isDown", true);
            }
            else
            {
                myAnimator.SetBool("isDown", false);
            }
        }
        else
        {
            myAnimator.SetBool("isMoving", false);
        }
    }

    // if the player is in the attacking state, the proper attack animation will play based on the current direction of the player
    void handleFightAnimations()
    {
        if (isAttacking)
        {
            isMoving = false;
            myAnimator.SetBool("isMoving", false);
            myAnimator.SetBool("isAttacking", true);

            if (direction == "left")
            {
                myAnimator.SetBool("isLeftRight", true);
                mySpriteRenderer.flipX = true;  // flip the sprite of the play

                leftHitBox.SetActive(true);
                rightHitBox.SetActive(false);
            }
            else if (direction == "right")
            {
                myAnimator.SetBool("isLeftRight", true);
                mySpriteRenderer.flipX = false;

                rightHitBox.SetActive(true);
                leftHitBox.SetActive(false);
            }
            else
            {
                myAnimator.SetBool("isLeftRight", false);

                rightHitBox.SetActive(false);
                leftHitBox.SetActive(false);
            }

            if (direction == "up")
            {
                myAnimator.SetBool("isUp", true);
                upHitBox.SetActive(true);
            }
            else
            {
                upHitBox.SetActive(false);
            }

            if (direction == "down")
            {
                myAnimator.SetBool("isDown", true);
                downHitBox.SetActive(true);
            }
            else
            {
                downHitBox.SetActive(false);
            }
        }
        else
        {
            myAnimator.SetBool("isAttacking", false);
            upHitBox.SetActive(false);
            downHitBox.SetActive(false);
            leftHitBox.SetActive(false);
            rightHitBox.SetActive(false);
        }
    }
    
    // when a directional hit box is active, the program will check over every object overlapping with the hit box
    // from there we can check what object the player has hit and perform the proper operation
    void handleAttacks()
    {
        // check collision from the left hit box
        if (leftHitBox.activeInHierarchy)
        {
            List<Collider2D> results = new List<Collider2D>();
            leftHitBox.GetComponent<Collider2D>().Overlap(results);

            foreach (Collider2D c in results)
            {
                // find a specifc collision object by name
                Debug.Log(c.name);
            }
        }

        // check collision from the right hit box
        if (rightHitBox.activeInHierarchy)
        {
            List<Collider2D> results = new List<Collider2D>();
            rightHitBox.GetComponent<Collider2D>().Overlap(results);

            foreach (Collider2D c in results)
            {
                // find a specifc collision object by name
                Debug.Log(c.name);
            }
        }

        // check collision from the top hit box
        if (upHitBox.activeInHierarchy)
        {
            List<Collider2D> results = new List<Collider2D>();
            upHitBox.GetComponent<Collider2D>().Overlap(results);

            foreach (Collider2D c in results)
            {
                // find a specifc collision object by name
                Debug.Log(c.name);
            }
        }
        
        // check collision from the bottom hit box
        if (downHitBox.activeInHierarchy)
        {
            List<Collider2D> results = new List<Collider2D>();
            downHitBox.GetComponent<Collider2D>().Overlap(results);

            foreach (Collider2D c in results)
            {
                // find a specifc collision object by name
                Debug.Log(c.name);
            }
        }
    }
}
