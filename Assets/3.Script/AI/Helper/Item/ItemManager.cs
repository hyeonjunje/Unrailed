using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; } = null;
    public List<AI_Item> RegisteredObjects { get; private set; } = new List<AI_Item>();

   

    private void Awake()
    {
        if(Instance!=null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

    }

    public void RegisterItem(AI_Item toRegister)
    {
        RegisteredObjects.Add(toRegister);
    }

    public void DeregisterItem(AI_Item toDeregister)
    {
        RegisteredObjects.Remove(toDeregister);
    }
}
