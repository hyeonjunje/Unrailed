using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TrainMovement : Singleton<TrainMovement>
{
    //public Queue<Transform> nextTarget = new Queue<Transform>();
    [SerializeField] private GameObject[] rails;
    [SerializeField] private int targetCount;
    public float trainMoveSpeed;
    public float trainRotateSpeed;
    Transform startRayPos;
    // Start is called before the first frame update
    private void Awake()
    {
        startRayPos = transform.GetChild(0).GetComponent<Transform>();
    }
    // Update is called once per frame
    void Update()
    {
        TrainMovePos();
    }
    void TrainMovePos()
    {
       // ť�� ���� ���� ��ġ�� �̵�
        transform.position = Vector3.MoveTowards(transform.position, rails[targetCount].transform.position, trainMoveSpeed * Time.deltaTime);
        if (transform.position == rails[targetCount].transform.position)
        {
            targetCount += 1;
        }
        
        transform.LookAt(rails[targetCount].transform.position);

        // var trainRotate = Quaternion.LookRotation(rails[targetCount].transform.position);
        // transform.rotation = Quaternion.Slerp(transform.rotation, trainRotate, trainRotateSpeed * Time.deltaTime);
    }
    public void EnqueueRailPos(Transform transform)
    {
        // Rail�� ��ġ�Ǹ� Queue�� �ֱ�. 
       // nextTarget.Enqueue(transform);
    }
}
