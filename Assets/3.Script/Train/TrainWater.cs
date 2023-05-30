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
    [SerializeField] List<ParticleSystem> waterDestroyList= new List<ParticleSystem>();


    // �ÿ��϶� R = 0 G = 10 B = 8 
    // �����϶� R = 10 G = 3 B = 3 


    [SerializeField] private float _R;
    [SerializeField] private float _G;
    [SerializeField] private float _B;

    int engineOver;
    int boxOver;
    int benchOver;
    int spareOver;
    #region ���� ���� ����
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
        TrainMovePos();
 
        if (!isReady)
        {
            fireTime += Time.deltaTime;
            FireColor();
            FireOn();
        }

        if (!_isPlay && !isReady && !isGoal && isBurn && !isOver)
        {
            StartCoroutine(Warning());
        }
    }
    
    public override void TrainUpgrade()
    {
       
        base.TrainUpgrade();
        //���׷��̵� �޼���
        switch (trainUpgradeLevel)
        {
            case 1:
                engineOver = 70;
                boxOver = 80;
                benchOver = 90;
                spareOver = 100;
                overFireTime = 120;
                break;
            case 2:
                engineOver = 90;
                boxOver = 100;
                benchOver = 110;
                spareOver = 120;
                overFireTime = 140;
                break;
           default:
                engineOver = 110;
                boxOver = 120;
                benchOver = 130;
                spareOver = 140;
                overFireTime = 160;
                break;
        }
    }
    public void FireOn()
    {
        //��Ÿ����
        if (fireLimit < fireTime)
        {
            isBurn = true;

            Fire();

            fireEffect.gameObject.SetActive(true);


            //���� ����
            if (overFireTime < fireTime)
            {
                for (int i = 0; i < trains.Length; i++)
                {
                    fireTime = 0;
                    TrainOver();
                    //bool������ ���ӿ��� �� �ð��� �Ȱ��� ���ǹ� �����
                }
            }
        }
    }

    public void FireOff()
    {
        fireTime = 0;
        isBurn = false;
        _Red = 0;
        _Green = 100;
        _Blue = 80;

        for (int i = 0; i < trains.Length; i++)
        {
            if(trains[i].trainType == TrainType.Engine)
            {
                trains[i].GetComponent<TrainEngine>().EngineCool();
            }
            trains[i].isBurn = false;
            if(trains[i].trainType != TrainType.Spare)
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
            var main = waterDestroyList[i].main;
            waterColorList[i].material.color = new Color(_Red * 0.01f, _Green * 0.01f, _Blue * 0.01f);
            main.startColor = new Color(_Red * 0.01f, _Green * 0.01f, _Blue * 0.01f);
        }
    }
    private void Fire()
    {
        if (engineOver < fireTime)
        {
            for (int i = 0; i < trains.Length; i++)
            {
                if (trains[i].trainType == TrainType.Engine)
                {
                    trains[i].isBurn = true;
                    trains[i].GetComponent<TrainEngine>().EngineFire();
                }
            }
        }

        if (boxOver < fireTime)
        {
            for (int i = 0; i < trains.Length; i++)
            {
                if (trains[i].trainType == TrainType.ChestBox)
                {
                    trains[i].isBurn = true;
                    trains[i].fireEffect.gameObject.SetActive(true);
                }
            }
        }

        if (benchOver < fireTime)
        {
            for (int i = 0; i < trains.Length; i++)
            {
                if (trains[i].trainType == TrainType.WorkBench)
                {
                    trains[i].isBurn = true;
                    trains[i].fireEffect.gameObject.SetActive(true);
                }
            }
        }

        // if (spareOver < fireTime)
        // {
        //     for (int i = 0; i < trains.Length; i++)
        //     {
        //         if (trains[i].trainNum == 3)
        //         {
        //             trains[i].isBurn = true;
        //             trains[i].fireEffect.gameObject.SetActive(true);
        //         }
        //     }
        // }
    }

}
