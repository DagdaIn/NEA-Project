#region includes
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endregion

public class Follow_Player : MonoBehaviour
{
    #region public variables
    public GameObject player;
    public Vector3 displacement;
    public bool followRotation;
    #endregion

    #region methods

    #region unity methods
    /// <summary>
    /// Sets the colour of the player to be the focused colour
    /// </summary>
    void Start()
    {
        UpdatePlayerColour(player, true);
    }

    /// <summary>
    /// Every fixed interval gets the camera to move behind the player
    /// </summary>
    void FixedUpdate()
    {
        // The object will follow the players rotation, and hence be at a fixed distance behind it's local rotation
        if (followRotation)
        {
            this.transform.position = player.transform.position + player.transform.rotation * displacement;
            this.transform.rotation = player.transform.rotation * Quaternion.Euler(30, 0, 0);
        }

        // The object will simply follow behind by the global displacement
        else
        {
            this.transform.position = player.transform.position + displacement; 
        }

    }

    /// <summary>
    /// Every frame checks if the user clicked on a new vehicle to focus
    /// </summary>
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject clicked = GetClickedObject();

            // ignore if the object clicked is the floor
            if (clicked == null || clicked.name[0] == 'F')
            {
                return;
            }

            // List of names associated with floor pieces
            List<string> Tiles = new List<string>()
            {
                "Floor",
                "North",
                "South",
                "East",
                "West"
            };

            if (clicked != null && !Tiles.Contains(clicked.name.Split(' ')[0]))
            {
                UpdatePlayerColour(player, false);

                player = clicked;

                UpdatePlayerColour(clicked, true);
            }
        }
    }
    #endregion

    /// <summary>
    /// Changes the colour between red and white
    /// </summary>
    /// <param name="player">The gameObject to be focused</param>
    /// <param name="isFocused">Whether or not the target is being focused on, or being defocused</param>
    private void UpdatePlayerColour(GameObject player, bool isFocused)
    {
        Color color = player.GetComponent<Renderer>().material.color;
        
        if (isFocused)
        {
            color.r = 1.0f;
            //color.g = 1.0f;
            //color.b = 1.0f;
        }
        else
        {
            color.r = 0.0f;
            //color.g = 0.0f;
            //color.b = 0.0f;
        }

        player.GetComponent<Renderer>().material.color = color;
    }

    /// <summary>
    /// Casts a ray from the mouse to the position on the screen
    /// </summary>
    /// <returns>The gameobject that was clicked on by the user</returns>
    private GameObject GetClickedObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 200))
        {
            return hit.collider.gameObject;
        }

        return null;
    }
    #endregion
}
