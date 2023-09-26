using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// When enabled, objects must have transparent material
// This causes the meshes to collide and render weirdly
// Hence I opted not to turn this code on and instead 
// Decided to make it so the walls are shorter.
public class Transparent_Objects_In_Front_Of_Camera : MonoBehaviour
{
    [SerializeField] private List<GameObject> transparentObjects;
    [SerializeField] private List<GameObject> hitObjects;
    [SerializeField] private Transform FocussedVehicle;
    private Transform cameraTransform;

    void Start()
    {
        transparentObjects = new List<GameObject>();
        hitObjects = new List<GameObject>();

        cameraTransform = this.transform;
    }

    void FixedUpdate()
    {
        GetAllObjectsBetweenPlayerAndCamera();
    }

    private void GetAllObjectsBetweenPlayerAndCamera()
    {
        float distanceBetweenCameraAndPlayer = Vector3.Magnitude(cameraTransform.position - FocussedVehicle.position);

        Ray forwardRay = new Ray(cameraTransform.position, FocussedVehicle.position - cameraTransform.position);
        // Second ray required incase cameraTransform intersects object
        Ray backwardRay = new Ray(FocussedVehicle.position, cameraTransform.position - FocussedVehicle.position);

        RaycastHit[] forwardHits = Physics.RaycastAll(forwardRay, distanceBetweenCameraAndPlayer);
        RaycastHit[] backwardHits = Physics.RaycastAll(backwardRay, distanceBetweenCameraAndPlayer);

        hitObjects = new List<GameObject>();

        foreach (RaycastHit hit in forwardHits)
        {
            if (!transparentObjects.Contains(hit.collider.gameObject))
            {
                SetObjectAlpha(0.1f, hit.collider.gameObject);
                transparentObjects.Add(hit.collider.gameObject);
            }
            hitObjects.Add(hit.collider.gameObject);
        }

        foreach (RaycastHit hit in backwardHits)
        {
            if (!transparentObjects.Contains(hit.collider.gameObject))
            {
                SetObjectAlpha(0.1f, hit.collider.gameObject);
                transparentObjects.Add(hit.collider.gameObject);
            }
            if (!hitObjects.Contains(hit.collider.gameObject))
            {
                hitObjects.Add(hit.collider.gameObject);
            }
        }

        List<GameObject> temp = new List<GameObject>();

        foreach (GameObject obj in transparentObjects)
        {
            if (!hitObjects.Contains(obj))
            {
                SetObjectAlpha(1f, obj);
                temp.Add(obj);
            }
        }

        foreach (GameObject obj in temp)
        {
            transparentObjects.Remove(obj);
        }
    }

    private void SetObjectAlpha(float newAlpha, GameObject obj)
    {
        //obj.GetComponent<Renderer>().enabled = (newAlpha == 1);

        
        Color color = obj.GetComponent<Renderer>().material.color;
        color.a = newAlpha;
        color.r = newAlpha;
        color.g = newAlpha;
        obj.GetComponent<Renderer>().material.color = color;
        
    }
}
