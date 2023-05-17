using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TrainSpawnRail : MonoBehaviour
{
    private TrainMovement[] trainComponents;
    private void Awake()
    {
        trainComponents = FindObjectsOfType<TrainMovement>();
    }
    private void OnEnable()
    {
        for (int i = 0; i < trainComponents.Length; i++)
        {
            trainComponents[i].EnqueueRailPos(gameObject);
        }
    }
}
