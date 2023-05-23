using System.Collections.Generic;
using UnityEngine;

public class TrainEngine : TrainMovement
{
    [SerializeField] private List<GameObject> smokeMesh = new List<GameObject>();

    // Start is called before the first frame update
    void Awake()
    {
        GetMesh();

        smokeMesh[0].SetActive(false);
        smokeMesh[1].SetActive(false);
        smokeMesh[1].SetActive(true);
    }


    // Update is called once per frame
    void Update()
    {
        TrainMovePos();

        if (!isBurn)
        {
           EngineCool();
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