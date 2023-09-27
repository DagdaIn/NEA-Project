#region includes
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endregion

/// <summary>
/// Testing class for the track system
/// </summary>
public class Testing_Tile_Control : MonoBehaviour
{
    public GameObject[] Walls;

    public void Start()
    {
        Walls[0].SetActive(false);
        
        Walls[0].SetActive(true);
    }
}
