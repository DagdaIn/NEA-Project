#region includes
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;
#endregion

public class Network_Options_Script : MonoBehaviour
{
    #region publicVariables
    [Header("References")]
    public GameObject geneticAlgorithmManager;
    public GameObject vehicle;
    public GameObject[] optionsControls;

    public Dictionary<string, float> optionsValues;


    #endregion


    #region Setup
    public void Awake()
    {
        InitialiseOptionsValues();
        
        ShowCurrentSettings();
    }

    public void InitialiseOptionsValues()
    {
        optionsValues = new Dictionary<string, float>()
        {
            {"Mutation Rate", 0f},
            {"Survival Rate", 0f},
            {"Acceleration", 0f},
            {"Turning", 0f},
        };
    }

    public void LoadCurrentSettings()
    {
        optionsValues["Mutation Rate"] = geneticAlgorithmManager.GetComponent<GeneticAlgorithmManager>().mutationRate;
        optionsValues["Survival Rate"] = geneticAlgorithmManager.GetComponent<GeneticAlgorithmManager>().bestAgentSelection;
        optionsValues["Acceleration"] = vehicle.GetComponent<NetworkCarController>().acceleration;
        optionsValues["Mutation Rate"] = vehicle.GetComponent<NetworkCarController>().turning;
    }

    public void ShowCurrentSettings()
    {
        LoadCurrentSettings();

        optionsControls[0].GetComponent<Slider>().value = optionsValues["Mutation Rate"];
        optionsControls[1].GetComponent<Slider>().value = optionsValues["Survival Rate"];
        optionsControls[2].GetComponent<Slider>().value = optionsValues["Acceleration"];
        optionsControls[3].GetComponent<Slider>().value = optionsValues["Turning"];
    }
    #endregion

    #region Confirmation
    public void SetMutationRate(float value)
    {
        geneticAlgorithmManager.GetComponent<GeneticAlgorithmManager>().mutationRate = value;
    }

    public void SetSurvivalRate(float value)
    {
        geneticAlgorithmManager.GetComponent<GeneticAlgorithmManager>().bestAgentSelection = (int)value;
    }

    public void SetAcceleration(float value)
    {
        vehicle.GetComponent<NetworkCarController>().acceleration = value;
    }

    public void SetTurning(float value)
    {
        vehicle.GetComponent<NetworkCarController>().turning = value;
    }
    #endregion
}
