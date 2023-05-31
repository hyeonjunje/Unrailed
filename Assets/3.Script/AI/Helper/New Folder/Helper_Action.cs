using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Helper_Action : MonoBehaviour
{
    public enum EState
    {
        Wood,
        Stone,
        Tracks,
        Sleep

    }

    public Dictionary<KeyCode, System.Action> Order;
    public EState HelperState = EState.Wood;

    private void Start()
    {
        Order = new Dictionary<KeyCode, Action>();
        Init();

    }

    void Init()
    {
        Order[KeyCode.Alpha1] = () =>
        {
            //여기서 가능한 명령인지 확인하는거 나중에 추가하기(바닥에 도구가 있나요?)
            HelperState = EState.Wood;
            Debug.Log("나무 캐세용");
        };

        Order[KeyCode.Alpha2] = () =>
        {
            HelperState = EState.Stone;
            Debug.Log("돌 캐세용");
        };

        Order[KeyCode.Alpha3] = () =>
        {
            HelperState = EState.Tracks;
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
