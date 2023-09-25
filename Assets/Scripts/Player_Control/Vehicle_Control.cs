using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle_Control : MonoBehaviour
{
    [Header("References")]
    public GameObject trackManager;
    public GameObject UIManager;

    private bool hasCrashed;
    private float speed;
    private float verticalInput;
    private float horizontalInput;
    public int lastCheckpoint;
    private int maxCheckpoint;
    public bool completedLap;

    // Start is called before the first frame update
    void Start()
    {
        InitialisePlayer();
    }

    // Update is called once per frame
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

    // Accelerate, unless the vehicle is at the max velocity
    void accelerate(float inputMultiplier)
    {
        // Two values that represent the rate of acceleration, and max velocity respectively
        float accelerationConstant = 0.2f;
        float maxSpeed = 10.0f;

        // Updates the speed
        this.speed += accelerationConstant * inputMultiplier;

        // Ensures that the vehicle is not going too fast forwards OR backwards
        this.speed = Mathf.Min(this.speed, maxSpeed);
        this.speed = Mathf.Max(this.speed, -maxSpeed);
    }

    // Rotate the vehicle based on user input
    void turn(float inputMultiplier)
    {
        // Sets a value that represents how quickly the vehicle should turn
        float turnConstant = 2.0f;

        // Turns the vehicle
        this.transform.Rotate(new Vector3(0, turnConstant * inputMultiplier, 0), Space.Self);
    }

    // Stops vehicle when it has crashed
    private void checkCrashed()
    {
        if (this.hasCrashed)
        {
            this.gameObject.GetComponent<Vehicle_Control>().enabled = false;
        }
    }

    // Calculates whether the vehicle has crashed in to a wall
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

    private void HandleCrash()
    {
        UIManager.GetComponent<Failure_Menu_Script>().EnableMenu();
        this.InitialisePlayer();
    }

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
