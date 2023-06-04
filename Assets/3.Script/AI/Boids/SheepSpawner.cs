using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepSpawner : MonoBehaviour
{

    [SerializeField] private FlockBT FlockPrefab;
    [SerializeField] private float _spawnRadius = 10;
    [SerializeField] private int _spawnCount = 10;

    void Awake()
    {
        for (int i = 0; i < _spawnCount; i++)
        {
            Vector3 pos = transform.position + Random.insideUnitSphere * _spawnRadius;
            FlockBT flock = Instantiate(FlockPrefab);
            flock.transform.position = new Vector3(pos.x, 0.5f, pos.z);
        }
    }


}
