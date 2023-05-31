using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Helper_Action : Helper
{
    public Dictionary<KeyCode, System.Action> Order;

    private void Awake()
    {
        Order = new Dictionary<KeyCode, Action>();
        Init();

    }

    void Init()
    {
        Order[KeyCode.Alpha1] = () =>
        {
            TargetResource = WorldResource.EType.Wood;
            Debug.Log("나무 캐세용");
        };

        Order[KeyCode.Alpha2] = () =>
        {
            TargetResource = WorldResource.EType.Stone;
            Debug.Log("돌 캐세용");
        };

        Order[KeyCode.Alpha3] = () =>
        {
            TargetResource = WorldResource.EType.Wood;
            Debug.Log("자원 수집");
        };
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            foreach(var dic in Order)
            {
                if(Input.GetKeyDown(dic.Key))
                {
                    dic.Value();
                }

            }
        }
    }

}
