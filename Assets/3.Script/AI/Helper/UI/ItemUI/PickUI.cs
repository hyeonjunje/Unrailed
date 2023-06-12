using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUI : UIBase
{
    private void Start()
    {
        _item = FindObjectOfType<Item_Pick>();
    }
    protected override void Update()
    {
        base.Update();
    }
}
