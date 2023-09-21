#region includes
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#endregion

public class ManageTimer : MonoBehaviour
{
    #region variables
    public GameObject timerText;
    private float raceTimer;
    #endregion

    #region MonoBehaviourSubroutines
    void Start()
    {
        raceTimer = 0f;
        timerText.GetComponent<TextMeshProUGUI>().SetText($"<mspace=0.7em>{TimeSpan.FromSeconds(raceTimer):g}".PadRight(25, '0'));
    }

    void Update()
    {
        raceTimer += Time.deltaTime;
        timerText.GetComponent<TextMeshProUGUI>().SetText($"<mspace=0.7em>{TimeSpan.FromSeconds(raceTimer):g}".PadRight(25, '0'));
        
    }
    #endregion

    
}
