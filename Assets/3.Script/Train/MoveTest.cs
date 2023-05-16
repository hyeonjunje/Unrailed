using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class MoveTest : MonoBehaviour
{
    private NavMeshAgent nav;

    [SerializeField] private Transform target;
    public float curSpeed;
    // Start is called before the first frame update
    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
    }
    // Update is called once per frame
    void Update()
    {
        curSpeed = nav.speed;
        nav.SetDestination(target.position);

        if(nav.speed == 0)
        {
            Debug.Log("Stop");
        }

        
    }
}
