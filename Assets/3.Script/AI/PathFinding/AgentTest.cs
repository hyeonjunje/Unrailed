using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentTest : MonoBehaviour
{
    [SerializeField] private Transform target;

    private PathFindingAgent agent;

    private void Awake()
    {
        agent = GetComponent<PathFindingAgent>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            agent.MoveTo(target.position);
        }
        if(Input.GetKeyDown(KeyCode.Z))
        {
            agent.MoveToRandomPosition();
        }
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            agent.MoveToClosestEndPosition();
        }
    }
}
