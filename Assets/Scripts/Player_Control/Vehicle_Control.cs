#region includes
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endregion

public class Vehicle_Control : MonoBehaviour
{
    #region Public Variables
    [Header("References")]
    public GameObject trackManager;
    public GameObject UIManager;

    [Header("Lap completion")]
    public bool completedLap;
    public int lastCheckpoint;
    #endregion

    #region Private Variables
    private bool hasCrashed;
    private float speed;
    private float verticalInput;
    private float horizontalInput;
    private int maxCheckpoint;
    #endregion

    /// <summary>
    /// Sets up the object to default values before the first frame
    /// </summary>
    void Start()
    {
        InitialisePlayer();
    }

    /// <summary>
    /// Handles inputs at a regular intervale
    /// </summary>
    void FixedUpdate()
    {
        // Handle taking player control
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");

        accelerate(verticalInput);
        turn(horizontalInput);

        // Move forward based on speed
        this.transform.position += this.transform.rotation * Vector3.forward * this.speed * Time.deltaTime;

        // Updates its position relative to the track
        this.updateCheckpoint();

        // Stops the vehicle if it has crashed
        checkCrashed();
    }

    /// <summary>
    /// Reverts the player back to default values
    /// </summary>
    public void InitialisePlayer()
    {
        this.maxCheckpoint = trackManager.GetComponent<Track_Manager_Script>().track.maxIndex;

        // The vehicle starts without having passed a checkpoint
        lastCheckpoint = 0;

        this.completedLap = false;
        this.verticalInput = 0f;
        this.horizontalInput = 0f;

        // The vehicle starts stationary
        this.speed = 0.0f;

        // The vehicle starts at the origin
        this.transform.position = new Vector3(-30f, 5f, -5f);

        // The vehicle starts facing to the right
        this.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        this.hasCrashed = false;
    }

    /// <summary>
    /// Attempts to accelerate the vehicle, unless it's already at max speed
    /// </summary>
    /// <param name="inputMultiplier">The value of the user input</param>
    void accelerate(float inputMultiplier)
    {
        // Two values that represent the rate of acceleration, and max velocity respectively
        float accelerationConstant = 0.2f;
        float maxSpeed = 10.0f;

        // Needs to be altered to feel good
        float deceleration = 0.04f;

        // The vehicle slows down if the user isn't inputting
        if (inputMultiplier == 0)
        {
            this.speed -= deceleration;
        }

        // Updates the speed
        this.speed += accelerationConstant * inputMultiplier;

        // Ensures that the vehicle is not going too fast forwards OR backwards
        this.speed = Mathf.Min(this.speed, maxSpeed);
        this.speed = Mathf.Max(this.speed, -maxSpeed);
    }

    /// <summary>
    /// Rotates the vehicle for turns
    /// </summary>
    /// <param name="inputMultiplier">The horizontal user input</param>
    void turn(float inputMultiplier)
    {
        // Sets a value that represents how quickly the vehicle should turn
        float turnConstant = 2.0f;

        // Turns the vehicle
        this.transform.Rotate(new Vector3(0, turnConstant * inputMultiplier, 0), Space.Self);
    }

    // Obsolete
    /*
    /// <summary>
    /// Checks if the vehicle has crashed, and if it has, then disables the object
    /// </summary>
    private void checkCrashed()
    {
        if (this.hasCrashed)
        {
            this.gameObject.GetComponent<Vehicle_Control>().enabled = false;
        }
    }
    */

    /// <summary>
    /// When the vehicle collides with a wall, sets hasCrashed to true
    /// </summary>
    /// <param name="other">The collider of the object that the vehicle collided with</param>
    void OnTriggerEnter(Collider other)
    {
        List<string> Walls = new List<string>()
        {
            "North",
            "East",
            "South",
            "West"
        };
        if (Walls.Contains(other.name))
        {
            this.hasCrashed = true;
            HandleCrash();
        }
    }

    /// <summary>
    /// When it has crashed, enables the failure menu, and resets the player
    /// </summary>
    private void HandleCrash()
    {
        UIManager.GetComponent<Failure_Menu_Script>().EnableMenu();
        this.InitialisePlayer();
    }

    /// <summary>
    /// Casts a ray downwards, and if the floor is a higher position, updates the vehicles position
    /// around the track.
    /// </summary>
    private void updateCheckpoint()
    {
        RaycastHit hit;
        Physics.Raycast(this.transform.position, Quaternion.Euler(90, 0, 0) * Vector3.forward, out hit);
        // Debug.DrawRay(this.transform.position, Quaternion.Euler(90, 0, 0) * Vector3.forward);

        if (hit.transform.gameObject.name.Split(' ')[1] == (lastCheckpoint + 1).ToString())
        {
            lastCheckpoint = int.Parse(hit.transform.gameObject.name.Split(' ')[1]);
        }
        else if (hit.transform.gameObject.name.Split(' ')[1] == "1" && lastCheckpoint == this.maxCheckpoint)
        {
            lastCheckpoint = int.Parse(hit.transform.gameObject.name.Split(' ')[1]);
            completedLap = true;
        }
        else
        {
            lastCheckpoint = int.Parse(hit.transform.gameObject.name.Split(' ')[1]);
        }
    }
}
