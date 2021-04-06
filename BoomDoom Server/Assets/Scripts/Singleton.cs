using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T: Component
{
    protected static T Instance { get; private set; }
    protected virtual void Awake()
    {
        if(Instance == null)
            Instance = GetComponent<T>();
        else
        {
            Destroy(Instance.gameObject);
            GameObject obj = new GameObject();
            T comp = obj.AddComponent<T>();
            Instance = comp;
        }
    }  
}
