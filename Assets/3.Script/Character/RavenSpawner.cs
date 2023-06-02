using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RavenSpawner : MonoBehaviour
{
    [SerializeField] private GameObject prefabs;
    [SerializeField] private GameObject[] ravens;

    [SerializeField] private int spawnCount;
    public int countPlus;

    public Vector3 spawnPoint;
    public Vector3 endPoint;
    public Vector3 poolPoint;

    private void Awake()
    {
        ravens = new GameObject[spawnCount];
        for (int i = 0; i < ravens.Length; i++)
        {
            ravens[i] = Instantiate(prefabs, poolPoint, Quaternion.identity, transform);
            ravens[i].SetActive(false);
        }
        SpawnRavens();
    }
    // Update is called once per frame
    public void SpawnRavens()
    {
        if (spawnCount == countPlus) countPlus = 0;

        ravens[countPlus].transform.position = spawnPoint;
        ravens[countPlus].SetActive(true);

    }
}
