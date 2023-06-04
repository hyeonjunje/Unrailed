using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Axe : AI_Item
{
    private int id = 1;

    public bool IsOn { get; protected set; } = false;

    public void PickUp()
    {
        IsOn = !IsOn;
        Debug.Log($"도끼는 지금 {(IsOn ? "들려있어용":"바닥이에용")}");
    }

    public override int Id()
    {
        return id;
    }

}
