using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manage_Cameras : MonoBehaviour
{
    /// <summary>
    /// Stores the camera objects that will be toggled between.
    /// </summary>
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Camera trackCamera;

    /// <summary>
    /// Defines the subroutine used to switch between cameras
    /// </summary>
    private SwapCamera toggleCamera;

    /// <summary>
    /// Defaults the camera to be the track camera
    /// </summary>
    void Awake()
    {
        UseTrackCamera();
    }

    /// <summary>
    /// Swaps camera when the key 'T' is pressed
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            this.toggleCamera();
        }
    }

    /// <summary>
    /// Enables the top-down track camera, and disables the player-following camera
    /// </summary>
    void UseTrackCamera()
    {
        trackCamera.enabled = true;
        playerCamera.enabled = false;
        this.toggleCamera = UsePlayerCamera;
    }

    /// <summary>
    /// Enables the player-following character, and disables the top-down character
    /// </summary>
    void UsePlayerCamera()
    {
        playerCamera.enabled = true;
        trackCamera.enabled = false;
        this.toggleCamera = UseTrackCamera;
    }

    /// <summary>
    /// Swaps between player camera, and track camera
    /// </summary>
    private delegate void SwapCamera();
}
