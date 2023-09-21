using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Network_Manager : MonoBehaviour
{
    #region Variables
    public GameObject networkPrefab;
    private Evolution_Manager networkGenerationManager;
    private GameObject[] vehicles;
    #endregion

    #region Constructors
    public Network_Manager()
    {
        this.networkGenerationManager = new Evolution_Manager();
        vehicles = new GameObject[this.networkGenerationManager.networks.Length];
    }

    void Start()
    {
        CreateVehicles();
    }
    #endregion

    #region Methods
    public void CreateVehicles()
    {
        for (int i = 0; i < this.networkGenerationManager.networks.Length; i++)
        {
            this.vehicles[i] = Instantiate(networkPrefab, new Vector3(-30f, 5f, 0f), Quaternion.Euler(0, 0, 0), this.transform);
            this.vehicles[i].GetComponent<Vehicle_Agent>().network = this.networkGenerationManager.networks[i];
        }
    }

    public void NextGeneration()
    {
        // This does not
        this.networkGenerationManager.NextGeneration();

        // This works
        for (int i = 0; i < this.vehicles.Length; i++)
        {
            Destroy(this.vehicles[i]);
            this.vehicles[i] = Instantiate(networkPrefab, new Vector3(-30f, 5f, 0f), Quaternion.Euler(0, 0, 0), this.transform);
            this.vehicles[i].GetComponent<Vehicle_Agent>().network = this.networkGenerationManager.networks[i];
        }
    }
    #endregion
}
