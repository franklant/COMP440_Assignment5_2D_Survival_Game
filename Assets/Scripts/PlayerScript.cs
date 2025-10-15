using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public Rigidbody2D myRigidBody;
    public float movementSpeed;
    
    // --- REMOVED UNUSED VARIABLES ---
    // private string dirHorizontal; 
    // private string dirVertical;   

    // --- NEW VARIABLE ---
    // A reference to our CraftingManager so we can talk to it.
    private CraftingManager craftingManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // --- NEW CODE ---
        // Find the CraftingManager in the scene so we can use it later.
        // This requires your GameManager object to be active.
        craftingManager = FindObjectOfType<CraftingManager>();
        if (craftingManager == null)
        {
            Debug.LogError("Player could not find the CraftingManager in the scene!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        prototypeMovement();    // subject to change
    }

    void prototypeMovement()
    {
        // Horizontal Movement
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            // dirHorizontal = "left"; // -- REMOVED
            transform.position += (Vector3.left * movementSpeed) * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            // dirHorizontal = "right"; // -- REMOVED
            transform.position += (Vector3.right * movementSpeed) * Time.deltaTime;
        }
        // else // -- REMOVED
        // { // -- REMOVED
        //     dirHorizontal = ""; // -- REMOVED
        // } // -- REMOVED

        // Vertical Movement
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            // dirVertical = "up"; // -- REMOVED
            transform.position += (Vector3.up * movementSpeed) * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            // dirVertical = "down"; // -- REMOVED
            transform.position += (Vector3.down * movementSpeed) * Time.deltaTime;
        }
        // else // -- REMOVED
        // { // -- REMOVED
        //     dirVertical = ""; // -- REMOVED
        // } // -- REMOVED
    }

    // --- NEW SECTION FOR CRAFTING STATION DETECTION ---

    /// <summary>
    /// This is a built-in Unity function that is called automatically
    /// when this object's collider enters another collider marked as a "Trigger".
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object we collided with has a CraftingStationIdentifier script on it.
        CraftingStationIdentifier station = other.GetComponent<CraftingStationIdentifier>();

        // If it does, it's a crafting station!
        if (station != null)
        {
            Debug.Log("Entered crafting station area: " + station.stationType);
            // Tell the CraftingManager what our current station is.
            craftingManager.SetCurrentCraftingStation(station.stationType);
        }
    }

    /// <summary>
    /// This is a built-in Unity function that is called automatically
    /// when this object's collider leaves a Trigger zone.
    /// </summary>
    private void OnTriggerExit2D(Collider2D other)
    {
        // Check if the object we are leaving is a crafting station.
        CraftingStationIdentifier station = other.GetComponent<CraftingStationIdentifier>();

        // If it is...
        if (station != null)
        {
            Debug.Log("Left crafting station area: " + station.stationType);
            // Tell the CraftingManager we are no longer at a station.
            craftingManager.SetCurrentCraftingStation(CraftingStation.None);
        }
    }
}

