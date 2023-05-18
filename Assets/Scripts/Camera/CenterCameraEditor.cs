using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CenterCameraEditor : ScriptableObject
{
    [MenuItem("Tools/Center Camera on Cubes")]
    static void CenterCameraOnCubes()
    {
        GameObject[] cubes = GameObject.FindGameObjectsWithTag("Room"); // Assuming your cubes have the tag "Cube"

        if (cubes.Length == 0)
        {
            Debug.Log("No cubes found!");
            return;
        }

        // Calculate the center point
        Vector3 center = new Vector3();
        foreach (GameObject cube in cubes)
        {
            center += cube.transform.position;
        }
        center /= cubes.Length;

        // Calculate the bounds
        Bounds bounds = new Bounds(center, Vector3.zero);
        foreach (GameObject cube in cubes)
        {
            bounds.Encapsulate(cube.GetComponent<Renderer>().bounds);
        }

        // Get the scene view
        SceneView sceneView = SceneView.lastActiveSceneView;
        if (sceneView != null)
        {
            // Position the camera
            sceneView.pivot = bounds.center;
            sceneView.rotation = Quaternion.Euler(90, 0, 0); // Rotate to top down view

            // Calculate the correct distance to cover the bounds
            float cameraDistance = Mathf.Max(bounds.size.x, bounds.size.y);
            cameraDistance += 10; // Adding a bit more distance to cover all cubes

            // Set the camera distance
            sceneView.size = cameraDistance;
            sceneView.orthographic = true; // Set to orthographic
            sceneView.Repaint(); // Update the scene view
        }
    }
}
