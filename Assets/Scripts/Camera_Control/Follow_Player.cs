using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow_Player : MonoBehaviour
{
    public GameObject player;
    public Vector3 displacement;
    public bool followRotation;

    // Start is called before the first frame update
    void Start()
    {
        UpdatePlayerColour(player, true);
    }

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

            List<string> Tiles = new List<string>()
            {
                "Floor",
                "North",
                "South",
                "East",
                "West"
            };

            if (clicked != null && !Tiles.Contains(clicked.name))
            {
                UpdatePlayerColour(player, false);

                player = clicked;

                UpdatePlayerColour(clicked, true);
            }
        }
    }

    private void UpdatePlayerColour(GameObject player, bool isFocussed)
    {
        Color color = player.GetComponent<Renderer>().material.color;
        
        if (isFocussed)
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
}
