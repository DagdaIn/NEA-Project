#region includes
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endregion

public class NetworkCarController : MonoBehaviour
{
    #region Public Variables
    public GameObject trackManager;

    [Range(-1f, 1f)]
    public float forwardInput, horizontalInput;
    
    public float timeSinceStart = 0f;
    
    [Header("Fitness")]
    public float overallFitness;
    public float distanceMultiplier = 1.4f;
    public float avgSpeedMultiplier = 0.2f;
    public float sensorMultiplier = 0.1f;
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
    public void ResetWithNetwork(NeuralNetwork _network)
    {
        network = _network;
        Reset();
    }
    
    /// <summary>
    /// Resets all the values of the vehicle
    /// </summary>
    public void Reset()
    {
        network.Initialise(layers, neurons);

        timeSinceStart = 0f;
        totalDistanceTravelled = 0f;
        avgSpeed = 0f;
        lastPosition = startPosition;
        overallFitness = 0f;
        lastCheckpoint = 0;
        numCheckpointsPassed = 0;
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

    private void FixedUpdate()
    {
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
        Vector3 left = Vector3.Normalize(-transform.right);
        Vector3 forwardLeft = Vector3.Normalize(transform.forward - transform.right);
        Vector3 forward = Vector3.Normalize(transform.forward);
        Vector3 forwardRight = Vector3.Normalize(transform.forward + transform.right);
        Vector3 right = Vector3.Normalize(transform.right);

        Sensors[0] = GetInput(left);
        Sensors[1] = GetInput(forwardLeft);
        Sensors[2] = GetInput(forward);
        Sensors[3] = GetInput(forwardRight);
        Sensors[4] = GetInput(right);
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

        float retValue = 0f;

        if (Physics.Raycast(r, out hit))
        {
            // Normalize the sensor values
            retValue = hit.distance / 30;

            // Debug.DrawLine(r.origin, hit.point, Color.red);
        }

        return retValue;
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

        overallFitness = /*(totalDistanceTravelled * distanceMultiplier) + */ (avgSpeed * avgSpeedMultiplier) + /* (((Sensors[0] + Sensors[1] + Sensors[2] + Sensors[3] + Sensors[4]) / 5) * sensorMultiplier) + */ (numCheckpointsPassed * checkpointMultiplier);

        if (timeSinceStart > 20 && overallFitness < 40)
        {
            Death();
        }

        /*
        if (overallFitness >= 100)
        {
            // Save network
            Death();
        }
        */
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
        input = Vector3.Lerp(Vector3.zero, new Vector3(0, 0, verticalInput*11.4f), 0.02f);
        input = transform.TransformDirection(input);
        transform.position += input;

        // Rotates the vehicle
        transform.eulerAngles += new Vector3(0, (horizontalInput * 90) * 0.02f, 0);
    }
}
