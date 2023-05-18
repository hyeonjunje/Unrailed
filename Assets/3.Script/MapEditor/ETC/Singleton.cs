using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance = null;


    public static T Iinstance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = GameObject.Find(typeof(T).Name);

                if (!obj.TryGetComponent(out instance))
                {
                    obj = new GameObject(typeof(T).Name);
                    instance = obj.AddComponent<T>();
                }
                else
                {
                    obj.TryGetComponent(out instance);
                }
            }
            return instance;
        }
    }
}
