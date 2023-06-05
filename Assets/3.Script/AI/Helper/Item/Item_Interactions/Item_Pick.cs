using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Pick : AI_Item
{
    private int id = 2;
    public bool IsOn { get; protected set; } = false;

    public override void PickUp()
    {
        IsOn = !IsOn;
        Debug.Log($"곡괭이는 지금 {(IsOn ? "들려있어용" : "바닥이에용")}");
    }


    public override int Id()
    {
        return id;
    }
}
