using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AxeUI : UIBase
{
    private void Start()
    {
        _item = FindObjectOfType<Item_Axe>();
    }
    protected override void Update()
    {
        base.Update();
    }
}
