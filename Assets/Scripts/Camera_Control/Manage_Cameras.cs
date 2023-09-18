using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manage_Cameras : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Camera trackCamera;

    void Awake()
    {
        UseTrackCamera();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SwapCamera();
        }
    }

    void UseTrackCamera()
    {
        trackCamera.enabled = true;
        playerCamera.enabled = false;
    }

    void UsePlayerCamera()
    {
        playerCamera.enabled = true;
        trackCamera.enabled = false;
    }

    void SwapCamera()
    {
        if (trackCamera.isActiveAndEnabled)
        {
            UsePlayerCamera();
        }
        else
        {
            UseTrackCamera();
        }
    }
}
