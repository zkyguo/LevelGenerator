#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public static class MatchGameViewToSceneView
{
    static MatchGameViewToSceneView()
    {
        EditorApplication.playModeStateChanged += SyncCamera;
    }

    static void SyncCamera(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode || state == PlayModeStateChange.EnteredPlayMode)
        {
            Camera gameCamera = Camera.main; // assuming you want to sync the main camera, change this if not
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView != null && gameCamera != null)
            {
                gameCamera.transform.position = sceneView.camera.transform.position;
                gameCamera.transform.rotation = sceneView.camera.transform.rotation;
            }
        }
    }
}
#endif