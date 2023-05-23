using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainWater : TrainMovement
{
    public float fireTime;
    public float overFireTime;

    [SerializeField] private float fireLimit;
    [SerializeField] TrainMovement[] trains;

    [SerializeField] List<Renderer> waterColorList = new List<Renderer>();


    // 냉열일땐 R = 0 G = 10 B = 8 
    // 과열일땐 R = 10 G = 3 B = 3 


    [SerializeField] private float _R;
    [SerializeField] private float _G;
    [SerializeField] private float _B;
    #region 과열 색상 설정
    private float _Red
    {
        get { return _R; }
        set
        {
            _R = value;
            _R = Mathf.Clamp(_R, 0, 100);
        }
    }
    private float _Blue
    {
        get { return _B; }
        set
        {
            _B = value;
            _B = Mathf.Clamp(_B, 30, 80);
        }
    }
    private float _Green
    {
        get { return _G; }
        set
        {
            _G = value;
            _G = Mathf.Clamp(_G, 30, 100);
        }
    }
    #endregion 

    // Start is called before the first frame update
    void Awake()
    {
        GetMesh();
        trains = FindObjectsOfType<TrainMovement>();

    }

    private void OnEnable()
    {
        fireTime = 0;
        GetMesh();
        fireEffect.gameObject.SetActive(false);
        _Red = 0;
        _Green = 100;
        _Blue = 80;
    }

    void Update()
    {
        fireTime += Time.deltaTime;
        TrainMovePos();
        FireColor();
        FireOn();
    }

    public void FireOn()
    {
        //불타는중
        if (fireLimit < fireTime)
        {
            isBurn = true;

            Fire();

            fireEffect.gameObject.SetActive(true);


            //게임 오버
            if (overFireTime < fireTime)
            {
                for (int i = 0; i < trains.Length; i++)
                {
                    trains[i].gameObject.SetActive(false);
                    fireTime = 0;
                    //bool값으로 게임오버 시 시간이 안가는 조건문 만들기
                }
            }
        }
    }

    public void FireOff()
    {
        fireTime = 0;
        isBurn = false;

  
        for (int i = 0; i < trains.Length; i++)
        {
            if(trains[i].trainNum == 0)
            {
                trains[i].GetComponent<TrainEngine>().EngineCool();
            }
            trains[i].isBurn = false;
            trains[i].fireEffect.gameObject.SetActive(false);
        }
    }

    private void FireColor()
    {
        _Red += Time.deltaTime * 4f;

        if (_Red > 50)
        {
            _Green -= Time.deltaTime * 4f;
        }
        if (_Green < 50)
        {
            _Blue -= Time.deltaTime * 1.5f;

        }

        for (int i = 0; i < waterColorList.Count; i++)
        {
            waterColorList[i].material.color = new Color(_Red * 0.01f, _Green * 0.01f, _Blue * 0.01f);
        }
    }
    private void Fire()
    {
        if (70 < fireTime)
        {
            for (int i = 0; i < trains.Length; i++)
            {
                if (trains[i].trainNum == 0)
                {
                    trains[i].isBurn = true;
                    trains[i].GetComponent<TrainEngine>().EngineFire();
                }
            }
        }

        if (80 < fireTime)
        {
            for (int i = 0; i < trains.Length; i++)
            {
                if (trains[i].trainNum == 1)
                {
                    trains[i].isBurn = true;
                    trains[i].fireEffect.gameObject.SetActive(true);
                }
            }
        }

        if (90 < fireTime)
        {
            for (int i = 0; i < trains.Length; i++)
            {
                if (trains[i].trainNum == 2)
                {
                    trains[i].isBurn = true;
                    trains[i].fireEffect.gameObject.SetActive(true);
                }
            }
        }
    }

}
