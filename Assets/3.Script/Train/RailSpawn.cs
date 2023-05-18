using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailSpawn : MonoBehaviour
{

    public GameObject prefabs;

    public Vector3 bottomLeft;
    public Vector3 topRight;

    public void SpawnRandRail()
    {
        int randX = Random.Range((int)bottomLeft.x, (int)topRight.x);
        int randz = Random.Range((int)bottomLeft.z, (int)topRight.z);
        Vector3 randVec = new Vector3(randX, -1.94f, randz);
        Instantiate(prefabs, randVec, Quaternion.identity);
       //GameObject prefab = Instantiate(prefabs, randVec, Quaternion.identity);
       //prefab.AddComponent<TrainSpawnRail>().EnqueueRail();
    }
}
