using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TrainSpawnRail : MonoBehaviour
{
   [SerializeField] private TrainMovement[] trainComponents;

    public void EnqueueRail()
    {
        trainComponents = FindObjectsOfType<TrainMovement>();

        for (int i = 0; i < trainComponents.Length; i++)
        {
            trainComponents[i].EnqueueRailPos(gameObject);
            Debug.Log("기차에 위치값 추가");
        }
    }
}
