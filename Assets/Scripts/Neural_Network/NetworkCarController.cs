using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCarController : MonoBehaviour
{
    private Vector3 startPosition, startRotation;
    private NeuralNetwork network;

    private float lastCheckpoint;
    private int maxCheckpoint;
    public GameObject trackManager;
    private int numCheckpointsPassed = 0;

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

    private Vector3 lastPosition;
    private float totalDistanceTravelled;
    private float avgSpeed;

    private float ASensor, BSensor, CSensor, DSensor, ESensor;

    private void Awake()
    {
        startPosition = transform.position;
        startRotation = transform.eulerAngles;

        network = new NeuralNetwork();
        network.Initialise(layers, neurons);
    }

    private void Start()
    {
        this.maxCheckpoint = trackManager.GetComponent<Track_Manager_Script>().track.maxIndex;
    }

    public void ResetWithNetwork(NeuralNetwork _network)
    {
        network = _network;
        Reset();
    }

    private void Death()
    {
        GameObject.FindObjectOfType<GeneticAlgorithmManager>().Death(overallFitness, network);
    }

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

    private void OnTriggerEnter (Collider other)
    {
        Death();
    }

    private void FixedUpdate()
    {
        InputSensors();
        lastPosition = transform.position;

        (forwardInput, horizontalInput) = network.RunNetwork(ASensor, BSensor, CSensor, DSensor, ESensor);

        MoveCar(forwardInput, horizontalInput);

        UpdateCheckpoint();

        timeSinceStart += Time.deltaTime;

        CalculateFitness();
        // forwardInput = 0f;
        // horizontalInput = 0f;
    }

    private void CalculateFitness()
    {
        totalDistanceTravelled += Vector3.Distance(transform.position, lastPosition);
        avgSpeed = totalDistanceTravelled / timeSinceStart;

        overallFitness = /*(totalDistanceTravelled * distanceMultiplier) + */ (avgSpeed * avgSpeedMultiplier) + /* (((ASensor + BSensor + CSensor + DSensor + ESensor) / 5) * sensorMultiplier) + */ (numCheckpointsPassed * checkpointMultiplier);

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

    private void InputSensors()
    {
        Vector3 a = Vector3.Normalize(transform.forward + transform.right);
        Vector3 b = Vector3.Normalize(transform.forward);
        Vector3 c = Vector3.Normalize(transform.forward - transform.right);
        Vector3 d = Vector3.Normalize(transform.right);
        Vector3 e = Vector3.Normalize(-transform.right);

        Ray r = new Ray(transform.position, a);
        RaycastHit hit;

        if (Physics.Raycast(r, out hit))
        {
            // Normalize the sensor values
            ASensor = hit.distance / 30;

            Debug.DrawLine(r.origin, hit.point, Color.red);
            //print($"A: {ASensor}");
        }

        r.direction = b;

        if (Physics.Raycast(r, out hit))
        {
            // Normalize the sensor values
            BSensor = hit.distance / 30;

            Debug.DrawLine(r.origin, hit.point, Color.red);
            //print($"B: {BSensor}");
        }

        r.direction = c;

        if (Physics.Raycast(r, out hit))
        {
            // Normalize the sensor values
            CSensor = hit.distance / 30;

            Debug.DrawLine(r.origin, hit.point, Color.red);
            //print($"C: {CSensor}");
        }

        r.direction = d;

        if (Physics.Raycast(r, out hit))
        {
            // Normalize the sensor values
            DSensor = hit.distance / 30;

            Debug.DrawLine(r.origin, hit.point, Color.red);
            //print($"D: {DSensor}");
        }

        r.direction = e;

        if (Physics.Raycast(r, out hit))
        {
            // Normalize the sensor values
            ESensor = hit.distance / 30;

            Debug.DrawLine(r.origin, hit.point, Color.red);
            //print($"E: {ESensor}");
        }
    }

    private Vector3 input;
    public void MoveCar (float v, float h)
    {
        input = Vector3.Lerp(Vector3.zero, new Vector3(0, 0, v*11.4f), 0.02f);
        input = transform.TransformDirection(input);
        transform.position += input;

        transform.eulerAngles += new Vector3(0, h * 90 * 0.02f, 0);
    }
}
