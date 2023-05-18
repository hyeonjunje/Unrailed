using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailController : MonoBehaviour
{
    [SerializeField] GameObject[] railPrefabs;

    private TrainSpawnRail _trainPos;

    public int dirCount;

    public bool isInstance;

    public bool isFront;
    public bool isBack;
    public bool isLeft;
    public bool isRight;

    private void Awake()
    {
        _trainPos = GetComponentInChildren<TrainSpawnRail>();
    }
    private void OnEnable()
    {
        _trainPos.EnqueueRail();
        for (int i = 0; i < railPrefabs.Length; i++)
        {
            railPrefabs[i].SetActive(true);
            if (dirCount != i)
            {
                railPrefabs[i].SetActive(false);
            }
        }
    }

    
    public void RailSwitch()
    {

    }
    public void RaycastOn()
    {
        Vector3 frontPos = new Vector3(transform.position.x, transform.position.y , transform.position.z + 1.8f);
        Vector3 backPos = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1.8f);
        Vector3 rightPos = new Vector3(transform.position.x+ 1.8f, transform.position.y, transform.position.z);
        Vector3 leftPos = new Vector3(transform.position.x- 1.8f, transform.position.y, transform.position.z);

        Debug.DrawRay(frontPos, transform.forward * 1, Color.red);
        Debug.DrawRay(backPos, -transform.forward * 1, Color.green);
        Debug.DrawRay(rightPos, transform.right * 1, Color.yellow);
        Debug.DrawRay(leftPos, -transform.right * 1, Color.blue);

        isFront = Physics.Raycast(frontPos, transform.forward, 1, LayerMask.GetMask("Rail"));
        isBack = Physics.Raycast(backPos, -transform.forward, 1, LayerMask.GetMask("Rail"));
        isRight = Physics.Raycast(rightPos, transform.right, 1, LayerMask.GetMask("Rail"));
        isLeft = Physics.Raycast(leftPos, -transform.right, 1, LayerMask.GetMask("Rail"));
    }
    void Update()
    {
        RaycastOn();
    }
}
