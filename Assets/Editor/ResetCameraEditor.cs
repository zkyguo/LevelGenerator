using UnityEditor;
using UnityEngine;

public class ResetCameraEditor : ScriptableObject
{
    [MenuItem("Tools/Reset Scene View")]
    static void ResetSceneView()
    {
        SceneView sceneView = SceneView.lastActiveSceneView;
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
}