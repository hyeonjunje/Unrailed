using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raven : MonoBehaviour
{

    [SerializeField] private GameObject[] ravenMesh;
    [SerializeField] private float speed;
    private RavenSpawner _spawner;


    private void Awake()
    {
        _spawner = GetComponentInParent<RavenSpawner>();
        ravenMesh = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            ravenMesh[i] = transform.GetChild(i).gameObject;
        }
    }
    private void OnEnable()
    {
        int num = Random.Range(0, 3);

        switch (num)
        {
            case 0:
                for (int i = 0; i < ravenMesh.Length; i++)
                {
                    ravenMesh[i].SetActive(true);
                    if(i != 0)
                    {
                        ravenMesh[i].SetActive(false);
                    }
                }
                break;

            case 1:
                for (int i = 0; i < ravenMesh.Length; i++)
                {
                    ravenMesh[i].SetActive(true);
                    if (i >= 3)
                    {
                        ravenMesh[i].SetActive(false);
                    }
                }
                break;

            case 2:
                for (int i = 0; i < ravenMesh.Length; i++)
                {
                    ravenMesh[i].SetActive(true);
                }
                break;
            default: return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        if (transform.localPosition.x >= _spawner.endPoint.x)
        {
            gameObject.SetActive(false);
        }
    }
    private void OnDisable()
    {
        transform.localPosition = _spawner.spawnPoint;
        _spawner.countPlus++;
        _spawner.SpawnRavens();
    }
}
