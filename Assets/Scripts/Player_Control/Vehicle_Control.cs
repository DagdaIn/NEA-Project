using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle_Control : MonoBehaviour
{
    private bool hasCrashed;
    private float speed;
    private float verticalInput;
    private float horizontalInput;

    // Start is called before the first frame update
    void Start()
    {
        // The vehicle starts stationary
        this.speed = 0.0f;

        // The vehicle starts at the origin
        this.transform.position = new Vector3(-30f, 5f, 0f);
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

        // Stops the vehicle if it has crashed
        checkCrashed();
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
        }
    }
}
