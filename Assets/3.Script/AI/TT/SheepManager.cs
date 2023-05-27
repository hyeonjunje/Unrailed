using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepManager : MonoBehaviour
{
    public Transform Target;
    private Flock[] _flock;
    void Start()
    {
        _flock = FindObjectsOfType<Flock>();
        foreach (Flock sheep in _flock)
        {
            sheep.Init(Target);
        }

    }
    void Update()
    {
        if(_flock != null)
        {
            int numFlock = _flock.Length;


            var flockData = new BoidData[numFlock];

            for (int i = 0; i < _flock.Length; i++)
            {
                flockData[i].Position = _flock[i].Position;
            }

            for (int i = 0; i < _flock.Length; i++)
            {
                _flock[i].CentreOfFlockmates = flockData[i].FlockCentre;
                _flock[i].NumPerceivedFlockmates = _flock.Length;

                _flock[i].UpdateFlock();
            }



        }
    }


    public struct BoidData
    {
        public Vector3 Position;
        public Vector3 FlockCentre;
    }


}
