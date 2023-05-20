

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

    void Awake()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        if (Application.isPlaying)
        {
            CollectSingletons();
        }
        else
        {
#if UNITY_EDITOR
            EditorApplication.update += CollectSingletons;
#endif
            if (Instance == null)
            {
                Instance = this;
            }
        }
    }

    void OnDisable()
    {
        if (Application.isPlaying)
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
        else
        {
#if UNITY_EDITOR
            EditorApplication.update -= CollectSingletons;
            if (Instance != null)
            {
                Instance = null;
            }
#endif
        }
    }

    void Update()
    {
        if (Application.isPlaying)
        {
            CollectSingletons();
        }
    }

    void CollectSingletons()
    {
        singletons.Clear();
        Scene activeScene = SceneManager.GetActiveScene();
        GameObject[] rootGameObjects = activeScene.GetRootGameObjects();
        foreach (var rootGameObject in rootGameObjects)
        {
            MonoBehaviour singleton = rootGameObject.GetComponent<MonoBehaviour>();
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

