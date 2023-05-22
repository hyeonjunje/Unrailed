using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainEngine : TrainMovement
{
    // Start is called before the first frame update
    void Awake()
    {
        GetMesh();
        fireEffect.Play();

    }


    // Update is called once per frame
    void Update()
    {

        TrainMovePos();
    }
}