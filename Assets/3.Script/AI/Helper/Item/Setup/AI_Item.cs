using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Item : MonoBehaviour
{
    [SerializeField] public WorldResource.EType Type = WorldResource.EType.Wood;
    [SerializeField] protected Transform _InteractionMarker;
    public Vector3 InteractionPoint => _InteractionMarker != null ? _InteractionMarker.position : transform.position;
    protected List<BaseInteraction> CachedInteractions = null;

    public List<BaseInteraction> Interactions
    {
        get
        {
            if (CachedInteractions == null)
                CachedInteractions = new List<BaseInteraction>(GetComponents<BaseInteraction>());

            return CachedInteractions;
        }
    }
    void Start()
    {
        ItemManager.Instance.RegisterItem(this);
    }

    private void OnDestroy()
    {
        ItemManager.Instance.DeregisterItem(this);
    }

}
