
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Sirenix.OdinInspector;

[ExecuteInEditMode]
public class SingletonManager : SerializedMonoBehaviour
{
    public Dictionary<System.Type, MonoBehaviour> singletons = new Dictionary<System.Type, MonoBehaviour>();
    public static SingletonManager Instance;
    void OnEnable()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorApplication.update += CollectSingletons;
            if(Instance == null)
            {
                Instance = this;
            }
        }
    }

    void OnDisable()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorApplication.update -= CollectSingletons;
            if (Instance != null)
            {
                Instance = null;
            }
        }
    }

    void CollectSingletons()
    {
        singletons.Clear();
        Scene activeScene = SceneManager.GetActiveScene();
        GameObject[] rootGameObjects = activeScene.GetRootGameObjects();
        foreach (var rootGameObject in rootGameObjects)
        {
            MonoBehaviour singleton = rootGameObject.GetComponent<Singleton>();
            if (singleton != null && singleton != this)
            {
                singletons[singleton.GetType()] = singleton;
            }
        }
    }

    public T GetSingleton<T>() where T : MonoBehaviour
    {
        System.Type type = typeof(T);
        if (singletons.ContainsKey(type))
        {
            return (T)singletons[type];
        }
        else
        {
            return null;
        }
    }
}
#endif
