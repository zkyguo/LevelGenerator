using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class SceneGUIExample
{
    static SceneGUIExample()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        Handles.BeginGUI();
        if (GUI.Button(new Rect(10, 10, 100, 50), "View Mode"))
        {
            if (sceneView != null)
            {
                // Reset position
                sceneView.pivot = Vector3.zero;

                // Reset rotation
                sceneView.rotation = Quaternion.identity;

                // Reset zoom
                sceneView.size = 10;

                // Reset to perspective
                sceneView.orthographic = false;

                // Repaint the scene view
                sceneView.Repaint();
            }
        }
        else if (GUI.Button(new Rect(10, 70, 100, 50), "Edit Mode"))
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
        Handles.EndGUI();
    }
}