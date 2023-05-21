using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EOffmeshLinkStatus
{
    NotStarted,
    InProgress
}

[RequireComponent(typeof(NavMeshAgent))]
public class CharacterAgent : CharacterBase
{
    [SerializeField] private float _nearestPointSearchRange = 5f;

    private NavMeshAgent _agent;

    //목적지 설정이 되었는지
    private bool _destinationSet = false;

    //목적지에 닿았는지
    private bool _reachedDestination = false;
    EOffmeshLinkStatus OffMeshLinkStatus = EOffmeshLinkStatus.NotStarted;

    //AI가 움직이고 있으면 true 아니면 false
    public bool isMoving => _agent.velocity.magnitude > float.Epsilon;
    public bool AtDestination => _reachedDestination;
    public bool DestinationSet => _destinationSet;

    protected void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        
    }

    protected void Update()
    {
        //탐색중이 아니고, 목적지 설정이 되었고, 남은 거리가 멈춤 거리보다 적으면
        if ((_agent.remainingDistance <= _agent.stoppingDistance)&&_destinationSet&&!_agent.pathPending)
        //if (!_agent.pathPending && _destinationSet && !_agent.isOnOffMeshLink&&(_agent.remainingDistance <= _agent.stoppingDistance))
        {
            //도착으로 간주   
            _destinationSet = false;
            _reachedDestination = true;
        }

        if(_agent.remainingDistance > _agent.stoppingDistance&&_destinationSet)
        {
            _reachedDestination = false;
        }


        if (_agent.isOnOffMeshLink) //오프 메시 링크 위에 있고
        {
            if (OffMeshLinkStatus == EOffmeshLinkStatus.NotStarted) //시작하지 않았다면
                StartCoroutine(FollowOffmeshLink()); // 쫓아가기
        }

    }

    IEnumerator FollowOffmeshLink()
    {
        //네비메시 컨트롤비활성화
        OffMeshLinkStatus = EOffmeshLinkStatus.InProgress;
        _agent.updatePosition = false;
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;

        Vector3 newPosition = transform.position;
        //목적지 도착할 때까지 이동
        while (!Mathf.Approximately(Vector3.Distance(newPosition, _agent.currentOffMeshLinkData.endPos), 0f))
        {
            newPosition = Vector3.MoveTowards(transform.position, _agent.currentOffMeshLinkData.endPos, _agent.speed * Time.deltaTime);
            transform.position = newPosition;

            yield return new WaitForEndOfFrame();
        }

        // 도착
        OffMeshLinkStatus = EOffmeshLinkStatus.NotStarted;
        _agent.CompleteOffMeshLink();

        //네비메시 컨트롤 활성화
        _agent.updatePosition = true;
        _agent.updateRotation = true;
        _agent.updateUpAxis = true;
    }

    public void StopNav()
    {
        _agent.ResetPath();
        _agent.updatePosition = false;
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }


    public Vector3 PickLocationInRange(float range) //무작위 이동
    {
        Vector3 searchLocation = transform.position;
        searchLocation += Random.Range(-range, range) * Vector3.forward;
        searchLocation += Random.Range(-range, range) * Vector3.right;

        NavMeshHit hitResult;
        if (NavMesh.SamplePosition(searchLocation, out hitResult, _nearestPointSearchRange, NavMesh.AllAreas))
            return hitResult.position;

        return transform.position;
    }

    public virtual void CancelCurrentCommand() //목적지 초기화
    {
        _agent.ResetPath();

        _destinationSet = false;
        _reachedDestination = false;
        OffMeshLinkStatus = EOffmeshLinkStatus.NotStarted;
    }

    public virtual void MoveTo(Vector3 destination) // 목적지로 이동
    {
        CancelCurrentCommand(); // 리셋하고
        SetDestination(destination); //목적지 설정을 하고

    }


    public virtual void SetDestination(Vector3 destination) // 목적지 설정하기
    {
        NavMeshHit hitResult;
        if (NavMesh.SamplePosition(destination, out hitResult, _nearestPointSearchRange, NavMesh.AllAreas)
            )
        {
            _agent.SetDestination(hitResult.position);
            _destinationSet = true;
            _reachedDestination = false;
        }
    }





}
