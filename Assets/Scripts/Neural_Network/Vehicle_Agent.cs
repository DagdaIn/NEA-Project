using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Vehicle_Agent : MonoBehaviour
{
    #region Variables
    public Neural_Network network;
    private float velocity;
    private float fitness;
    private int lastCheckpoint;
    #endregion

    #region Constructors
    public Vehicle_Agent(Neural_Network _network)
    {
        this.network = _network;
        this.fitness = 0;
        this.lastCheckpoint = 0;
        this.velocity = 0f;
    }
    #endregion

    #region Methods
    private void Accelerate(float forwardInput)
    {
        float accelerationConstant = 2f;
        float maxVelocity = 10f;

        this.velocity += forwardInput * accelerationConstant;
        this.velocity = Math.Min(maxVelocity, this.velocity);
        
    }

    private void Turn(float horizontalInput)
    {
        // Debug.Log(horizontalInput);
        float turnConstant = 3f;
        if (horizontalInput > 0.2f)
        {
            this.transform.Rotate(new Vector3(0, horizontalInput * turnConstant, 0), Space.Self);
        }
        else if (horizontalInput < -0.2f)
        {
            this.transform.Rotate(new Vector3(0, horizontalInput * turnConstant, 0), Space.Self);
        }
    }

    private float[] getInputs()
    {
        RaycastHit hit;
        float[] inputs = new float[5];

        for (int i = 0; i < 5; i ++)
        {
            Physics.Raycast(this.transform.position, Quaternion.Euler(0, 30 * (i - 2), 0) * this.transform.rotation * Vector3.forward, out hit);
            inputs[i] = hit.distance;

            // Debug.DrawRay(this.transform.position, Quaternion.Euler(0, 30 * (i - 2), 0) * this.transform.rotation * Vector3.forward * hit.distance);
        }

        return inputs;
    }

    void FixedUpdate()
    {
        float[] inputs = getInputs();
        float[] outputs = network.CalculateOutputs(inputs);

        Accelerate(outputs[0]);
        Turn(outputs[1]);

        this.transform.position += this.transform.rotation * Vector3.forward * this.velocity * 0.01f;
        checkFitness();
        this.network.fitness = this.fitness;
    }

    private void checkFitness()
    {
        RaycastHit hit;
        Physics.Raycast(this.transform.position, Quaternion.Euler(90, 0, 0) * Vector3.forward, out hit);
        // Debug.DrawRay(this.transform.position, Quaternion.Euler(90, 0, 0) * Vector3.forward);

        if (hit.transform.gameObject.name.Split(' ')[1] == (lastCheckpoint + 1).ToString() || (hit.transform.gameObject.name.Split(' ')[1] == "0" && lastCheckpoint > 10))
        {
            fitness ++;
            lastCheckpoint = int.Parse(hit.transform.gameObject.name.Split(' ')[1]);
        }
        else
        {
            fitness --;
            lastCheckpoint = int.Parse(hit.transform.gameObject.name.Split(' ')[1]);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        List<string> Walls = new List<string>()
        {
            "North",
            "South",
            "East",
            "West"
        };

        if (Walls.Contains(other.name))
        {
            this.enabled = false;
        }
    }
    #endregion
}
