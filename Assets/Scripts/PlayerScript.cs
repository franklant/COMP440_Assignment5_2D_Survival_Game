using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public Rigidbody2D myRigidBody;
    public Animator myAnimator;
    public SpriteRenderer mySpriteRenderer;     // control the way the sprite faces the correct direction
    public float movementSpeed;
    private string dirHorizontal;               // keeps track of the players current horizontal direction
    private string dirVertical;                 // keeps track of the players current vertical direction

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        prototypeMovement();    // subject to change
        handleAnimations();
    }

    void prototypeMovement()
    {
        // Horizontal Movement
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            dirHorizontal = "left";
            transform.position += (Vector3.left * movementSpeed) * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            dirHorizontal = "right";
            transform.position += (Vector3.right * movementSpeed) * Time.deltaTime;
        }
        else
        {
            dirHorizontal = "";
        }

        // Vertical Movement
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            dirVertical = "up";
            transform.position += (Vector3.up * movementSpeed) * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            dirVertical = "down";
            transform.position += (Vector3.down * movementSpeed) * Time.deltaTime;
        }
        else
        {
            dirVertical = "";
        }
    }

    void handleAnimations()
    {
        if (dirHorizontal != "")
        {
            myAnimator.SetBool("isMovingHorizontal", true);
            //myAnimator.SetBool("isMovingVertical", false);

            if (dirHorizontal == "left")
            {
                mySpriteRenderer.flipX = true;
            }
            else
            {
                mySpriteRenderer.flipX = false;
            }
        }
        else
        {
            myAnimator.SetBool("isMovingHorizontal", false);
        }

        if (dirVertical != "")
        {
            myAnimator.SetBool("isMovingVertical", true);

            // check if going backward
            if (dirVertical == "up")
            {
                myAnimator.SetBool("flipY", true);
            }
            else
            {
                myAnimator.SetBool("flipY", false);
            }
        }
        else
        {
            myAnimator.SetBool("isMovingVertical", false);
        }
    }
}
