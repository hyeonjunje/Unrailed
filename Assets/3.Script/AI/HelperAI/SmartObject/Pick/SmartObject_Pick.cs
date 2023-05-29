using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartObject_Pick : SmartObject
{
    public Transform RightHand;
    public bool IsOn { get; protected set; } = true;

    public void Pick()
    {
        IsOn = !IsOn;
        transform.SetParent(RightHand);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
}
