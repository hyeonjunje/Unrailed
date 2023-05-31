using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Item : MonoBehaviour
{
    [SerializeField] protected Transform _InteractionMarker;
    public Vector3 InteractionPoint => _InteractionMarker != null ? _InteractionMarker.position : transform.position;
    void Start()
    {
        ItemManager.Instance.RegisterItem(this);
    }

    private void OnDestroy()
    {
        ItemManager.Instance.DeregisterItem(this);
    }

}
