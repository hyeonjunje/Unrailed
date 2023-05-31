using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MyItem : MonoBehaviour
{
    [SerializeField] protected EItemType itemType;
    public EItemType ItemType => itemType;
    protected virtual void RePosition(Transform parent, Vector3 pos)
    {
        transform.SetParent(parent);
        transform.localPosition = pos;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }
}
