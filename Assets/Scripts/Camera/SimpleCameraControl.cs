using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SimpleCameraControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Camera cameraToControl = this.gameObject.GetComponent<Camera>();
        GameObject[] rooms = GameObject.FindGameObjectsWithTag("Room");

        if (rooms.Length == 0)
        {
            Debug.Log("No rooms found!");
            return;
        }

        // Calculate the center point
        Vector3 center = new Vector3();
        foreach (GameObject room in rooms)
        {
            center += room.transform.position;
        }
        center /= rooms.Length;

        // Calculate the bounds
        Bounds bounds = new Bounds(center, Vector3.zero);
        foreach (GameObject room in rooms)
        {
            bounds.Encapsulate(room.transform.GetChild(0).GetComponent<Renderer>().bounds);
        }

        if (cameraToControl != null)
        {
            // Position the camera
            cameraToControl.transform.position = bounds.center + new Vector3(0, bounds.size.y, 0);
            cameraToControl.transform.rotation = Quaternion.Euler(90, 0, 0);

            // Adjust the camera's field of view or orthographic size to cover the bounds
            // We'll use a simple heuristic to try to cover the bounds by setting the field of view 
            // according to the largest dimension of the bounds.
            if (cameraToControl.orthographic)
            {
                cameraToControl.orthographicSize = Mathf.Max(bounds.size.x, bounds.size.y);
            }
            else
            {
                cameraToControl.fieldOfView = Mathf.Max(bounds.size.x, bounds.size.y);
            }
        }
    }
}
