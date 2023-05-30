using System.Collections.Generic;
using UnityEngine;

public class TrainEngine : TrainMovement
{
    [SerializeField] private List<GameObject> smokeMesh = new List<GameObject>();
    [Header("In Game Obj Not Prefabs")]
    public List<TrainMovement> trains = new List<TrainMovement>();

    public Animator anim;
    // Start is called before the first frame update
    void Awake()
    {
        GetMesh();

        smokeMesh[0].SetActive(false);
        smokeMesh[1].SetActive(false);
        smokeMesh[1].SetActive(true);
        TryGetComponent(out anim);
    }


    // Update is called once per frame
    void Update()
    {
        TrainMovePos();

        if (!isBurn)
        {
            EngineCool();
            
            if (isReady)
            {
                for (int i = 0; i < trains.Count; i++)
                {
                    trains[i].isReady = true;

                }
                smokeMesh[0].SetActive(false);
                smokeMesh[1].SetActive(false);
            }
            else
            {
                for (int i = 0; i < trains.Count; i++)
                {
                    trains[i].isReady = false;
                }
                anim.SetBool("CountDown", false);

            }
            if (isGoal)
            {
                for (int i = 0; i < trains.Count; i++)
                {
                    trains[i].isGoal = true;
                }
            }
            else
            {
                for (int i = 0; i < trains.Count; i++)
                {
                    trains[i].isGoal = false;
                }
            }
        }

        if(!_isPlay &&!isReady && !isGoal && !isBurn && rails.Count < 5 && !isOver)
        {
            StartCoroutine(Warning());
        }
    }
    public void EngineFire()
    {
        smokeMesh[0].SetActive(true);
        smokeMesh[1].SetActive(false);
    }
    public void EngineCool()
    {
        smokeMesh[0].SetActive(false);
        smokeMesh[1].SetActive(true);
    }
}