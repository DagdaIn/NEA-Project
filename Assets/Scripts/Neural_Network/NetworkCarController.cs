#region includes
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endregion

public class NetworkCarController : MonoBehaviour
{
    #region Public Variables
    public GameObject trackManager;

    public float acceleration = 0.02f;
    public float turning = 0.02f;

    [Range(-1f, 1f)]
    public float forwardInput, horizontalInput;
    
    public float timeSinceStart = 0f;
    
    [Header("Fitness")]
    public float overallFitness;
    public float avgSpeedMultiplier = 0.2f;
    public float checkpointMultiplier = 10f;
    
    [Header ("Network Options")]
    public int layers = 3;
    public int neurons = 30;
    #endregion

    #region Private Variables
    private Vector3 startPosition, startRotation;
    private NeuralNetwork network;

    private float lastCheckpoint;
    private int maxCheckpoint;
    private int numCheckpointsPassed = 0;

    private Vector3 lastPosition;
    private float totalDistanceTravelled;
    private float avgSpeed;

    private float[] Sensors;
    #endregion

    #region Initialisation
    /// <summary>
    /// When created, this vehicle should store its start postion, and create a new network
    /// </summary>
    private void Awake()
    {
        startPosition = transform.position;
        startRotation = transform.eulerAngles;
        Sensors = new float[5];

        network = new NeuralNetwork();
        network.Initialise(layers, neurons);
    }

    /// <summary>
    /// Sets the max checkpoint, to properly handle lap completion
    /// </summary>
    private void Start()
    {
        this.maxCheckpoint = trackManager.GetComponent<Track_Manager_Script>().track.maxIndex;
    }

    /// <summary>
    /// Resets the vehicle, and sets the network to be <paramref name="_network"/>
    /// </summary>
    /// <param name="_network">The network that should control the vehicle</param>
    public void SetCurrentNetworkAndReset(NeuralNetwork _network)
    {
        Reset();
        network = _network;
    }
    
    /// <summary>
    /// Resets all the values of the vehicle
    /// </summary>
    public void Reset()
    {
        // Initialises the network again
        network.Initialise(layers, neurons);

        // Manages the fitness based values
        timeSinceStart = 0f;
        avgSpeed = 0f;
        lastPosition = startPosition;
        overallFitness = 0f;

        // Handles track progress
        lastCheckpoint = 0;
        numCheckpointsPassed = 0;

        // Resets vehicles position
        transform.position = startPosition;
        transform.eulerAngles = startRotation;
    }
    #endregion

    #region Input Management
    private void UpdateCheckpoint()
    {
        RaycastHit hit;
        Physics.Raycast(this.transform.position, Quaternion.Euler(90, 0, 0) * Vector3.forward, out hit);
        // Debug.DrawRay(this.transform.position, Quaternion.Euler(90, 0, 0) * Vector3.forward);

        // Handles the checkpoint progress for the vehicle
        if (hit.transform.gameObject.name.Split(' ')[1] == (lastCheckpoint + 1).ToString())
        {
            lastCheckpoint = int.Parse(hit.transform.gameObject.name.Split(' ')[1]);
            numCheckpointsPassed ++;
        }
        else if (hit.transform.gameObject.name.Split(' ')[1] == "1" && lastCheckpoint == this.maxCheckpoint)
        {
            lastCheckpoint = int.Parse(hit.transform.gameObject.name.Split(' ')[1]);
            numCheckpointsPassed ++;
        }
        else
        {
            lastCheckpoint = int.Parse(hit.transform.gameObject.name.Split(' ')[1]);
        }
    }

    // At a fixed rate, handle moving the vehicle
    private void FixedUpdate()
    {
        // Updates the values of the last position, and input sensors
        InputSensors();
        lastPosition = transform.position;

        (forwardInput, horizontalInput) = network.RunNetwork(Sensors);

        MoveCar(forwardInput, horizontalInput);

        UpdateCheckpoint();

        timeSinceStart += Time.deltaTime;

        CalculateFitness();
        // forwardInput = 0f;
        // horizontalInput = 0f;
    }

    /// <summary>
    /// Retrieves each sensors new value
    /// </summary>
    private void InputSensors()
    {
        Vector3[] directions = new Vector3[]
        {
            Vector3.Normalize(-transform.right), // Left
            Vector3.Normalize(transform.forward - transform.right), // Forward and left
            Vector3.Normalize(transform.forward), // Forward
            Vector3.Normalize(transform.forward + transform.right), // Forward and right
            Vector3.Normalize(transform.right) // Right
        };

        for (int i = 0; i < 5; i++)
        {
            Sensors[i] = GetInput(directions[i]);
        }
    }

    /// <summary>
    /// Returns the distance between the car and the wall
    /// </summary>
    /// <param name="direction">The direction to check for distance</param>
    /// <returns>A float representing distance</returns>
    private float GetInput(Vector3 direction)
    {
        Ray r = new Ray(transform.position, direction);
        RaycastHit hit;

        float result = 0f;

        if (Physics.Raycast(r, out hit))
        {
            // Normalize the sensor values
            result = hit.distance / 30;

            // Debug.Log(result);
            // Debug.DrawLine(r.origin, hit.point, Color.red);
        }

        return result;
    }
    #endregion

    #region Failure
    /// <summary>
    /// Moves on to the next network
    /// </summary>
    private void Death()
    {
        GameObject.FindObjectOfType<GeneticAlgorithmManager>().Death(overallFitness, network);
    }

    /// <summary>
    /// Kills the network when it hits a wall
    /// </summary>
    /// <param name="other">The collider of the object that has been collied with</param>
    private void OnTriggerEnter (Collider other)
    {
        Death();
    }
    #endregion

    /// <summary>
    /// Calculates the current fitness of the vehicle, and kills it if it is too low after too long
    /// </summary>
    private void CalculateFitness()
    {
        totalDistanceTravelled += Vector3.Distance(transform.position, lastPosition);
        avgSpeed = totalDistanceTravelled / timeSinceStart;

        overallFitness = (numCheckpointsPassed * checkpointMultiplier);

        // Kill the network if it is not good enough
        if (timeSinceStart > 20 && overallFitness < 40)
        {
            Death();
        }
    }

    private Vector3 input;
    /// <summary>
    /// Handles vehicle movement
    /// </summary>
    /// <param name="verticalInput">The forward/backward input</param>
    /// <param name="horizontalInput">The left/right input</param>
    public void MoveCar (float verticalInput, float horizontalInput)
    {
        // Linearly interpolates between current position, by an amount based on forward input
        input = Vector3.Lerp(Vector3.zero, new Vector3(0, 0, verticalInput * 10f), acceleration);
        input = transform.TransformDirection(input);
        transform.position += input;

        // Rotates the vehicle
        transform.eulerAngles += new Vector3(0, (horizontalInput * 90) * turning, 0);
    }

    private float Sigmoid (float input)
    {
        return (1/(1 + Mathf.Exp(-input)));
    }
}
