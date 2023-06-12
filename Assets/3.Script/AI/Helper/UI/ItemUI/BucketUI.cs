using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BucketUI : UIBase
{
    private void Start()
    {
        _item = FindObjectOfType<Item_Bucket>();
    }
    protected override void Update()
    {
        base.Update();
    }
}
