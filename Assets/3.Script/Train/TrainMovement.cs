using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class TrainMovement : MonoBehaviour
{
    public Queue<GameObject> rails = new Queue<GameObject>();
    //public List<GameObject> rails = new List<GameObject>();
    //[SerializeField] private GameObject[] rails;
    [SerializeField] private int targetCount;
    public float trainMoveSpeed;
    public float trainRotateSpeed;
    Transform startRayPos;

    public List<GameObject> listToQue = new List<GameObject>();

    // Start is called before the first frame update
    private void Awake()
    {
        startRayPos = transform.GetChild(0).GetComponent<Transform>();
    }
    // Update is called once per frame
    void Update()
    {
        listToQue = rails.ToList();

        TrainMovePos();
    }
    void TrainMovePos()
    {
        if(rails.Count != 0)
        {
            // 큐에 들어온 레일 위치로 이동
            transform.position = Vector3.MoveTowards(transform.position, rails.Peek().transform.position, trainMoveSpeed * Time.deltaTime);
            transform.LookAt(rails.Peek().transform.position);

            if (transform.position == rails.Peek().transform.position)
            {
                rails.Dequeue();
            }


            // var trainRotate = Quaternion.LookRotation(rails[targetCount].transform.position);
            // transform.rotation = Quaternion.Slerp(transform.rotation, trainRotate, trainRotateSpeed * Time.deltaTime);
        }
        else
        {
            Destroy(gameObject);
        }

    }
    public void EnqueueRailPos(GameObject gameObject)
    {
        //rails.Add(gameObject);
        rails.Enqueue(gameObject);
    }
}
