using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailController : MonoBehaviour
{
    [SerializeField] private GameObject[] railPrefabs;
    private Transform[] railChild;
    [SerializeField] private RailController neighborRail;
    [SerializeField] private List<bool> dirList = new List<bool>();

    private TrainSpawnRail _trainPos;

    
    public int dirCount;

    public bool isInstance;

    public bool isFront;
    public bool isBack;
    public bool isLeft;
    public bool isRight;

    private Vector3 _frontPos;
    private Vector3 _backPos;
    private Vector3 _rightPos;
    private Vector3 _leftPos;

    private void Awake()
    {
        _trainPos = GetComponentInChildren<TrainSpawnRail>();
        _frontPos = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1.3f);
        _backPos = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1.3f);
        _rightPos = new Vector3(transform.position.x + 1.3f, transform.position.y, transform.position.z);
        _leftPos = new Vector3(transform.position.x - 1.3f, transform.position.y, transform.position.z);

        railChild = this.GetComponentsInChildren<Transform>();


    }
    private void OnEnable()
    {
        foreach (Transform child in railChild)
        {
            child.gameObject.layer = 23;
        }
        isInstance = false;
        _trainPos.EnqueueRail();
        RaycastOn();
    }

    public void RailSwitch()
    {
        for (int i = 0; i < railPrefabs.Length; i++)
        {
            railPrefabs[i].SetActive(true);
            if (dirCount != i)
            {
                railPrefabs[i].SetActive(false);
            }
        }
    }
    public void RaycastOn()
    {
        RaycastHit raycastHit;

        isFront = Physics.Raycast(_frontPos, transform.forward, 1.4f, LayerMask.GetMask("Rail"));
        isBack = Physics.Raycast(_backPos, -transform.forward, 1.4f, LayerMask.GetMask("Rail"));
        isRight = Physics.Raycast(_rightPos, transform.right, 1.4f, LayerMask.GetMask("Rail"));
        isLeft = Physics.Raycast(_leftPos, -transform.right, 1.4f, LayerMask.GetMask("Rail"));


        if ((Physics.Raycast(_frontPos, transform.forward, out raycastHit, 1) && !raycastHit.collider.GetComponentInParent<RailController>().isInstance)
            || (Physics.Raycast(_rightPos, transform.right, out raycastHit, 1 )&& !raycastHit.collider.GetComponentInParent<RailController>().isInstance) 
            || (Physics.Raycast(_backPos, -transform.forward, out raycastHit, 1) && !raycastHit.collider.GetComponentInParent<RailController>().isInstance)
            || (Physics.Raycast(_leftPos, -transform.right, out raycastHit, 1) && !raycastHit.collider.GetComponentInParent<RailController>().isInstance))
        {
            neighborRail = raycastHit.collider.GetComponentInParent<RailController>();

            if (neighborRail != null && !neighborRail.isInstance)
            {
                if (isFront) neighborRail.isBack = true;
                if (isRight) neighborRail.isLeft = true;
                if (isBack) neighborRail.isFront = true;
                if (isLeft) neighborRail.isRight = true;

 
                neighborRail.isInstance = true;
                neighborRail.railDirSelet();
                neighborRail.RailSwitch();

                foreach (Transform child in neighborRail.railChild)
                {
                    child.gameObject.layer = 0;
                }
            }
        }
        railDirSelet();
        RailSwitch();
    }
    private void railDirSelet()
    {
        if (isFront || isBack) dirCount = 5;
        if (isRight || isLeft) dirCount = 0;
        if (isFront && isRight) dirCount = 3;
        else if (isFront && isLeft) dirCount = 1;
        else if (isBack && isRight) dirCount = 4;
        else if (isBack && isLeft) dirCount = 2;
    }
    void Update()
    {
        Debug.DrawRay(_frontPos, transform.forward * 1, Color.red);
        Debug.DrawRay(_backPos, -transform.forward * 1, Color.green);
        Debug.DrawRay(_rightPos, transform.right * 1, Color.yellow);
        Debug.DrawRay(_leftPos, -transform.right * 1, Color.blue);
    }
}
